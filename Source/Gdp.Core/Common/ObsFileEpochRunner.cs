//2016.08.20, czs, create in fujianyongan, 宽项计算器，多站观测数据遍历器
//2016.08.29, czs, edit in hongqing, 重构多站观测数据遍历器
//2016.11.19，czs, refact in hongqing, 提取更通用的观测文件数据流

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
 

using Gdp.Data;
using Gdp.Data.Rinex;   
using Gdp.IO; 
using Gdp.Utils;  

namespace Gdp
{
    /// <summary>
    /// 通用文件数据流解析器。只处理观测数据，没有钟差、星历等其它信息。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObsFileEpochRunner<T> : AbstractProcess//, IBasicProcess, Namable
    {
        /// <summary>
        /// 日志
        /// </summary>
        protected ILog log = new Log(typeof(ObsFileEpochRunner<T>));

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObsFileEpochRunner()
        {
            this.RunnerFileExtension = Setting.RinexOFileFilter;

            IsClearTableWhenOutputted = true;
        }

        #region  事件
        /// <summary>
        /// 信息产生
        /// </summary>
        public event InfoProducedEventHandler InfoProduced;
        /// <summary>
        /// 产生一个实体
        /// </summary>
        public event EntityProducedEventHandler<T> EpochEntityProduced;

        /// <summary>
        /// 产生了信息
        /// </summary>
        /// <param name="info"></param>
        protected virtual void OnInfoProduced(string info) { if (InfoProduced != null) { InfoProduced(info); } }
        /// <summary>
        /// 实体产生了。即将进行处理 Process
        /// </summary>
        /// <param name="info"></param>
        protected virtual void OnEpochEntityProduced(T info)
        {
            this.Previous = this.Current;
            this.Current = info;

            if (EpochEntityProduced != null && info != null) { EpochEntityProduced(info); }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 一次计算（正算或反算）的循环次数
        /// </summary>
        public int ExtraStreamLoopCount { get; set; }
        /// <summary>
        /// 当输出完毕后，是否清空结果表。默认清空，有的需要界面显示，则不清空。
        /// </summary>
        public bool IsClearTableWhenOutputted { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表格输出管理器
        /// </summary>
        public ObjectTableManager TableTextManager { get; set; }
        /// <summary>
        /// 所有参数名称
        /// </summary>
        public List<string> ParamNames { get { if (TableTextManager != null && TableTextManager.Count > 0) { return TableTextManager.First.ParamNames; } return new List<string>(); } }
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputDirectory { get; set; }
        /// <summary>
        /// 运行文件的后缀名
        /// </summary>
        public string RunnerFileExtension { get; set; }
        /// <summary>
        /// 文件输入管理器
        /// </summary>
       // public InputFileManager InputFileManager { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public int CurrentIndex { get; set; }
        /// <summary>
        /// 当前
        /// </summary>
        public T Current { get; set; }
        /// <summary>
        /// 上一个
        /// </summary>
        public T Previous { get; set; }
        /// <summary>
        /// 是否取消
        /// </summary>
        public bool IsCancel { get;
            set; }
        /// <summary>
        /// 数据流
        /// </summary>
        public BufferedStreamService<T> BufferedStream { get; set; }

        #endregion

        #region 方法


        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            this.BufferedStream = BuildBufferedStream();
            this.BufferedStream.MaterialBuffersFullOrEnd += OnMaterialBuffersFullOrEnd;
            this.BufferedStream.MaterialCheck += CheckMaterial;
            this.BufferedStream.AfterMaterialCheckPassed += OnAfterMaterialCheckPassed;
            
            log.Info(BufferedStream.Name + " 起始处理编号: " + BufferedStream.StartIndex + ", 最大处理数量: " + BufferedStream.EnumCount);
        }

        /// <summary>
        /// 缓存已满。
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnMaterialBuffersFullOrEnd(IWindowData<T> obj)
        {

        }
        /// <summary>
        /// 检查数据源是否合法，变量是否初始化完成等。
        /// 在运行前检核。
        /// </summary>
        /// <returns></returns>
        public override bool InitCheck()
        {
            return true;
        }

        /// <summary>
        /// 检核新读入数据是否合格，合格才加入缓存。
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        protected virtual bool CheckMaterial(T material)
        {

            return true;
        }

        /// <summary>
        /// 数据刚刚读入
        /// </summary>
        /// <param name="material"></param>
        protected virtual void OnAfterMaterialCheckPassed(T material)
        {
            //if (Option.ProcessType == ProcessType.仅计算) { return; }
            if (this.IsCancel) { return; }

            RawRevise(material);
        }
        /// <summary>
        /// 数据刚刚进入，尚未加入缓存时执行。
        /// </summary>
        /// <param name="material"></param>
        public virtual void RawRevise(T material)
        {
        }

        /// <summary>
        /// 构建数据流
        /// </summary>
        /// <returns></returns>
        protected abstract BufferedStreamService<T> BuildBufferedStream();

        /// <summary>
        /// 运行后调用
        /// </summary>
        public virtual void PostRun()
        {
        }

        /// <summary>
        /// 正式运行
        /// </summary>
        public override void Run()
        {
            //try
            //{
            if (!InitCheck())
            {
                log.Error(this.Name + ", 数据初始检核未通过。计算失败！");
                return;
            }

            CurrentIndex = -1;
            foreach (var mEpochInfo in BufferedStream)
            {
                if (this.IsCancel) { break; }
                Run(mEpochInfo);
                //CurrentIndex++;
            }

            //额外的循环，比如递归最小二乘
            while (ExtraStreamLoopCount > 0)
            {
                CurrentIndex = -1;
                ExtraStreamLoopCount--;
                BufferedStream.Reset();

                foreach (var mEpochInfo in BufferedStream)
                {
                    if (this.IsCancel) { break; }
                    Run(mEpochInfo);
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    var msg = "计算发生错误：" + ex.Message;
            //    log.Error(msg);
            //    //Geo.Utils.FormUtil.ShowErrorMessageBox(msg);
            //}
            //finally
            //{
            //}

            PostRun();
            OnCompleted();
        }
        /// <summary>
        /// 执行一个
        /// </summary>
        /// <param name="mEpochInfo"></param>
        public void Run(T mEpochInfo)
        {
            CurrentIndex++;

            OnEpochEntityProduced(mEpochInfo);

            //3.计算 
            Process(mEpochInfo);

            //4.显示输出 

            //通知显示
            OnInfoProduced("当前进度：" + CurrentIndex);
        }

        /// <summary>
        /// 处理历元
        /// </summary>
        /// <param name="mEpochInfo"></param>
        public abstract void Process(T mEpochInfo);
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            if (TableTextManager != null) { TableTextManager.Dispose(); }
            if (BufferedStream != null) { BufferedStream.Dispose(); BufferedStream = null; }
        }

        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + " ObsFileStreamer";
        }

        /// <summary>
        /// 完成
        /// </summary>
        public override void Complete()
        {
            OnCompleted();
        }
    }

}