//2016.11.27, czs, create in hongqing, 启用浮点数界面

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
    /// 启用浮点数界面
    /// </summary>
    public partial class EnabledFloatControl : UserControl//, INamedFloat
    {
       /// <summary>
       /// 构造函数
       /// </summary>
        public EnabledFloatControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Title { get { return this.label_name.Text; } set { this.label_name.Text = value; } }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public EnableFloat GetEnabledValue()
        {
            var val = double.Parse(this.textBox_value.Text);
            return new EnableFloat(val, this.checkBox_enabled.Checked) ;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="title"></param>
        /// <param name="val"></param>
        public void Init(string title, double val = 0)
        {
            this.textBox_value.Text = val+ "";
            this.label_name.Text = title;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="enabledVal"></param>
        public void SetEnabledValue(EnableFloat enabledVal)
        {
            if(enabledVal == null) { return; }

            this.textBox_value.Text =  enabledVal.Value + "";
            this.checkBox_enabled.Checked = enabledVal.Enabled;
        }

        private void checkBox_enabled_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox_value.Enabled = this.checkBox_enabled.Checked;
        }

        public double GetValue()
        {
            return GetEnabledValue().Value;
        }

        public void SetValue(double val)
        {
             this.textBox_value.Text = val + "";
        }

        public double Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
    }
}
