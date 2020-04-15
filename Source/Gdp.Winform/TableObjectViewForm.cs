//2015.06.17, czs, create in namu, 表显示器
//2017.02.06, czs, edit in hongqing, 增加一些显示设置
//2017.02.25, czs, edit in hongqing, 增加绘图
//2018.07.12, czs, edit in HMX, 数据绑定control的表格

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using Gdp.Utils;

namespace Gdp.Winform
{
    /// <summary>
    /// General table data display window. TableObjectStorage
    /// </summary>
    public partial class TableObjectViewForm : Form
    {        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isShowToolStrip"></param>
        /// <param name="isShowNavigator"></param>
        public TableObjectViewForm(string path, bool isShowToolStrip = true, bool isShowNavigator = true)
        {
            InitializeComponent();

            ObjectTableReader reader = new ObjectTableReader(path, Encoding.Default);
            var table = reader.Read();  
            this.Text = Path.GetFileName(path); 
            Init(table);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="isShowToolStrip"></param>
        /// <param name="isShowNavigator"></param>
        public TableObjectViewForm(ObjectTableStorage dataSource = null, bool isShowToolStrip = true, bool isShowNavigator = true)
        {
            InitializeComponent();
            this.Text = dataSource.Name; 
            Init(dataSource);
        }
        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="table"></param>
        public void Init(ObjectTableStorage table)
        {
            BindDataSource(table);
        }
        /// <summary>
        /// Current table
        /// </summary>
        ObjectTableStorage TableObjectStorage { get => objectTableControl1.TableObjectStorage; }
        /// <summary>
        /// Curret file path
        /// </summary>
        public string FilePath { get; set; }
            /// <summary>
        /// Bingding data
        /// </summary>
        /// <param name="table"></param>
        public void BindDataSource(ObjectTableStorage table)
        {
            this.objectTableControl1.DataBind(table);
        }
    
        /// <summary>
        /// display information
        /// </summary>
        /// <param name="info"></param>
        public void ShowInfo(string info)
        {
            this.Invoke(new Action(() =>
            {
                objectTableControl1.ShowInfo(info);
             //   toolStripLabel_info.Text = info;
            }));
        }

        private void DataTableViewForm_Load(object sender, EventArgs e)
        {
            if (TableObjectStorage != null)
            {
                this.ShowInfo("The number of Column is " + TableObjectStorage.ColCount);
            }
        }
        private void LoadTable()
        {
            ObjectTableReader reader = new ObjectTableReader(FilePath, Encoding.Default);

            var table = reader.Read();//.GetDataTable(); 
            var fileName = Path.GetFileName(FilePath);
            // new DataTableViewForm(table) { Text = fileName }.Show();
            this.Init(table);
        }

        private void 另存为SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs(TableObjectStorage);
        }
        /// <summary>
         /// 另存为
         /// </summary>
         /// <param name="TableObjectStorage"></param>
        public static void SaveAs(ObjectTableStorage TableObjectStorage)
        {
            if (TableObjectStorage == null) { Utils.FormUtil.ShowOkMessageBox("数据表为空！"); return; }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = Setting.TextTableFileFilter;
            dlg.FileName = TableObjectStorage.Name;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ObjectTableWriter writer = new ObjectTableWriter(dlg.FileName, Encoding.Default);
                writer.Write(TableObjectStorage);
                Utils.FormUtil.ShowOkAndOpenDirectory(System.IO.Path.GetDirectoryName(dlg.FileName));
            }
        }
    }
}
