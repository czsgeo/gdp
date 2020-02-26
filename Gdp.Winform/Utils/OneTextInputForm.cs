using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gdp.Utils
{
    /// <summary>
    /// 一个文本输入行窗口
    /// </summary>
    public partial class OneTextInputForm : Form
    {
        /// <summary>
        /// 用户输入的值
        /// </summary>
        public string InputValue { get; set; }

        private string initValue;
        /// <summary>
        /// 初始字符串
        /// </summary>
        public string InitValue
        {
            get { return initValue; }
            set
            {
                initValue = value;
                this.textBox_value.Text = InitValue;
            }
        }
        private List<string> canNotBeValues = new List<string>();
        string canNotBeMsg = "该名称已经存在，请换一个 ";
        private string canNotBeWarnMsg;
        /// <summary>
        /// 该名称已经存在，请换一个
        /// </summary>
        public string CanNotBeMsg
        {
            get { return canNotBeMsg; }
            set { canNotBeMsg = value; }
        }

        #region 构造函数们
        /// <summary>
        /// 构造函数
        /// </summary>
        public OneTextInputForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="inputValue"></param>
        public OneTextInputForm(string inputValue)
        {
            InitializeComponent();
            this.InputValue = inputValue;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initValue">初始数据</param>
        /// <param name="canNotBeValues">不能为哪些数据</param>
        public OneTextInputForm(string initValue, List<string> canNotBeValues)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.InitValue = initValue;
            this.canNotBeValues = canNotBeValues;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initValue">初始数据</param>
        /// <param name="canNotBeValues">不能为哪些数据</param>
        /// <param name="canNotBeWarnMsg">不能为哪些数据提示</param>
        public OneTextInputForm(string initValue, List<string> canNotBeValues, string canNotBeWarnMsg)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.InitValue = initValue;
            this.canNotBeValues = canNotBeValues;
            this.canNotBeWarnMsg = canNotBeWarnMsg;
        }
        #endregion

        private void button_ok_Click(object sender, EventArgs e)
        {
            string val = this.textBox_value.Text.Trim();
            if (String.IsNullOrWhiteSpace(val))//基础检测
            { 
                return;
            }
            //else if (String.Equals(currentVal, initValue))
            //{
            //    FormUtil.ShowWarningMessageBox("与初始内容相同，如果不用修改，请取消。" + currentVal);
            //    return;
            //}
            else
            {
                //检查输入
                foreach (string notbe in canNotBeValues)
                {
                    if (notbe == val)
                    {
                        FormUtil.ShowWarningMessageBox(canNotBeMsg + val);
                        return;
                    }
                }

                InputValue = val;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Close();
        }
    }
}
