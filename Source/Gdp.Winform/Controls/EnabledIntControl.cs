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
    /// 启用整数界面
    /// </summary>
    public partial class EnabledIntControl : UserControl//,INamedInt
    {
       /// <summary>
       /// 构造函数
       /// </summary>
        public EnabledIntControl()
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
        public EnableInteger GetEnabledValue()
        {
            var val = (int)numericUpDown1.Value;// (this.textBox_value.Text);
            return new EnableInteger(val) { Enabled = this.checkBox_enabled.Checked };
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="title"></param>
        /// <param name="val"></param>
        public void Init(string title, int val = 0)
        {
            this.numericUpDown1.Value = (Decimal)val;
            this.label_name.Text = title;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="enabledVal"></param>
        public void SetEnabledValue(EnableInteger enabledVal)
        {
            this.numericUpDown1.Value = enabledVal.Value;
            this.checkBox_enabled.Checked = enabledVal.Enabled;
        }

        private void checkBox_enabled_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown1.Enabled = this.checkBox_enabled.Checked;
        }


        public int GetValue()
        {
            return GetEnabledValue().Value;
        }

        public void SetValue(int val)
        {
            this.numericUpDown1.Value = val;
        }

        public int Value
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
