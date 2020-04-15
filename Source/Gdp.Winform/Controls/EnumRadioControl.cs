//2016.07.27, czs, create in fujian yongan, 枚举生成界面
//2016.08.04, czs, edit in fujian yongan, 修改，重命名委托
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gdp.Winform
{
    /// <summary>
    /// 枚举项目选择委托
    /// </summary>
    /// <param name="val"></param>
    //public delegate void EnumItemSelectedEventHandler(string val, bool isSelected);

    /// <summary>
    /// 枚举生成界面
    /// </summary>
    public partial class EnumRadioControl : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EnumRadioControl()
        {
            InitializeComponent();
            IsReady = false;
        }
        /// <summary>
        /// 是否已经可用
        /// </summary>
        public bool IsReady { get;  set; }

        /// <summary>
        /// 已经选择枚举项目。
        /// </summary>
        public event Action<string, bool>  EnumItemSelected;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return this.groupBox1.Text; }
            set { this.groupBox1.Text = value; }
        }
       
        /// <summary>
        /// 初始化
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="IsCheckOne">初始化时，是否要默认选中[第]一个。</param>
        public void Init<TEnum>(bool IsCheckOne = true)
        {
            this.flowLayoutPanel1.Controls.Clear();

            var type = typeof(TEnum);
            var names = Enum.GetNames(type); 
            int i = 0;
            foreach (var name in names)
            {
                var btn = Add(name);
                if (i == 0 && IsCheckOne) { btn.Checked = true; }
                i++;
            }
            IsReady = true;
        }

        public void ClearCheck()
        {
            foreach (var item in  this.flowLayoutPanel1.Controls)
            {
                if (item is RadioButton)
                {
                    ((RadioButton)item).Checked = false;
                }
            }
        }

        /// <summary>
        /// 添加一个
        /// </summary>
        /// <param name="txt"></param>
        public RadioButton Add(string txt)
        {
            RadioButton btn = new RadioButton();
            btn.Text = txt;
            btn.Name = txt;
            btn.AutoSize = true;
            btn.CheckedChanged += btn_CheckedChanged;
            this.flowLayoutPanel1.Controls.Add(btn);
            return btn;
        }

        void btn_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (IsReady && EnumItemSelected !=null)
            {
                EnumItemSelected(btn.Text, btn.Checked);
            }
        }
        /// <summary>
        /// 当前类型
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public TEnum GetCurrent<TEnum>()
        {
            return (TEnum)Enum.Parse(typeof(TEnum), CurrentText);
        }
        /// <summary>
        /// 当前类型
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public void SetCurrent<TEnum>(TEnum val)
        {
             Select<TEnum>(val);
        }
        /// <summary>
        /// 使其选择上
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="val"></param>
        /// <param name="isEnableOther"></param>
        public void Select<TEnum>(TEnum val, bool isEnableOther=true)
        {
            foreach (var item in this.flowLayoutPanel1.Controls)
            {
                if (item is RadioButton)
                {
                    var btn = item as RadioButton;
                    if (btn.Text == val + "") { if(!btn.Checked) btn.Checked = true;  }
                    else if (!isEnableOther)
                    {
                        btn.Enabled = false;
                    }
                }
            }
        }
        /// <summary>
        /// 只启用这些。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enums"></param>
        public void EnableOnly<TEnum>(List<TEnum> enums)
        {
            Remove<TEnum>(Gdp.Utils.EnumUtil.GetOthers<TEnum>(enums));
        }

        /// <summary>
        /// 禁用指定选项
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enums"></param>
        public void Disable<TEnum>(IEnumerable<TEnum> enums)
        {
            foreach (var item in this.flowLayoutPanel1.Controls)
            {
                if (item is RadioButton)
                {
                    var btn = item as RadioButton;
                    var type = Gdp.Utils.EnumUtil.Parse<TEnum>(btn.Text);
                    if (enums.Contains(type))
                    {
                        btn.Checked = false;
                        btn.Enabled = false;
                    }
                }
            }
        }
        /// <summary>
        /// 禁用指定选项
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enums"></param>
        public void Remove<TEnum>(IEnumerable<TEnum> enums)
        {
            List<RadioButton> items = new List<RadioButton>();
            foreach (var item in this.flowLayoutPanel1.Controls)
            {
                var btn = item as RadioButton;
                items.Add(btn);
            }
            foreach (var item in items)
            {
                if (item is RadioButton)
                {
                    var btn = item as RadioButton;
                    var type = Gdp.Utils.EnumUtil.Parse<TEnum>(btn.Text);
                    if (enums.Contains(type))
                    {
                        btn.Checked = false;
                        btn.Enabled = false;

                        this.flowLayoutPanel1.Controls.Remove(btn);
                    }
                }
            }
        }

        /// <summary>
        /// 当前选择
        /// </summary>
        public string CurrentText
        {
            get
            {
                foreach (var item in this.flowLayoutPanel1.Controls)
                {
                    if (item is RadioButton)
                    {
                        var btn = item as RadioButton;
                        if (btn.Checked) return btn.Text;
                    }
                }
                return "";
            }
        }

        private void EnumRadioControl_Load(object sender, EventArgs e)
        {
           
        }
    }
}
