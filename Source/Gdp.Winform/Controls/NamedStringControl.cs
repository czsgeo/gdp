//2017.07.25, czs, create in hongqing, 字符串界面 

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
    /// 字符串界面
    /// </summary>
    public partial class NamedStringControl : UserControl
    {
       /// <summary>
       /// 构造函数
       /// </summary>
        public NamedStringControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public String GetValue()
        {
            return (this.textBox_value.Text).Trim();
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public String [] GetLines()
        {
            return (this.textBox_value.Lines);
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Title { get { return this.label_name.Text; } set { this.label_name.Text = value; } }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="title"></param>
        /// <param name="val"></param>
        /// <param name="isMultiLines"></param>
        public void Init(string title, string val, bool isMultiLines = false)
        {
            this.textBox_value.Multiline = isMultiLines;
            this.textBox_value.Text = val + "";
            this.label_name.Text = title;
            if (this.textBox_value.Multiline && this.Height < 30)
            {
                this.Height *= 3;
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="enabledVal"></param>
        public void SetValue(String enabledVal)
        {
            this.textBox_value.Text = enabledVal + ""; 
        }


    }
}
