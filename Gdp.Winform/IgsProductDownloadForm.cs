//2014.12.04, czs, create in jinxingliaomao shuangliao jilin, 星历生成下载器
//2017.06.13, czs, edit in hongqing, 增加二位数年和年内周的表达。

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Gdp.Data;

namespace Gdp.Winform
{
    public partial class IgsProductDownloadForm : Form
    {
        public IgsProductDownloadForm()
        {
            InitializeComponent();

            var datasource = Enum.GetNames(typeof(IgsProductType));

            bindingSource_productTypes.DataSource = datasource;
        }

        static bool IsCancel = false;
        public string SaveDir { get { return this.directorySelectionControl1.Path; } }

        string[] FileUrls
        {
            get
            {
                string[] urls = null;
                this.Invoke(new Action(delegate()
               {
                   urls = this.richTextBoxControl_allUrls.Lines;
               }));
                return urls;
            }
        }
        string[] UrlDirectories { get { return textBox_pathDirs.Lines; } set { textBox_pathDirs.Lines = value; } }
     
        string[] SourceNames
        {
            get
            {
                return textBox_sourcenames.Text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        string[] UrlModels { get { return textBox_model.Lines; } set { textBox_model.Lines = value; } }

        #region 人工交互
        private void button_buildPathes_Click(object sender, EventArgs e)
        {
            var type = (IgsProductType)Enum.Parse(typeof(IgsProductType), this.bindingSource_productTypes.Current.ToString());
            int step = int.Parse(this.textBox_stepHour.Text);
            string[] siteNames = namedStringControl_siteNames.GetValue().Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);


            var EphemerisPathBuilder = new IgsProductUrlPathBuilder(
                UrlDirectories,UrlModels, SourceNames,
                this.timePeriodUserControl1.TimePeriod.Start.DateTime, this.timePeriodUserControl1.TimePeriod.End.DateTime, type, step * 3600);
            EphemerisPathBuilder.SiteNames = new List<string>(siteNames);


            this.richTextBoxControl_allUrls.Lines =  EphemerisPathBuilder.Build();
            ShowInfo("The addresses are generated successfully!");
            MessageBox.Show("The address is generated successfully! And the number is " + FileUrls.Length);
        }
        private void button_cancel_Click(object sender, EventArgs e)
        {
            ShowInfo("Downloading is to be stopped!");
            IsCancel = true;
            backgroundWorker1.CancelAsync();
        }

        private void button_download_Click(object sender, EventArgs e)
        {
            if (FileUrls.Length == 0)
            {
                MessageBox.Show("Please generate the address, or paste the address into the 'addresses' textbox below！");
                return;
            }

            IsCancel = false;
            this.progressBarComponent1.InitProcess( FileUrls.Length); 

            this.button_download.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }
        #endregion
        

        private void ShowInfo(string url)
        {
            this.Invoke(new Action(delegate()
            {
                this.textBox_result.Text = DateTime.Now + ":" + url + "\r\n" + this.textBox_result.Text;
                this.textBox_result.Update();
            }));
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);

                List<string> failed = new List<string>();
                int okCount = 0;
                foreach (string url in FileUrls)
                {
                    string info = "Downloading successfully!";
                    if (IsCancel || backgroundWorker1.CancellationPending)
                    {
                        ShowInfo("Downloading ending!");
                        break;
                    }
                    if (!Gdp.Utils.NetUtil.FtpDownload(url, Path.Combine(SaveDir, Path.GetFileName(url))))
                    {
                        failed.Add(url);
                        info = "Downloading Filed!";
                    }
                    else
                    {
                        okCount++;
                    }
                    ShowInfo(info + url);

                    this.Invoke(new Action(PerformStep));
                }
                //outputing the failed pathes
                StringBuilder sb = new StringBuilder();
                foreach (string fail in failed)
                {
                    sb.AppendLine(fail);
                }
                if (sb.Length > 0)
                {
                    this.Invoke(new Action(delegate()
                    {
                        this.richTextBoxControl_failedUrls.Text = sb.ToString();
                    }));
                    ShowInfo("The failed pathes are \r\n" + sb.ToString());
                }

                String msg = "Finsh download, wherein " + FileUrls.Length + " files download successfully." + okCount + " \r\n And " + failed.Count + " files failed.\r\n";

                msg += "\r\n是否打开目录？";
                Gdp.Utils.FormUtil.ShowIfOpenDirMessageBox(SaveDir, msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" %>_<% Error!" + ex.Message);
            }
        }

        private void PerformStep()
        {
            this.progressBarComponent1.PerformProcessStep();
            this.Update();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.button_download.Enabled = true;
            ShowInfo("Downloading has been stopped!");
            //    MessageBox.Show("已经停止下载!");
        }


        private void DownFilesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                if (Gdp.Utils.FormUtil.ShowYesNoMessageBox("Confirm to close, downloading will be canceled.")
                == System.Windows.Forms.DialogResult.Yes)
                {
                    backgroundWorker1.CancelAsync();
                }
                else
                    e.Cancel = true;
        }
         
        private void EphemerisDownloadForm_Load(object sender, EventArgs e) {
            this.UrlDirectories = Setting.IgsProductUrlDirectories;
            this.UrlModels = Setting.IgsProductUrlModels; 
            directorySelectionControl1.Path = Setting.TempDirectory;

            directorySelectionControl_localLibFolder.Pathes = Setting.IgsProductLocalDirectories;
        }

        private void richTextBoxControl_allUrls_TextChanged(object sender, EventArgs e) { ShowInfo("The total number of adresses is " + FileUrls.Length); }
          
        private void radioButton1IGS周解模板_CheckedChanged(object sender, EventArgs e)
        {
            textBox_model.Text = "{UrlDirectory}/{Week}/{SourceName}{SubYear}P{WeekOfYear}.{ProductType}.Z";
            this.textBox_stepHour.Text = (7 * 24) + "";
            textBox_sourcenames.Text = "igs,wum,gbm,qzf,tum,com";

        }

        private void radioButton2IGS日解模板_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox_stepHour.Text = 24 + "";
            textBox_model.Text = "{UrlDirectory}/{Week}/{SourceName}{Week}{DayOfWeek}.{ProductType}.Z";
            textBox_sourcenames.Text = "igs,wum,gbm,qzf,tum,com";

        }

        private void radioButton3IGMAS小时产品_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox_stepHour.Text = 6 + "";
            textBox_model.Text = "http://124.205.50.178/Product/TreePage/downItem/?fid=/products/products/{BdsWeek}/isu{BdsWeek}{DayOfWeek}_{Hour}.{ProductType}.Z"; // "{UrlDirectory}/{BdsWeek}/{SourceName}{Week}{DayOfWeek}.{ProductType}.Z";
            textBox_sourcenames.Text = "isc,isr,isu";

        }

        private void radioButton4IGMAS日解产品_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox_stepHour.Text = 24 + "";
            textBox_model.Text = "http://124.205.50.178/Product/TreePage/downItem/?fid=/products/products/{BdsWeek}/isu{BdsWeek}{DayOfWeek}.{ProductType}.Z"; // "{UrlDirectory}/{BdsWeek}/{SourceName}{Week}{DayOfWeek}.{ProductType}.Z";
            textBox_sourcenames.Text = "isc,isr,isu";

        }

        private void button_checkLib_Click(object sender, EventArgs e)
        {
            var allLines = this.richTextBoxControl_allUrls.Lines;
            Dictionary<string, string> allNames = new Dictionary<string, string>();
            foreach (var line in allLines)
            {
                var fileName = Path.GetFileName(line);
                allNames[fileName] = line;
            }

            var igsLocalDires = directorySelectionControl_localLibFolder.Pathes;// Setting.IgsProductLocalDirectories;

            List<string> notIncludes = new List<string>();
            foreach (var item in allNames)
            {
                bool isContains = false;
                foreach (var igsLocalDir in igsLocalDires)
                {
                    var localPath = Path.Combine(igsLocalDir, item.Key);
                    var localPath2 = Path.Combine(igsLocalDir, item.Key.TrimEnd('Z', '.'));


                    if (Gdp.Utils.FileUtil.IsValid(localPath) 
                        || Gdp.Utils.FileUtil.IsValid(localPath2))
                    {
                        isContains = true;
                        break;
                    }
                }
                if (!isContains)
                {
                    notIncludes.Add(item.Value);
                }
            }
            var msg =  notIncludes.Count + " are not included in the inventory, and hence the generated address has been replaced with addresses not included in the inventory.";
            Gdp.Utils.FormUtil.ShowOkMessageBox(msg);
            ShowInfo(msg);

            this.richTextBoxControl_allUrls.Lines = notIncludes.ToArray();

        }

        private void Label9_Click(object sender, EventArgs e)
        {

        }
    }
}
