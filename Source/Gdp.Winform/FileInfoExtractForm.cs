//2019.02.15, czs, create in hongqing, creating 文件提取



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; 
using Gdp.IO;
using Gdp.Data.Rinex;

namespace Gdp.Winform
{
    public partial class FileInfoExtractForm : Form
    {
        Log log = new Log(typeof(FileInfoExtractForm));
        public FileInfoExtractForm()
        {
            InitializeComponent();
        }

        bool IsCancel;
        DateTime startTime;
        public string OutputDirectory => this.directorySelectionControl1.Path;

        private void Button_Run_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;

            button_Run.Enabled = false;
            IsCancel = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void Button_cancel_Click(object sender, EventArgs e)
        {
            IsCancel = true;
        }



        private void ShowInfo(string info)
        {
            this.Invoke(new Action(() =>
            {
                this.label_info.Text = info;
            }));
        }
        bool IsLonLatFirst => this.checkBox_gmtCorrdFirst.Checked;

        private void ObsFileConvertForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy && Utils.FormUtil.ShowYesNoMessageBox("Please wait, the background is working. Still quit?") == DialogResult.No) { e.Cancel = true; }
            else { IsCancel = true; }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Run();
        }
        ObjectTableStorage table = new ObjectTableStorage();
        private void Run()
        {
            table = new ObjectTableStorage();

            var InputRawPathes = this.fileOpenControl1.FilePathes;

            this.Invoke(new Action(() =>
            {
                this.progressBar1.Maximum = InputRawPathes.Length;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Value = 0;
                this.progressBar1.Step = 1;
            }));

            foreach (var inputPath in InputRawPathes)
            {
                if (IsCancel)
                {
                    ShowInfo("Canceled at :" + inputPath);
                    break;
                }
                string subDir = Gdp.Utils.PathUtil.GetSubDirectory(InputRawPathes, inputPath);

                Process(subDir, inputPath);
            }
        }


        private void Process(string subDir, string inputPath)
        {
            try
            {
                ShowInfo("processing :" + inputPath);

                var header = RinexObsFileReader.ReadHeader(inputPath);
                var siteInfo = header.SiteInfo;
                var obsInfo = header.ObsInfo;
                table.NewRow();
                if (IsLonLatFirst)
                {
                    table.AddItem("Lon", siteInfo.ApproxGeoCoord.Lon);
                    table.AddItem("Lat", siteInfo.ApproxGeoCoord.Lat);
                    table.AddItem("Name", siteInfo.SiteName);
                }
                else
                {
                    table.AddItem("Name", siteInfo.SiteName);
                    table.AddItem("Lon", siteInfo.ApproxGeoCoord.Lon);
                    table.AddItem("Lat", siteInfo.ApproxGeoCoord.Lat);
                }
                table.AddItem("Height", siteInfo.ApproxGeoCoord.Height);
                table.AddItem("X", siteInfo.ApproxXyz.X);
                table.AddItem("Y", siteInfo.ApproxXyz.Y);
                table.AddItem("Z", siteInfo.ApproxXyz.Z);
                table.AddItem("ReceiverType", siteInfo.ReceiverType);
                table.AddItem("ReceiverNumber", siteInfo.ReceiverNumber);
                table.AddItem("AntennaType", siteInfo.AntennaType);
                table.AddItem("AntennaNumber", siteInfo.AntennaNumber);


                this.Invoke(new Action(() =>
                {
                    this.progressBar1.PerformStep();
                    this.progressBar1.Refresh();
                }));
            }
            catch (Exception ex)
            {
                log.Error("转换出错：\t" + inputPath + "\t, " + ex.Message);
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Complete();
        }

        private void Complete()
        {
            if (!button_Run.Enabled)
            {
                table.Name = "SiteInfo";
                var path = Path.Combine(this.OutputDirectory, "SiteInfo.txt.xls");

                Utils.FileUtil.CheckOrCreateDirectory(this.OutputDirectory);


                button_Run.Enabled = true;
                var span = DateTime.Now - startTime;
                var m = span.TotalMinutes;
                var seconds = span.TotalSeconds;
                int FileCount = this.fileOpenControl1.FileCount;
                TableObjectViewForm form = new TableObjectViewForm(table);
                form.Show();
                ObjectTableWriter.Write(table, path);
                string info = "Completed, the total number is " + FileCount + ". The time consumption is " + m.ToString("0.000") + "m=" + seconds.ToString("0.00") + "s, with an average of " + (seconds / FileCount).ToString("0.000") + "s each file";

                Utils.FormUtil.ShowOkAndOpenDirectory(path, info + ", open it?");
                ShowInfo(info);
                log.Info(info);
                this.progressBar1.Value = this.progressBar1.Maximum;
            }
        }

        private void FileOpenControl1_FilePathSetted(object sender, EventArgs e)
        {
            if (!File.Exists(this.directorySelectionControl1.Path))
            {
                this.directorySelectionControl1.Path = Path.Combine(Path.GetDirectoryName(this.fileOpenControl1.FilePath), "Temp");
            }
            ShowInfo(fileOpenControl1.FileCount + " files selected");
        }

        private void ObsFileConvertForm_Load(object sender, EventArgs e)
        {
            this.fileOpenControl1.FilePath = Setting.SampleOFile;
        }
    }

}
