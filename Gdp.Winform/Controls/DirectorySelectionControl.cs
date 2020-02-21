//2018.10.01, czs, edti in hmx, 打开文件控件，增加增加属性

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Gdp.Winform.Controls
{ 

    /// <summary>
    /// 打开文件控件
    /// </summary>
    public partial class DirectorySelectionControl : UserControl
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DirectorySelectionControl()
        {
            InitializeComponent();
            IsAddOrReplase = false;
        }
        /// <summary>
        /// 目录改变
        /// </summary>
        public event Action<string> DirectoryChanged;


        private void button_setPath_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (IsAddOrReplase)
                {
                    var path = folderBrowserDialog1.SelectedPath;
                    if(!String.IsNullOrWhiteSpace( this.textBox_filepath.Text))
                    {
                        path = "\r\n" + path;
                    }
                    this.textBox_filepath.AppendText(path);
                }
                else
                {
                    this.textBox_filepath.Text = folderBrowserDialog1.SelectedPath;
                }
            }
        }
        #region 基本属性
        /// <summary>
        /// 是增加或移除
        /// </summary>
        public bool IsAddOrReplase { get; set; }

        /// <summary>
        /// 文件名称标签名称
        /// </summary>
        public string LabelName { get { return this.label_fileName.Text; } set { this.label_fileName.Text = value; } }

        /// <summary>
        /// 文件路径.获取时将进行判断，如果没有则创建目录。
        /// </summary>
        public string Path { get { if (String.IsNullOrWhiteSpace(this.textBox_filepath.Text)) return ""; Gdp.Utils.FileUtil.CheckOrCreateDirectory(Pathes[0]); return Pathes[0]; } set { this.textBox_filepath.Text = value; } }

        /// <summary>
        /// 所有路径
        /// </summary>
        public string [] Pathes { get { return this.textBox_filepath.Lines; } set { this.textBox_filepath.Lines = value; } }
        /// <summary>
        /// 是否多选文件
        /// </summary>
        public bool IsMultiPathes
        {
            get { return  this.textBox_filepath.Multiline;}
            set
            {
                //openFileDialog1.Multiselect = value;
                this.textBox_filepath.Multiline = value;
                if (value) { this.textBox_filepath.ScrollBars = ScrollBars.Both; }
                else { this.textBox_filepath.ScrollBars = ScrollBars.None; }
            }
        }
        
        #endregion

        private void textBox_filepath_TextChanged(object sender, EventArgs e)
        {
            if (DirectoryChanged != null)
            {
                DirectoryChanged(Path);
            }
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path)) { Directory.CreateDirectory(Path); }
            Gdp.Utils.FileUtil.OpenDirectory(Path);
        }

        private void DirectorySelectionControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void DirectorySelectionControl_DragDrop(object sender, DragEventArgs e)
        {   
            System.Array array = ((System.Array)e.Data.GetData(DataFormats.FileDrop));//.GetValue(0).ToString();
            List<string> filePaths = new List<string>();
            foreach (object o in array)
            {
                string path = o.ToString();
                //if (File.Exists(path))
                    filePaths.Add(path);
            }
            this.textBox_filepath.Lines = filePaths.ToArray();

        }

        private void DirectorySelectionControl_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.textBox_filepath.Text))
            {
                this.textBox_filepath.Text = Setting.TempDirectory;
            }
        }
    }
}
