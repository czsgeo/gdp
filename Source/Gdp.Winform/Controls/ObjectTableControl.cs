//2017.10.03, czs, create in hongqing, 表对象查看和操作UI
//2018.07.15, czs, edit in HMX, 增加多项式拟合等

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms; 
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;
using Gdp.Utils;
using System.IO;
using Gdp.IO;

namespace Gdp.Winform
{
    /// <summary>
    /// 表对象查看和操作UI
    /// </summary>
    public partial class ObjectTableControl : UserControl
    {
        Log log = new Log(typeof(ObjectTableControl));
        /// <summary>
        /// 构造函数
        /// </summary>
        public ObjectTableControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当前表
        /// </summary>
        public ObjectTableStorage TableObjectStorage { get; set; }
        /// <summary>
        /// 数据表
        /// </summary>
        public System.Windows.Forms.DataGridView DataGridView { get { return dataGridView1; } }

        /// <summary>
        /// 新建表
        /// </summary>
        /// <returns></returns>
        public ObjectTableStorage GetSelectedRowsTable() {

            var headerTexts = new List<string>();
            foreach (DataGridViewColumn item in DataGridView.Columns)
            {
                headerTexts.Add(item.HeaderText);
            }

            var table = new ObjectTableStorage("选择后");
            foreach (DataGridViewRow row in DataGridView.SelectedRows)
            { 
                table.NewRow();

                int i = 0;
                foreach (DataGridViewCell item in row.Cells)
                {

                    table.AddItem(headerTexts[i], item.Value);
                    i++;
                }
            }

            return table;
        }

        #region 调用方法
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="info"></param>
        public void ShowInfo(string info)
        {
            this.Invoke(new Action(() =>
            {
                toolStripLabel_info.Text = info;
            }));
        }
        /// <summary>
        /// 数据绑定
        /// </summary>
        /// <param name="table"></param>
        public void DataBind(ObjectTableStorage table)
        {
            var title = table.Name + "， 共 " + table.ColCount + "列 × " + table.RowCount + "行";
            log.Debug("绑定显示 " + title);
   
            this.TableObjectStorage = table;
            try
            {
                //this.Invoke(new Action(() =>//采用Invoke将出错
                //{
                this.dataGridView1.DataSource = table.GetDataTable();
                toolStripLabel_info.Text = title;
                bindingSource1.DataSource = this.dataGridView1.DataSource;
                bindingNavigator1.BindingSource = bindingSource1;
                //}));
            }catch(Exception ex)
            {
                this.dataGridView1.DataSource = null; 
                bindingSource1.DataSource = null;
                bindingNavigator1.BindingSource = bindingSource1;


                var path = Path.Combine(Application.StartupPath, table.Name + ".txt.xls");
                Gdp.Utils.FormUtil.ShowIfOpenDirMessageBox(path, "数据显示出错：" + ex.Message
                    + "\r\n" + "数据已保存在：" + path + ", 是否打开？");
                ObjectTableWriter.Write(table, path); 
            }
        }
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value is Time)
            {
                e.Value = e.Value.ToString();
            }
            if (e.Value is DateTime)
            {
                e.Value = ((DateTime)e.Value).ToString("yyyy-MM-dd HH:mm:ss");
            }

        }
        #endregion
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            this.dataGridView1.DataSource = null;
        } 
    }
}
