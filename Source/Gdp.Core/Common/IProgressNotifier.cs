//2017.08.31, create in hongqing, 进度通知接口
//2018.06.23, czs, edit in HMX, 增加反向（后退）进度

using System;
using System.Collections.Generic;
using System.Collections;

namespace Gdp
{

    /// <summary>
    /// 进度通知接口
    /// </summary>
    public interface IProgressViewer
    {
        /// <summary>
        /// 最小的进度改变。
        /// </summary>
        event IntValueChangedEventHandler ProgressValueChanged;
        /// <summary>
        /// 是否是后退，默认否
        /// </summary>
        bool IsBackwardProcess { get; set; }
        /// <summary>
        /// 当前实际进度值，非进度条的值。
        /// </summary>
        long CurrentPercessValue { get; }
        /// <summary>
        /// 初始化。只有一次分类的初始化。最简单的单进度。
        /// </summary>
        /// <param name="maxCount"></param>
        void InitProcess(long maxCount);
        /// <summary>
        /// 直接显示信息，一般显示当前正在进行的最小任务。
        /// </summary>
        /// <param name="txt"></param>
        void ShowInfo(string txt);

        /// <summary>
        /// 向前一步
        /// </summary>
        void PerformProcessStep();
        /// <summary>
        /// 填满
        /// </summary>
        void Full();
    }

    //2017.10.26, czs, create in hongqing, 进度条通知。
    /// <summary>
    /// 进度条通知接口。用于适配进度条通知。
    /// </summary>
    public interface IProgressNotifier
    {
        /// <summary>
        /// 进度通知接口
        /// </summary>
        IProgressViewer ProgressViewer { get; set; }

        #region 进度通知接口

        /// <summary>
        /// 初始化进度调
        /// </summary>
        /// <param name="steps"></param>
        void InitProcess(int steps);

        /// <summary>
        /// 进度调向前一步
        /// </summary>
        void PerformProcessStep();
        /// <summary>
        /// 将进度条填满
        /// </summary>
        void FullProcess();
        #endregion
    }

    /// <summary>
    /// 进度条通知接口。
    /// </summary>
    public class ProgressNotifier : IProgressNotifier
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
}