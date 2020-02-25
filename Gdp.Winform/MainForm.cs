//2019.08.21, czs, create in hongqing, creating GDP

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using Gdp.Data.Rinex;

namespace Gdp.Winform
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Button_convert_Click(object sender, EventArgs e)
        {
            new ObsFileConvertForm().Show();
        }

        private void Button_select_Click(object sender, EventArgs e)
        {
            new ObsFileSelectorForm().Show();
        }

        private void Button_download_Click(object sender, EventArgs e)
        {
            new IgsProductDownloadForm().Show();
        }

        private void Button_view_Click(object sender, EventArgs e)
        {
            new ObsFileViewerForm().Show();
        }

        private void LinkLabel_gnsserSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("www.gnsser.com");
        }

        private void LinkLabel_163mail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:gnsser@163.com");
        }

        private void button_extractSiteInfo_Click(object sender, EventArgs e)
        {
            new FileInfoExtractForm().Show();
        }
        int formIndex = 0;
        private void button_tableView_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Table Text(*.txt.xls)|*.txt.xls|All|*.*"; ;
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = openFileDialog.FileNames;
                foreach (var filePath in files)
                { 
                    var reader = new ObjectTableReader(filePath, Encoding.Default);
                    var table = reader.Read();//.GetDataTable();  
                    var fileName = System.IO.Path.GetFileName(filePath);

                    var form = new Winform.TableObjectViewForm(table) { Text = fileName + "_" + (formIndex++) };
                    form.FilePath = filePath;
                    form.Show(); 
                }
            }
        }

        private void button_openLog_Click(object sender, EventArgs e)
        {
            Utils.FileUtil.OpenFile("./sys.log");
        }

        private void button_testobs_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Rinex Text|*.rnx;*.??o|All|*.*"; ;
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = openFileDialog.FileNames;
                foreach (var filePath in files)
                {
                    var opt = new ObsFileConvertOption();
                    opt.IsEnableRinexVertion = true;
                    opt.Version = 3.02;
                    ObsFileFormater ObsFileFormater = new ObsFileFormater(opt, filePath); 
                    ObsFileFormater.Init();
                    ObsFileFormater.Run();
                }
                Utils.FormUtil.ShowOkMessageBox("执行完毕！");


            }

        }
    }
}
