//2014.10.18，czs, create in beijing, 任何结果皆为产品
//2014.10.23, czs, edit in flight 成都->沈阳 CA4185， 重构，增加批量生产，增加原材料，产品不再标识，任何类皆是产品，皆是原材料。
//2014.11.20, czs, refact in namu, AbstractMultiParamedService 命名为 AbstractMultiService
//2014.12.02, cy, edit in zhengzhou, 修正PrevMaterial和PrevProduct赋值为空 Bug
//2015.10.26, czs, edit in hongqing, 重命名为 AbstractStreamService
//2016.03.16, czs, edit in hongqing, 增加缓存

using System;
using System.IO;
using System.Collections.Generic;
using Gdp.IO; 

namespace Gdp
{
    /// <summary>
    /// 批量服务。规定了生产流程。
    /// </summary>
    /// <typeparam name="TMaterial">原料或者查询条件</typeparam>
    /// <typeparam name="TProduct">产品或计算结果</typeparam>
    public abstract class AbstractStreamService<TProduct, TMaterial> :
        AbstractProcessService<TProduct, TMaterial>,
        IStreamService<TProduct, TMaterial>
    {
        /// <summary>
        /// 日志记录。错误信息记录在日志里面。
        /// </summary>
        protected Log log = new Log(typeof(AbstractStreamService<TProduct, TMaterial>));

        /// <summary>
        /// 构造函数。
        /// </summary>
        public AbstractStreamService()
        {
            this.IsIgnoreError = true; 
            this.CurrentIndex = -1;
            EnumCount = int.MaxValue / 2;
        }

        #region 事件和触发函数 
        /// <summary>
        /// 生产产品。
        /// </summary>
        public virtual event Action<TProduct> ProductProduced;
        /// <summary>
        /// 从数据流中读入了原料
        /// </summary>
        public event MaterialEventHandler<TMaterial> MaterialInputted;
        /// <summary>
        /// 准备处理原料，可以在此预处理原料，如根据缓存情况处理原料。
        /// </summary>
        public event MaterialEventHandler<TMaterial> MaterialProducing;
        /// <summary>
        /// 原料用完啦！
        /// </summary>
        public event Action MaterialEnded;

        #region 事件触发 
        /// <summary>
        /// 生产前触发
        /// </summary>
        /// <param name="material"></param>
        protected virtual void OnMaterialProducing(TMaterial material)
        {
            if (MaterialProducing != null) MaterialProducing(material);
        }
        /// <summary>
        /// 原料用完，触发
        /// </summary>
        protected virtual void OnMaterialEnded()
        {
            IsMaterialEnded = true;
            log.Debug(Name + "输入数据已到末尾");
            if (MaterialEnded != null) MaterialEnded();
        }
        /// <summary>
        /// 从数据流读取一个原料触发，此时尚未加入缓存
        /// </summary>
        protected virtual void OnMaterialInputted(TMaterial m)
        {
            if (MaterialInputted != null) MaterialInputted(m);
        }
        #endregion
        #endregion

        #region 属性
        /// <summary>
        /// 是否结束
        /// </summary>
        public bool IsMaterialEnded { get;   set; }
         
        /// <summary>
        /// 出错后是否继续
        /// </summary>
        public bool IsIgnoreError { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public virtual IEnumer<TMaterial> DataSource { get; protected set; }
        /// <summary>
        /// 是否取消计算过程。
        /// </summary>
        public bool IsCancel { get; set; }   
        /// <summary>
        /// 起始编号，从0开始。
        /// </summary>
        public int StartIndex { get; set; }
        /// <summary>
        /// 遍历数量，默认为最大值的一半。
        /// </summary>
        public int EnumCount { get; set; }
        /// <summary>
        /// 最大的循环编号
        /// </summary>
        public int MaxEnumIndex { get { return StartIndex + EnumCount; } }
        /// <summary>
        /// 设置遍历数量
        /// </summary>
        /// <param name="StartIndex"></param>
        /// <param name="EnumCount"></param>
        public void SetEnumIndex(int StartIndex, int EnumCount) { this.StartIndex = StartIndex; this.EnumCount = EnumCount; }
        #endregion

        #region  方法
        /// <summary>
        /// 批量生产产品，只返回成功的产品。
        /// 如果其中一次计算失败，则继续往下计算，保证结果的数量。如果不足数量，则返回全部。
        /// </summary>
        /// <param name="startIndex">起始编号，从 0 开始</param>
        /// <param name="maxResultCount">最大的结果数量</param>
        /// <returns></returns>
        public virtual List<TProduct> Gets(int startIndex = 0, int maxResultCount = Int32.MaxValue)
        {
            List<TProduct> list = new List<TProduct>();
            this.CurrentIndex = 0;
            DataSource.GetEnumerator().Reset();//重置一下。

            foreach (var m in this)
            {
                if (this.IsCancel) break;

                if (CurrentIndex < startIndex) { CurrentIndex++; continue; } //略过之前的数据
                if (list.Count >= maxResultCount) { break; }//结果满足数量，则退出

                TProduct p = this.Current;

                if (p != null)
                {
                    list.Add(p);
                }
                else
                {
                    log.Error(Name + "第 " + CurrentIndex + " 个计算结果为空！");
                }
            }
            return list;
        }


        /// <summary>
        /// 获取产品，并设置为当前产品。如果失败则返回默认对象，通常为null。每次生产都要执行。
        /// </summary>
        /// <returns></returns>
        public override TProduct Get(TMaterial material)
        {
            this.SetMaterial( material);
            //try
            //{
            CurrentIndex++;

            if (material == null)
            {
                return default(TProduct);
            }
            //通知，即将进行生产
            OnMaterialProducing(material);

            TProduct product = Produce(material);
            //实际上，这就是在检核了
            if (product == null) { return product; }

            //这里隐含了，只保留成功的上一结果 //2016.02.05, czs，设计可否改进？？？以求得原始结果
            if (IsPostCheckEnabled && !CheckProduct(product))
            {
                return default(TProduct);
            }
            else
            {
                SetProduct(product);

                OnProduced(product, material);

                return product;
            }
            //}
            //catch (Exception ex)
            //{
            //    log.Error("计算出错！" + ex.Message);
            //    if (!IsIgnoreError) throw ex ;

            //    return default(TProduct);
            //}

        }

        #region  通用接口
        /// <summary>
        /// 移动到下一条，如要设置了缓存，则先填充缓存。
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveNext()
        {
            #region 流程控制
            CurrentIndex++;
            if (CurrentIndex == StartIndex) { log.Debug("数据流 " + this.Name + " 开始读取数据。"); }
            if (this.IsCancel) { log.Info("数据流 " + this.Name + " 已被手动取消。"); return false; }
            if (CurrentIndex > this.MaxEnumIndex) { log.Info("数据流 " + this.Name + " 已达指定的最大编号 " + this.MaxEnumIndex); return false; }
            while (CurrentIndex < this.StartIndex) { this.MoveNext(); }
            #endregion

            //先获取一个原料
            TMaterial nextBufferMaterial  = GetNextMaterial();
            if (nextBufferMaterial == null) return false;

            Get(nextBufferMaterial);
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
                OnMaterialEnded();
            }
            return default(TMaterial);
        }

        /// <summary>
        /// 重置数据流
        /// </summary>
        public virtual void Reset() { this.DataSource.Reset(); CurrentIndex = 0; IsMaterialEnded = false; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose() { this.DataSource.Dispose(); }
        /// <summary>
        /// 当前解算结果，可能为null。
        /// </summary>
        public TProduct Current { get; set; }
        /// <summary>
        /// 当前对象
        /// </summary>
        object System.Collections.IEnumerator.Current { get { return Current; } }
        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TProduct> GetEnumerator() { return this; }
        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this; }
        #endregion
        #endregion

        /// <summary>
        /// 批量获取。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<TProduct> GetNexts(int count)
        {
            List<TProduct> list = new List<TProduct>();
            int i = 0;
            while (this.MoveNext() && (i++) < count && !IsCancel)
            {
                list.Add(this.Current);
            }
            return list;
        }
    }
    

    /// <summary>
    /// 流式服务。没有指定数据源的流式服务。
    /// </summary> 
    /// <typeparam name="TProduct">产品或计算结果</typeparam>
    public abstract class AbstractStreamService<TProduct> : Named,
        IStreamService<TProduct>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AbstractStreamService() { 
            EnumCount = int.MaxValue / 2;
        this.CurrentIndex = -1;
        }
        /// <summary>
        /// 成功计算完成一次，激发一次。
        /// </summary>
        public virtual event Action<TProduct> ProductProduced;

        /// <summary>
        /// 日志记录。错误信息记录在日志里面。
        /// </summary>
        protected ILog log = Log.GetLog(typeof(AbstractStreamService<TProduct>));

        #region 属性
        /// <summary>
        /// 计算过程中，是否取消。
        /// </summary>
        public bool IsCancel { get; set; }
        /// <summary>
        /// 存储最后一次生产的产品
        /// </summary>
        public TProduct CurrentProduct { get; protected set; }
         
        /// <summary>
        /// 当前编号，从 0 开始。
        /// </summary>
        public virtual int CurrentIndex { get; set; }
        /// <summary>
        /// 起始编号，从0开始。
        /// </summary>
        public virtual int StartIndex { get; set; }
        /// <summary>
        /// 遍历数量，默认为最大值的一半。
        /// </summary>
        public virtual int EnumCount { get; set; }
        /// <summary>
        /// 最大的循环编号
        /// </summary>
        public int MaxEnumIndex { get { return StartIndex + EnumCount; } }
        /// <summary>
        /// 设置遍历数量
        /// </summary>
        /// <param name="StartIndex"></param>
        /// <param name="EnumCount"></param>
        public void SetEnumIndex(int StartIndex, int EnumCount) { this.StartIndex = StartIndex; this.EnumCount = EnumCount; }
        #endregion

        #region  方法
        /// <summary>
        /// 批量生产产品，只返回成功的产品。
        /// </summary>
        /// <param name="startIndex">起始编号，从 0 开始</param>
        /// <param name="maxCount">最大的计算数量</param>
        /// <returns></returns>
        public virtual List<TProduct> Gets(int startIndex = 0, int maxCount = Int32.MaxValue)
        {
            this.Reset();
            List<TProduct> list = new List<TProduct>();
            for (int i = 0; i >= startIndex && i < maxCount && MoveNext(); i++)
            {
                list.Add(this.Current);
            }

            return list;

        }
        /// <summary>
        /// 获取接下来的一个列表。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual List<TProduct> GetNexts(int count)
        {
            List<TProduct> list = new List<TProduct>();
            for (int i = 0; i < count && MoveNext(); i++)
            { 
                list.Add(this.Current);
            }

            return list;
        }
        /// <summary>
        /// 激发一次生产事件
        /// </summary>
        /// <param name="p"></param>
        public void OnProductProduced(TProduct p)
        {
            if (ProductProduced != null) ProductProduced(p);
        }
          

        #region  通用接口
        /// <summary>
        /// 移动到下一条
        /// </summary>
        /// <returns></returns>
        public abstract  bool MoveNext();


        /// <summary>
        /// 重置数据流
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Dispose();
        /// <summary>
        /// 当前解算结果，可能为null。
        /// </summary>
        public virtual TProduct Current { get { return CurrentProduct; } set { this.CurrentProduct = value; } }
        /// <summary>
        /// 当前对象
        /// </summary>
        object System.Collections.IEnumerator.Current { get { return Current; } }
        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TProduct> GetEnumerator() { return this; }
        /// <summary>
        /// 枚举器
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this; }
        #endregion
        #endregion 
    }
}
