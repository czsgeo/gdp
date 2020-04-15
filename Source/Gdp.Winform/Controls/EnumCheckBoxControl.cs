//2016.07.27, czs, create in fujian yongan, 枚举生成界面
//2016.08.04, czs, edit in fujian yongan, 修改自 Ratiobutton 重命名委托

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
    /// 枚举生成界面
    /// </summary>
    public partial class EnumCheckBoxControl : UserControl
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EnumCheckBoxControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 已经选择枚举项目。
        /// </summary>
        public event Action<string, bool> EnumItemSelected;
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
        public void Init<TEnum>()
        {
            this.flowLayoutPanel1.Controls.Clear();

            var type = typeof(TEnum);
            var names = Enum.GetNames(type);
            int i = 0;
            foreach (var name in names)
            {                
                var btn = Add(name);
                btn.Tag = Enum.Parse(typeof(TEnum), name);
                //if (i == 0) { btn.Checked = true; }
                i++;
            }
        }

        /// <summary>
        /// 界面同步到对象，若对象没有的，则直接添加。
        /// 返回同步后的本身,如果没有，则新建一个。。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="dic"></param>
        public Dictionary<TEnum, bool> UiToEntity<TEnum>(Dictionary<TEnum, bool> dic)
        {
            if (dic == null) { dic = new Dictionary<TEnum, bool>(); }
            foreach (CheckBox item in this.flowLayoutPanel1.Controls)
            {
                var eu = (TEnum)item.Tag;
                dic[eu] = item.Checked;
            }
            return dic;
        }
        /// <summary>
        /// 对象同步到界面,只同步已有的控件，是否选中，不改变无的值，不增减控件。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="dic"></param>
        public void EntityToUi<TEnum>(Dictionary<TEnum, bool> dic)
        { 
            if(dic ==null) {dic = new Dictionary<TEnum,bool>();}
            foreach (CheckBox item in this.flowLayoutPanel1.Controls)
            {
                var eu = (TEnum)item.Tag;
                if(dic.ContainsKey(eu) ){
                     item.Checked = dic[eu];
                }
            }
        }

        /// <summary>
        /// 添加一个
        /// </summary>
        /// <param name="txt"></param>
        public CheckBox Add(string txt)
        {
            CheckBox btn = new CheckBox();
            btn.Text = txt;
            btn.Name = txt;
            btn.AutoSize = true;
            btn.CheckedChanged += btn_CheckedChanged;
            this.flowLayoutPanel1.Controls.Add(btn);
            return btn;
        }

        void btn_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox btn = sender as CheckBox;
            if (EnumItemSelected!=null)
            {
                EnumItemSelected(btn.Text, btn.Checked);
            }
        }
        /// <summary>
        /// 当前类型
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public List<TEnum> GetSelected<TEnum>()
        {
            List<TEnum> list = new List<TEnum>();
            foreach (var item in SelectedNames)
            {
                var e = (TEnum)Enum.Parse(typeof(TEnum), item);
                list.Add(e);
            }
            return list;
        }
        /// <summary>
        /// 当前已选的数量
        /// </summary>
        public int SelectedCount { get { return SelectedNames.Count; } }

        /// <summary>
        /// 使其选择上
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="val"></param>
        public void Select<TEnum>(TEnum val, bool selectOrCancel = true)
        {
            foreach (var item in this.flowLayoutPanel1.Controls)
            {
                if (item is CheckBox)
                {
                    var btn = item as CheckBox;
                    if (btn.Text == val + "") { if (!btn.Checked) btn.Checked = selectOrCancel; break; }
                }
            }
        }
        /// <summary>
        /// 使其选择上
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="val"></param>
        public void Select<TEnum>(IEnumerable<TEnum> vals)
        {
            foreach (var val in vals)
            {
                Select<TEnum>(val);
            }
        }

        /// <summary>
        /// 当前选择
        /// </summary>
        public List<string> SelectedNames
        {
            get
            {
                List<string> list = new List<string>();
                foreach (var item in this.flowLayoutPanel1.Controls)
                {
                    if (item is CheckBox)
                    {
                        var btn = item as CheckBox;
                        if (btn.Checked) { list.Add(btn.Text); }
                    }
                }
                return list;
            }
        }

        private void EnumRadioControl_Load(object sender, EventArgs e)
        {
           
        }
    }
}
