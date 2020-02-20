//2017.10.26, czs, create in hongqing, 通用处理器接口
//2018.03.24, czs, edit in hmx, 具有返回参数的处理器

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 具有返回参数的处理器。一个抽象的参考处理类。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public abstract class AbstractProcess<TMaterial, TResult> : AbstractBasicProcess<TMaterial,TResult>
    {
        #region 进度通知接口
        /// <summary>
        /// 进度通知接口
        /// </summary>
        public IProgressViewer ProgressViewer { get; set; }
        /// <summary>
        /// 初始化进度调
        /// </summary>
        /// <param name="steps"></param>
        public void InitProcess(int steps) { if (ProgressViewer != null) { ProgressViewer.InitProcess(steps); } }

        /// <summary>
        /// 进度调向前一步
        /// </summary>
        public void PerformProcessStep() { if (ProgressViewer != null) { ProgressViewer.PerformProcessStep(); } }
        /// <summary>
        /// 将进度条填满
        /// </summary>
        public void FullProcess() { if (ProgressViewer != null) { ProgressViewer.Full(); } }
        #endregion
    }
    /// <summary>
    /// 一个抽象的参考处理类。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public abstract class AbstractProcess<TMaterial> : AbstractBasicProcess<TMaterial>, IProcess<TMaterial>
    {
        #region 进度通知接口
        /// <summary>
        /// 进度通知接口
        /// </summary>
        public IProgressViewer ProgressViewer { get; set; }
        /// <summary>
        /// 初始化进度调
        /// </summary>
        /// <param name="steps"></param>
        public void InitProcess(int steps) { if (ProgressViewer != null) { ProgressViewer.InitProcess(steps); } }

        /// <summary>
        /// 进度调向前一步
        /// </summary>
        public void PerformProcessStep() { if (ProgressViewer != null) { ProgressViewer.PerformProcessStep(); } }
        /// <summary>
        /// 将进度条填满
        /// </summary>
        public void FullProcess() { if (ProgressViewer != null) { ProgressViewer.Full(); } }
        #endregion
    }
    /// <summary>
    /// 一个抽象的参考处理类。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public abstract class AbstractProcess : AbstractBasicProcess, IProcess
    {
        #region 进度通知接口
        /// <summary>
        /// 进度通知接口
        /// </summary>
        public IProgressViewer ProgressViewer { get; set; }
        /// <summary>
        /// 初始化进度调
        /// </summary>
        /// <param name="steps"></param>
        public void InitProcess(int steps) { if (ProgressViewer != null) { ProgressViewer.InitProcess(steps); } }

        /// <summary>
        /// 进度调向前一步
        /// </summary>
        public void PerformProcessStep() { if (ProgressViewer != null) { ProgressViewer.PerformProcessStep();  } }
        
        /// <summary>
        /// 将进度条填满
        /// </summary>
        public void FullProcess() { if (ProgressViewer != null) { ProgressViewer.Full(); } }
        #endregion
    }

    /// <summary>
    /// 常用通用处理器接口。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public interface IProcess : IBasicProcess, IProgressNotifier
    {

    }
    /// <summary>
    /// 常用通用处理器接口。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public interface IProcess<TMaterial> : IBasicProcess<TMaterial>, IProgressNotifier
    {

    }
    /// <summary>
    /// 一个抽象的参考处理类。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public abstract class AbstractTopBasicProcess : ITopProcess
    {
        /// <summary>
        /// 完成事件。
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// 初始化，通常需要手动调用
        /// </summary>
        public virtual void Init() { }
         
        /// <summary>
        /// 完成
        /// </summary>
        public virtual void Complete() { OnCompleted(); }
        /// <summary>
        /// 完成
        /// </summary>
        protected virtual void OnCompleted() { if (Completed != null) { Completed(this, new EventArgs()); } }
        /// <summary>
        /// 检核
        /// </summary>
        /// <returns></returns>
        public virtual bool InitCheck() { return true; }
    }


    /// <summary>
    /// 一个抽象的参考处理类。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public abstract class AbstractBasicProcess : AbstractTopBasicProcess, IBasicProcess
    {         
        /// <summary>
        /// 执行。
        /// </summary>
        public abstract void Run(); 
    }

    /// <summary>
    /// 一个抽象的参考处理类。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public abstract class AbstractBasicProcess<TMaterial> : AbstractTopBasicProcess, IBasicProcess<TMaterial>
    { 
        /// <summary>
        /// 执行。
        /// </summary>
        public abstract void Run(TMaterial input); 
    }

    /// <summary>
    /// 具有返回结果对象，一个抽象的参考处理类。任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public abstract class AbstractBasicProcess<TMaterial, TResult> : AbstractTopBasicProcess, IBasicProcess<TMaterial, TResult>
    { 
        /// <summary>
        /// 执行。
        /// </summary>
        public abstract TResult Run(TMaterial input); 
    }
    /// <summary>
    /// 通用处理器接口，任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public interface IBasicProcess : ITopProcess
    { 
        /// <summary>
        /// 执行。
        /// </summary>
        void Run();
    }
    /// <summary>
    /// 通用处理器接口，任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public interface IBasicProcess<TMaterial> : ITopProcess
    {
        /// <summary>
        /// 执行。
        /// </summary>
        void Run(TMaterial input);
    }
    /// <summary>
    /// 具有返回结果。通用处理器接口，任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public interface IBasicProcess<TMaterial,TResult> : ITopProcess
    {
        /// <summary>
        /// 执行。
        /// </summary>
        TResult Run(TMaterial input);
    }

    /// <summary>
    /// 通用处理器接口，任何计算、过程、方法、数据流等等都可以表示为一个处理过程。
    /// 本接口封装一些基础的方法。
    /// </summary>
    public interface ITopProcess
    {
        /// <summary>
        /// 完成事件。
        /// </summary>
        event EventHandler Completed;


        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
        /// <summary>
        /// 检查数据源是否合法，变量是否初始化完成等。
        /// 在运行前检核。
        /// </summary>
        /// <returns></returns>
        bool InitCheck();


        /// <summary>
        /// 完成
        /// </summary>
        void Complete();
    }
}
