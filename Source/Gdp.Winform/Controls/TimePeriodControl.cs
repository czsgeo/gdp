//2015.10.13, czs, create in hongqing, 创建时段界面

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
    public partial class TimePeriodControl : UserControl
    {
        public TimePeriodControl()
        {
            InitializeComponent();
            ShowDateAndTime();
        }

        public string Title { set { this.label_Name.Text = value; } get { return this.label_Name.Text; } }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime TimeFrom { get { return this.dateTimePickerFrom.Value; } set { this.dateTimePickerFrom.Value = value; } }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime TimeTo { get { return this.dateTimePickerTo.Value; } set { this.dateTimePickerTo.Value = value; } }
        /// <summary>
        /// 获取
        /// </summary>
        public  TimePeriod TimePeriod { get { return new TimePeriod(TimeFrom, TimeTo); } }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="TimePerid"></param>
        public void SetTimePerid(TimePeriod TimePerid)
        {
            TimeFrom = TimePerid.Start.DateTime;
            TimeTo = TimePerid.End.DateTime;
        }
        public void SetTimePerid(BufferedTimePeriod TimePerid)
        {
            TimeFrom = TimePerid.Start.DateTime;
            TimeTo = TimePerid.End.DateTime;
        } 

        public void ShowDateOnly()
        {
            TimeStringFormat = "yyyy-MM-dd";
        }
        public void ShowDateAndTime()
        {
            TimeStringFormat = "yyyy-MM-dd HH:mm:ss";
        }

        public void ShowDateAndTimeToMiniSecond()
        {
            TimeStringFormat = "yyyy-MM-dd HH:mm:ss.fff";
        }
        public void ShowDateAndTimeToMinite()
        {
            TimeStringFormat = "yyyy-MM-dd HH:mm";
        }

        /// <summary>
        /// 时间字符串格式
        /// </summary>
        public string TimeStringFormat
        {
            get { return this.dateTimePickerTo.CustomFormat;}
            set
            {
                this.dateTimePickerTo.CustomFormat = value;
                this.dateTimePickerFrom.CustomFormat = value;
            }
        }

        private void dateTimePickerFrom_ValueChanged(object sender, EventArgs e)
        {
            if(this.TimeFrom > TimeTo)
            {
                this.TimeTo = TimeFrom;
            }
        }
    }
}
