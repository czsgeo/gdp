//2015.10.05, czs, refactor in pengzhou, 提取实现接口  Geo.IProgressCounter
//2016.09.02, czs, edit in hongqing, 实现百分制
//2016.09.05, czs, edit in hongqing, 调试，完善，尚未解决分类完成时信息提示多一条。
//2017.06.14, czs, edit in hongqing, 修改字符显示反映时间

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gdp.Winform.Controls
{
    /// <summary>
    /// 带进度提示的进度条组件.自动处理线程问题。
    /// </summary>
    //[System.Drawing.ToolboxBitmap(typeof(Image), "Image_progressIcon")]
    public partial class ProgressBarComponent : UserControl//, Gdp.IProgressCounter
    {
        /// <summary>
        /// 执行了一步,直接显示进度条进度值，如果是百分制则是百分值
        /// </summary>
        public event IntValueChangedEventHandler ProgressValueChanged;

        /// <summary>
        /// 分类执行了一步
        /// </summary>
        public event IntValueChangedEventHandler ClassifyValueChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProgressBarComponent()
        {
            InitializeComponent();
            ProgressValueChanged += new IntValueChangedEventHandler(ProgressBarComponent_progressValueChanged);
            this.MultiFactorPerStep = 1;
            this.ClassifyIndex = 0;
        }

        #region 属性
        /// <summary>
        /// 是否采用百进制
        /// </summary>
        public bool IsUsePercetageStep { get; set; }
        /// <summary>
        /// 是否逆向
        /// </summary>
        public bool IsBackwardProcess { get; set; }


        /// <summary>
        /// 分类进度列表。
        /// </summary>
        public List<string> Classifies { get; set; }


        int classifyIndex = 0;
        /// <summary>
        /// 当前分类步骤。从 0  开始，设值将触发 ClassifyValueChanged 事件。
        /// </summary>
        public int ClassifyIndex
        {
            get { return classifyIndex; }
            set
            {
                this.classifyIndex = value;
                if (this.ClassifyValueChanged != null) { ClassifyValueChanged(ClassifyIndex); }
            }
        }

        /// <summary>
        /// 当前分类信息
        /// </summary>
        public string CurrentClassify { get { if (ClassifyCount == 0 || ClassifyCount == 1 || Classifies.Count <= ClassifyIndex) return ""; return this.Classifies[ClassifyIndex]; } }

        /// <summary>
        /// 总共分类步骤数量.若为 0 表示当前没有分类。
        /// </summary>
        public int ClassifyCount { get { if (Classifies == null) { return 1; } return Classifies.Count; } }
        /// <summary>
        /// 一步换算到进度条一步间隙的乘法因子。
        /// </summary>
        public double MultiFactorPerStep { get; protected set; }
        /// <summary>
        /// 当前实际进度值，非进度条的值。
        /// </summary>
        public long CurrentPercessValue { get; protected set; }

        #endregion

        #region 方法
        #region  初始化方法
        /// <summary>
        /// 初始化。只有一次分类的初始化。
        /// </summary>
        /// <param name="processCount"></param>
        public void InitProcess(long processCount)
        {
            ShowClassifyInfo("进度");
            ShowClassifyProcessInfo("");
            this.Init(new List<string>(), processCount);
        }

        /// <summary>
        /// 具有分类的初始化
        /// </summary>
        /// <param name="classifies"></param>
        /// <param name="firstProcessCount"></param>
        public void Init(List<string> classifies, long firstProcessCount)
        {
            Init(classifies);
            InitFirstProcessCount(firstProcessCount);
        }
        /// <summary>
        /// 具有分类的初始化,调用此后，还必须调用InitFirstProcessCount
        /// </summary>
        /// <param name="classifies"></param>
        public void Init(List<string> classifies)
        {
            ClassifyIndex = 0;
            this.Classifies = classifies;
        }

        /// <summary>
        /// 初始化第一次进度条的数量
        /// </summary>
        /// <param name="firstProcessCount"></param>
        public void InitFirstProcessCount(long firstProcessCount)
        {
            InitNextClassProcessCount(firstProcessCount); 
        }

        /// <summary>
        /// 初始化设置进度条,并且更新分类进度信息。
        /// </summary>
        /// <param name="nextProcessCount"></param>
        public void InitNextClassProcessCount(long nextProcessCount)
        {
            if (!this.IsHandleCreated) { return; }

            this.Invoke(new Action(delegate ()
            {
                if (!IsBackwardProcess)
                {
                    this.CurrentPercessValue = 0;
                    this.SetProgressCount(nextProcessCount);
                    this.progressBar1.Minimum = 0;
                    this.progressBar1.Value = this.progressBar1.Minimum;
                }
                else
                {
                    this.CurrentPercessValue = nextProcessCount;
                    this.SetProgressCount(nextProcessCount);
                    this.progressBar1.Minimum = 0;
                    this.progressBar1.Value = this.progressBar1.Maximum;
                }
            }));
            ShowInfo("当前进度 ");
            ShowClassifyInfo();
        }
        #endregion

        #region 进度控制 

        /// <summary>
        /// 分类进度向前一步，并初始化下一次进度。
        /// </summary>
        /// <param name="nextProcessCount"></param>
        public void PerformClassifyStep(int nextProcessCount)
        {
            this.ClassifyIndex = this.ClassifyIndex + 1;
            this.InitNextClassProcessCount(nextProcessCount);
        }

        /// <summary>
        /// 执行一次。判断是否百进制。
        /// </summary>
        public void PerformProcessStep()
        {
            if (!IsBackwardProcess)
                CurrentPercessValue++;
            else CurrentPercessValue--;

            UpdateProgressBarValue();
        }
        /// <summary>
        /// 直接设值。
        /// </summary>
        /// <param name="CurrentPercessValue"></param>
        public void SetCurrentPercessValue(long CurrentPercessValue)
        {  
            this.CurrentPercessValue =CurrentPercessValue ;
            UpdateProgressBarValue();
        }

        /// <summary>
        /// 由 CurrentPercessValue 更新 进度条
        /// </summary>
        private void UpdateProgressBarValue()
        {
            if(CurrentPercessValue < 0) { CurrentPercessValue = 0; }

            var val = (int)(CurrentPercessValue * MultiFactorPerStep);

            if (val != this.progressBar1.Value && !this.IsDisposed)
            {
                this.Invoke(new Action(delegate()
                {
                    this.progressBar1.Value = val > this.progressBar1.Maximum ? this.progressBar1.Maximum : val;
                    if (ProgressValueChanged != null) ProgressValueChanged(this.progressBar1.Value);
                }));
            }
        }

        /// <summary>
        ///当前分类的总进度数量
        /// </summary>
        public void SetProgressCount(long count)
        {
            if (count < 1) count = 1;
            //大于1000个时自动采用百进制
            if (count > 1000) { IsUsePercetageStep = true; }
            if (IsUsePercetageStep) { MultiFactorPerStep = 100.0 / count; count = 100; }

            this.progressBar1.Step = 1;
            this.progressBar1.Maximum = (int)count;
            ShowPersentProcessInfo();
        }
        /// <summary>
        /// 填满进度条
        /// </summary>
        public void Full() { this.Invoke(new Action(delegate() { this.progressBar1.Value = progressBar1.Maximum; ShowPersentProcessInfo(); ShowClassifyInfo("完成。"); })); }
        #endregion
        #region 信息显示
        /// <summary>
        /// 显示当前分类信息
        /// </summary>
        protected void ShowClassifyInfo()
        {
            if (ClassifyCount > 0 && ClassifyIndex < ClassifyCount)
            {
                ShowClassifyInfo("," + CurrentClassify);
                ShowClassifyProcessInfo("总进度:" + (ClassifyIndex + 1) + "/" + ClassifyCount);
            }
        }
        /// <summary>
        /// 显示分类进度
        /// </summary>
        /// <param name="message"></param>
        public void ShowClassifyProcessInfo(string message)  {  Gdp.Utils.FormUtil.ShowNotice(label_classfyProcessInfo, message, false); }
        /// <summary>
        /// 显示分类信息
        /// </summary>
        /// <param name="message"></param>
        public void ShowClassifyInfo(string message) {  Gdp.Utils.FormUtil.ShowNotice(label_classfiyInfo, message, false); }

        DateTime LastShowingTime;
        //最小显示时间间隔，避免资源消耗。
        double MinShowingIntervalSec = 0.5;
        /// <summary>
        /// 直接显示信息。
        /// </summary>
        /// <param name="txt"></param>
        public void ShowInfo(string txt)
        {
            if (MinShowingIntervalSec <= (DateTime.Now - LastShowingTime).TotalSeconds)
            {
                LastShowingTime = DateTime.Now;
                Gdp.Utils.FormUtil.ShowNotice(label_progressInfo, txt, false);
            }
        }

        /// <summary>
        /// 显示完成的百分比。
        /// </summary>
        public void ShowPersentProcessInfo()
        {
            if (this.Created)
                this.Invoke(new Action(delegate()
                {
                    if (this.progressBar1.Value == this.progressBar1.Maximum)
                    {
                        ShowInfo(CurrentClassify + " 执行完毕！");
                    }

                    this.label_progressPersent.Text = this.progressBar1.Value + "/" + this.progressBar1.Maximum;
                    this.label_progressPersent.Update();
                }));
        }
        #endregion 
        void ProgressBarComponent_progressValueChanged(int newValue) { ShowPersentProcessInfo(); }
        #endregion

    }
}