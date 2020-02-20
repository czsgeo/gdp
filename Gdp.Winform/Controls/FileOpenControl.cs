//2015.10.21, czs, edit, 增加多选文件功能
//2018.03.22,, czs, hmx, 增加可设置文件打开文本

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Gdp.IO;

namespace Gdp.Winform.Controls
{
    /// <summary>
    /// 打开文件控件
    /// </summary>
    public partial class FileOpenControl : UserControl
    {
        Log log = new Log(typeof(FileOpenControl));
        /// <summary>
        /// 构造函数
        /// </summary>
        public FileOpenControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 文件选择后
        /// </summary>
        public event EventHandler FilePathSetted;
        /// <summary>
        /// 外部打开文件事件处理器，若设置，则不采用本地打开。
        /// </summary>
        public event Action<string[]> OpenFileEventHandler;


        private void button_setPath_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (IsMultiSelect)
                {
                    this.textBox_filepath.Lines = openFileDialog1.FileNames;
                }
                else
                {
                   this.textBox_filepath.Text = openFileDialog1.FileName; 
                }

                if (FilePathSetted != null) FilePathSetted(this, e);
            }
        }
        /// <summary>
        /// 设置按钮文本
        /// </summary>
        /// <param name="txt"></param>
        public void SetOpenBtnText(string txt)
        {
            button_open.Text = txt;
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.FilePath))
            {
                MessageBox.Show("请先设置路径后再试！");
                return;
            }
            if (OpenFileEventHandler != null)
            {
                OpenFileEventHandler(FilePathes);
            }
            else
            {
                foreach (var path in FilePathes)
                {
                    if (String.IsNullOrWhiteSpace(path))
                    {
                        continue;
                    }
                    if (path.EndsWith(Setting.TextTableFileExtension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        new TableObjectViewForm(path).Show();
                    }
                    else //外部打开
                    {
                        Gdp.Utils.FileUtil.OpenFileOrDirectory(path);
                    }
                }
            }
        }
        #region 基本属性
        /// <summary>
        /// 文件读取对话框
        /// </summary>
        public OpenFileDialog OpenFileDialog { get { return openFileDialog1; } }
        /// <summary>
        /// 是否多选文件
        /// </summary>
        public bool IsMultiSelect
        {
            get { return openFileDialog1.Multiselect; }
            set
            {
                openFileDialog1.Multiselect = value;
                this.textBox_filepath.Multiline = value;
                if (value) { this.textBox_filepath.ScrollBars = ScrollBars.Both; }
                else { this.textBox_filepath.ScrollBars = ScrollBars.None; }
            }
        }

        /// <summary>
        /// 文件名称标签名称
        /// </summary>
        public string LabelName { get { return this.label_fileName.Text; } set { this.label_fileName.Text = value; } }
        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount { get { if (FilePathes == null) return 0; return FilePathes.Length; } }

        /// <summary>
        /// 文件路径,或则第一个文件路径
        /// </summary>
        public string FilePath { get { if (this.textBox_filepath.Lines.Length > 0) return this.textBox_filepath.Lines[0]; return this.textBox_filepath.Text; } set { this.textBox_filepath.Text = value; if (FilePathSetted != null) FilePathSetted(this, null); } }
        /// <summary>
        /// 第一个文件路径
        /// </summary>
        public string FirstPath { get { if (FileCount == 0) return ""; return this.textBox_filepath.Lines[0]; } set { if (FileCount != 0) this.textBox_filepath.Lines[0] = value; if (FilePathSetted != null) FilePathSetted(this, null); } }
        /// <summary>
        /// 第一个目录
        /// </summary>
        public string FirstDirectory => Path.GetDirectoryName(FirstPath);
         
        /// <summary>
        /// 文件路径，批量的。
        /// </summary>
        public string [] FilePathes { get { return this.textBox_filepath.Lines; } set { this.textBox_filepath.Lines = value; } }

        /// <summary>
        /// 文件过滤器
        /// </summary>
        public string Filter { get { return openFileDialog1.Filter; } set { openFileDialog1.Filter = value; } }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 设置路径，推荐方法
        /// </summary>
        /// <param name="pathes"></param>
        public void SetFilePahtes(string[] pathes)
        { this.textBox_filepath.Lines = pathes;
        }
        /// <summary>
        /// 返回第一个匹配上的。返回路径，如果是文件路径，直接返回，如果是目录，则提取目录中匹配的路径。
        /// 如果没有，返回null。
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="SearchOption"></param>
        /// <returns></returns>
            public string GetFilePath(string extension = "*.*", SearchOption SearchOption = SearchOption.TopDirectoryOnly)
        {
            var pathes = GetFilePathes(extension, SearchOption);
            if (pathes !=null &&  pathes.Length > 0) { return pathes[0]; }
            return null;
        }
        /// <summary>
        /// 返回路径，如果是文件路径，直接返回，如果是目录，则提取目录中匹配的路径。
        /// 如果没有，返回null。
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="SearchOption"></param>
        /// <returns></returns>
        public string[] GetFilePathes(string extension, SearchOption SearchOption = SearchOption.TopDirectoryOnly)
        {
            if (FileCount == 0) { return null; }
            if (FileCount == 1) {
                if(Gdp.Utils.FileUtil.IsDirectory(FilePath)){
                    return Directory.GetFiles(FilePath, extension, SearchOption);
                }
                return new string[] { this.FilePath };            
            } 
             return FilePathes; 
        }

        /// <summary>
        /// 读取所有行
        /// </summary>
        /// <returns></returns>
        public string[] ReadAllLines()
        {
            return File.ReadAllLines(FilePath);
        }
        /// <summary>
        /// 读取所有文本
        /// </summary>
        /// <returns></returns>
        public string ReadAllText()
        {
            return File.ReadAllText(FilePath);
        }
        /// <summary>
        /// 返回数据流
        /// </summary>
        /// <returns></returns>
        public FileStream OpenRead()
        {
            return File.OpenRead(FilePath);
        }
        /// <summary>
        /// 返回数据流
        /// </summary>
        /// <returns></returns>
        public FileStream OpenWrite()
        {
            return File.OpenWrite(FilePath);
        } 
        #endregion
        private void textBox_filepath_TextChanged(object sender, EventArgs e)
        {
            if (this.textBox_filepath.Lines.Length == 1)
            {
                log.Info(LabelName.Replace("：","").Replace(":","") + "，载入了： " + this.textBox_filepath.Text );
            }
            else
            {
                log.Info(LabelName.Replace("：", "").Replace(":", "") + "，载入 " + this.textBox_filepath.Lines.Length + " 条项目：\r\n" + this.textBox_filepath.Text + "\r\n" + this.textBox_filepath.Lines.Length + "条");
            }
        }

        #region 拖拽
        private void FileOpenControl_DragDrop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //    e.Effect = DragDropEffects.Link;
            //else e.Effect = DragDropEffects.None;

            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            textBox.Text = dragDrop(e);
        }

        private void FileOpenControl_DragEnter(object sender, DragEventArgs e)
        {
            //System.Array array = ((System.Array)e.Data.GetData(DataFormats.FileDrop));//.GetValue(0).ToString();
            //List<string> filePaths = new List<string>();
            //foreach (object o in array)
            //{
            //    string path = o.ToString();
            //    if (File.Exists(path))
            //    filePaths.Add(path);
            //}
            //this.textBox_filepath.Lines = filePaths.ToArray();
            dragEnter(e);
        }



        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            dragEnter(e);
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;
            textBox.Text = dragDrop(e);
        }

        /// <summary>
        /// 文件拖进事件处理：
        /// </summary>
        public void dragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))    //判断拖来的是否是文件
                e.Effect = DragDropEffects.Link;                //是则将拖动源中的数据连接到控件
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 文件放下事件处理：
        /// 放下, 另外需设置对应控件的 AllowDrop = true; 
        /// 获取的文件名形如 "d:\1.txt;d:\2.txt"
        /// </summary>
        public string dragDrop(DragEventArgs e)
        {
            StringBuilder filesName = new StringBuilder("");
            Array file = (System.Array)e.Data.GetData(DataFormats.FileDrop);//将拖来的数据转化为数组存储

            foreach (object I in file)
            {
                string str = I.ToString();

                System.IO.FileInfo info = new System.IO.FileInfo(str);
                //若为目录，则获取目录下所有子文件名
                if ((info.Attributes & System.IO.FileAttributes.Directory) != 0)
                {
                    str = getAllFiles(str);
                    if (!str.Equals("")) filesName.Append((filesName.Length == 0 ? "" : ";") + str);
                }
                //若为文件，则获取文件名
                else if (System.IO.File.Exists(str))
                    filesName.Append((filesName.Length == 0 ? "" : ";") + str);
            }

            return filesName.ToString();
        }

        /// <summary>
        /// 判断path是否为目录
        /// </summary>
        public bool IsDirectory(String path)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            return (info.Attributes & System.IO.FileAttributes.Directory) != 0;
        }

        /// <summary>
        /// 获取目录path下所有子文件名
        /// </summary>
        public string getAllFiles(String path)
        {
            StringBuilder str = new StringBuilder("");
            if (System.IO.Directory.Exists(path))
            {
                //所有子文件名
                string[] files = System.IO.Directory.GetFiles(path);
                foreach (string file in files)
                    str.Append((str.Length == 0 ? "" : ";") + file);

                //所有子目录名
                string[] Dirs = System.IO.Directory.GetDirectories(path);
                foreach (string dir in Dirs)
                {
                    string tmp = getAllFiles(dir);  //子目录下所有子文件名
                    if (!tmp.Equals("")) str.Append((str.Length == 0 ? "" : ";") + tmp);
                }
            }
            return str.ToString();
        }
        #endregion
    }
}
