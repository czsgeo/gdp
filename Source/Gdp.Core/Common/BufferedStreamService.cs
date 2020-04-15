//2016.03.27, czs, create in hongqing, 数据缓存
//2016.08.29, czs, edit in hongqing, 缓存采用窗口对象管理
//2017.09.02, czs, edit in hongqing, 实现反向数据流

using System;
using System.IO;
using System.Collections.Generic;
using Gdp.IO; 

namespace Gdp
{ 
    /// <summary>
    /// 数据流缓存数据读取器。
    /// 缓存数据，不包含第一个项目。
    /// </summary>
    /// <typeparam name="TMaterial"></typeparam>
    public class BufferedStreamService<TMaterial> : AbstractEnumer<TMaterial>, IBufferedMaterial<TMaterial>
    {
        Log log = new Log(typeof(BufferedStreamService<TMaterial>));

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="Stream"></param>
        /// <param name="bufferSize"></param>
        public BufferedStreamService(IEnumer<TMaterial> Stream, int bufferSize = 50)
        {
            this.Name = Stream.Name;
            this.DataSource = Stream; 
            this.MaterialBuffers = new WindowData<TMaterial>(bufferSize);
            log.Info("数据流缓存大小：" + bufferSize);
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="Stream"></param>
        /// <param name="bufferSize"></param>
        public BufferedStreamService(IEnumerable<TMaterial> Stream, int bufferSize = 50)
        {
            this.Name = "无名数据流";
            this.DataSource = new SimpleEnumer<TMaterial>(Stream); 
            this.MaterialBuffers = new WindowData<TMaterial>(bufferSize);
            log.Info("数据流缓存大小：" + bufferSize);
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumer<TMaterial> DataSource { get; set; }

        #region 流程控制

        /// <summary>
        /// 起始编号，从0开始。
        /// </summary>
        public override int StartIndex { get { return DataSource.StartIndex; } set { DataSource.StartIndex = value; } }
        /// <summary>
        /// 遍历数量，默认为最大值的一半。
        /// </summary>
        public override int EnumCount { get { if (DataSource != null) return DataSource.EnumCount; return 0; } set { if (DataSource != null)  DataSource.EnumCount = value; } }
        /// <summary>
        /// 当前编号
        /// </summary>
        public override int CurrentIndex { get { return DataSource.CurrentIndex; } set { DataSource.CurrentIndex = value; } }

        #endregion


        #region 事件和触发函数
        /// <summary>
        /// 0.从数据流中读入了原料，并通过了检核，但还没有加入缓存
        /// </summary>
        public event MaterialEventHandler<TMaterial> MaterialInputted;
        /// <summary>
        /// 1.对刚刚读取进入的材料进行检核，如果不合格则不加入缓存。
        /// </summary>
        public event MaterialCheckEventHandler<TMaterial> MaterialCheck;
        /// <summary>
        /// 2.数据通过了检核。
        /// </summary>
        public event MaterialEventHandler<TMaterial> AfterMaterialCheckPassed;
        /// <summary>
        /// 3.准备处理原料，可以在此预处理原料，如根据缓存情况处理原料。
        /// </summary>
        public event MaterialEventHandler<TMaterial> MaterialProducing;
        /// <summary>
        /// 当缓存填充满或数据流读完时激发。激发一次。
        /// </summary>
        public event Action<IWindowData<TMaterial>> MaterialBuffersFullOrEnd;
        /// <summary>
        /// 原始数据中，读取到了数据结束！
        /// </summary>
        public event Action MaterialEnded;
        /// <summary>
        /// 缓存也已经读完，结束时激发。
        /// </summary>
        public event Action Completed;

        #region 事件触发
        /// <summary>
        /// 结束了
        /// </summary>
        /// <param name="m"></param>
        protected virtual void OnCompleted() { Completed?.Invoke(); }
        /// <summary>
        /// 从数据流读取一个原料触发，并且已经通过了检核，此时尚未加入缓存。
        /// </summary>
        protected virtual void OnMaterialInputted(TMaterial m) { MaterialInputted?.Invoke(m); }
        /// <summary>
        /// 对刚刚读取进入的材料进行检核，如果不合格则不加入缓存。
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public bool OnMaterialCheck(TMaterial material) { if (MaterialCheck != null) { return MaterialCheck(material); } return true; }
        /// <summary>
        /// 检核通过后激发
        /// </summary>
        /// <param name="material"></param>
        protected virtual void OnAfterMaterialCheckPassed(TMaterial material) { AfterMaterialCheckPassed?.Invoke(material); }
        /// <summary>
        /// 生产前触发,传入当前原料。
        /// </summary>
        /// <param name="material"></param>
        protected virtual void OnMaterialProducing(TMaterial material) { MaterialProducing?.Invoke(material); }
       /// <summary>
       /// 填满时激发，或者没有填满，但是已经结束了。
       /// </summary>
        /// <param name="materials"></param>
        protected virtual void OnMaterialBuffersFullOrEnd(IWindowData<TMaterial> materials) { MaterialBuffersFullOrEnd?.Invoke(materials); }
        
        /// <summary>
        /// 原料用完，触发
        /// </summary>
        protected virtual void OnMaterialEnded()
        {
            IsMaterialEnded = true;
            log.Debug(Name + "输入数据已到末尾");
            MaterialEnded?.Invoke();
        }
        #endregion
        #endregion

        #region 主要方法
        /// <summary>
        /// 移动到下一条，如要设置了缓存，则先填充缓存。
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (this.DataSource.IsCancel || IsCancel) { log.Info(Name + "缓存数据流已经被取消！"); OnCompleted();  return false; }

            //先获取一个原料
            TMaterial next = default(TMaterial);
            if (!IsMaterialEnded) { 
                next = GetNextMaterial(); 
            }//先获取一个原料

            //如果为空，则尝试读取缓存，并直接返回结果
            if (next == null) {
                return TryGetCurrentFromBuffer(); 
            }

            //检核是否合格,不合格则继续读取
            if (!OnMaterialCheck(next)) { return MoveNext(); }
            else { OnAfterMaterialCheckPassed(next); }

            //检查缓存是否已满,若是，弹出最先进入的一个作为当前结果，并添加缓存
            if (MaterialBuffers.IsFull)
            {
                this.Current = MaterialBuffers.Pop();
                OnMaterialProducing(this.Current);
                AppendMaterialToBuffer(next);
                return true;
            }
            else//没有满，则继续添加
            {
                AppendMaterialToBuffer(next);
                if (MaterialBuffers.IsFull)
                {                    
                    log.Info(Name + "缓存数据流已经填满！");
                    OnMaterialBuffersFullOrEnd(MaterialBuffers);
                }

                //直接快速填满，避免迭代过多，堆栈溢出
                for (int i =MaterialBuffers.Count; i < MaterialBuffers.WindowSize; i++)
                {
                     next = GetNextMaterial();
                     //如果为空，则尝试读取缓存，并直接返回结果
                     if (next == null)
                     {
                         return TryGetCurrentFromBuffer();
                     }
                     //检核是否合格,不合格则继续读取
                     if (!OnMaterialCheck(next)) { return MoveNext(); }
                     else { OnAfterMaterialCheckPassed(next); }

                     AppendMaterialToBuffer(next);
                } 

                return MoveNext();
            }
        }

        //从缓存中读取
        private bool TryGetCurrentFromBuffer()
        {
            //读取缓存
            TMaterial material = MaterialBuffers.Pop();
            if (material == null) { log.Info(Name + "缓存数据读取结束！"); OnCompleted();  return false; }

            this.Current = (material);
            return true;
        }

        /// <summary>
        /// 获取数据流中的下一个原材料。
        /// </summary>
        /// <returns></returns>
        protected virtual TMaterial GetNextMaterial()
        {
            var istrue = this.DataSource.MoveNext();
            if (istrue && !this.IsCancel)
            {
                var m = this.DataSource.Current;
                return m;
            }
            if (!istrue)
            {
                //数据读完还没有填满。
                if (!MaterialBuffers.IsFull)
                {
                    log.Info(Name + "缓存数据流读完了，但缓存没有填满，直接激发事件 MaterialBuffersFullOrEnd！");
                    OnMaterialBuffersFullOrEnd(MaterialBuffers);
                }
                OnMaterialEnded();
            }
            return default(TMaterial);
        }

        /// <summary>
        /// 添加原料到缓存，这是一个原料入口。
        /// </summary>
        /// <param name="nextBufferMaterial"></param>
        private void AppendMaterialToBuffer(TMaterial nextBufferMaterial)
        {
            OnMaterialInputted(nextBufferMaterial);

            MaterialBuffers.Add(nextBufferMaterial);
        }


        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose() { if (DataSource != null) { DataSource.Dispose(); this.MaterialBuffers.Dispose(); DataSource = null; } }
        /// <summary>
        /// 重置数据
        /// </summary>
        public override void Reset() { DataSource.Reset(); }

        #region 缓存
        /// <summary>
        /// 指示是否已经读完。
        /// </summary>
        public bool IsMaterialEnded { get; set; }
        /// <summary>
        /// 缓存大小，通常用于预读取，预处理数据。
        /// 默认为 1 ，表示实时处理，不需要缓存。
        /// </summary>
        public int BufferSize { get { return MaterialBuffers.WindowSize; } set { MaterialBuffers.WindowSize = value; } }
        /// <summary>
        /// 缓存原料，为预处理后的原料。
        /// 不包含当前原料。
        /// </summary>
        public IWindowData<TMaterial> MaterialBuffers { get; protected set; }
        /// <summary>
        /// 当前原料缓存的大小
        /// </summary>
        public int MaterialBufferSize { get { return MaterialBuffers.Count; } }
        /// <summary>
        /// 获取最后一次进入缓存的原料。
        /// </summary>
        public TMaterial LastBufferedMaterial { get { return MaterialBuffers.Last; } }
        #endregion

        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.DataSource.Name;
        }
    }


    /// <summary>
    /// 批量服务。规定了生产流程。
    /// </summary>
    /// <typeparam name="TMaterial">原料或者查询条件</typeparam>
    /// <typeparam name="TProduct">产品或计算结果</typeparam>
    public abstract class AbstractBufferdStreamService<TProduct, TMaterial> :
        AbstractStreamService<TProduct, TMaterial>,
        IBufferedStreamService<TProduct, TMaterial>
    {
        /// <summary>
        /// 日志记录。错误信息记录在日志里面。
        /// </summary>
        protected new Log log = new Log(typeof(AbstractBufferdStreamService<TProduct, TMaterial>));

        /// <summary>
        /// 构造函数。
        /// </summary>
        public AbstractBufferdStreamService()
        {
            this.IsIgnoreError = true;
            this.IsPostCheckEnabled = true;
            this.BufferSize = 1;
            this.IsMaterialEnded = false;          
        }          

        #region 属性

        #region 缓存 
        /// <summary>
        /// 缓存大小，通常用于预读取，预处理数据。
        /// 默认为 1 ，表示实时处理，不需要缓存。
        /// </summary>
        public int BufferSize { get; set; }
        ///// <summary>
        ///// 缓存原料，为预处理后的原料。
        ///// </summary>
        //public virtual IWindowData<TMaterial> MaterialBuffers { get; set; }
        /// <summary>
        /// 当前原料缓存的大小
        /// </summary>
        public int MaterialBufferSize { get { return MaterialBuffers.Count; } }
        /// <summary>
        /// 获取最后一次缓存的原料。
        /// </summary>
        public TMaterial LastBufferedMaterial { get { if (MaterialBufferSize > 0) return MaterialBuffers[MaterialBufferSize - 1]; return default(TMaterial); } }
        #endregion   
        #endregion

        #region  方法 
         

        #region  通用接口
        /// <summary>
        /// 移动到下一条，如要设置了缓存，则先填充缓存。
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            //先获取一个原料
            TMaterial nextBufferMaterial = default(TMaterial);
            if (!IsMaterialEnded)
            {
                //先获取一个原料
                nextBufferMaterial = GetNextMaterial();
            }

            //如果为空，则尝试读取缓存
            if (nextBufferMaterial == null)
            {
                //读取缓存
                nextBufferMaterial = PopMaterial();
                if (nextBufferMaterial == null) { log.Debug(Name + "缓存数据读取结束！"); return false; }
                log.Debug(Name + "读取缓存数据");

                Get(nextBufferMaterial);
                return true;
            }

            //不为空，则追加到缓存
            AppendMaterialToBuffer(nextBufferMaterial);

            //若缓存不足，则继续填充
            while (MaterialBufferSize < BufferSize)
            {
                nextBufferMaterial = GetNextMaterial();
                if (nextBufferMaterial == null)
                {
                    break;
                }
                AppendMaterialToBuffer(nextBufferMaterial);
            }

            //获取原料
            var nextMaterial = PopMaterial();

            //this.Current = Get(nextMaterial);


            Get(nextMaterial);

            //最后移除第一个，为先的腾位置
            //RemoveFirstMaterial();
            return true;
        }

        /// <summary>
        /// 添加原料到缓存，这是一个原料入口。
        /// </summary>
        /// <param name="nextBufferMaterial"></param>
        private void AppendMaterialToBuffer(TMaterial nextBufferMaterial)
        {
            OnMaterialInputted(nextBufferMaterial);

            MaterialBuffers.Add(nextBufferMaterial);
        }

        /// <summary>
        /// 返回队列中第一个，如果队列为空，则返回 null;
        /// </summary>
        /// <returns></returns>
        private TMaterial PopMaterial()
        {
            if (MaterialBufferSize == 0) return default(TMaterial);

            var nextMaterial = MaterialBuffers[0];
            MaterialBuffers.RemoveAt(0);
            return nextMaterial;
        }
        ///// <summary>
        ///// 移除第一个原料，为后面的腾位置。
        ///// </summary>
        //private void RemoveFirstMaterial()
        //{
        //    if (MaterialBufferSize > 0)
        //    MaterialBuffers.RemoveAt(0);
        //}
         
             
        #endregion
        #endregion
         
    }

     
}
