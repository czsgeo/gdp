//2016.11.27, czs, create in hongqing, 启用浮点数界面
//2017.03.16, czs, edit in hongqing, 界面浮点数

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
    public partial class NamedFloatControl : UserControl//, Gdp.Winform.Controls.INamedFloat
    {
       /// <summary>
       /// 构造函数
       /// </summary>
        public NamedFloatControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 值
        /// </summary>
        public double Value { get { return GetValue(); } set { this.SetValue(value); } }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Title { get { return this.label_name.Text; } set { this.label_name.Text = value; } }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public double GetValue()
        {
            this.textBox_value.Text = this.textBox_value.Text.Replace("。", ".");
            return  double.Parse(this.textBox_value.Text); 
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
        /// <param name="val"></param>
        public void SetValue(double val)
        {
            this.textBox_value.Text =  val + ""; 
        } 
    }
}
