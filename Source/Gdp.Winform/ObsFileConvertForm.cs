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
using System.IO; 
using Gdp.IO;


namespace Gdp.Winform
{
    public partial class ObsFileConvertForm : Form
    {
        Log log = new Log(typeof(ObsFileConvertForm));
        public ObsFileConvertForm()
        {
            InitializeComponent();
        }

        ObsFileConvertOption ObsFileConvertOption { get; set; }
        bool IsCancel;
        DateTime startTime;
        public string OutputDirectory => this.directorySelectionControl1.Path;

        private void Button_Run_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;

            button_Run.Enabled = false;
            IsCancel = false;
            CurrentRunners.Clear();
            if (!IsParallel) //test
            {
                Run();
            }
            else
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void Button_cancel_Click(object sender, EventArgs e)
        {
            IsCancel = true;
            foreach (var item in CurrentRunners)
            {
                item.IsCancel = this.IsCancel;
            }
        }

        bool IsParallel => checkBox_parallel.Checked;

        private void Button_detailSetting_Click(object sender, EventArgs e)
        {
            this.ObsFileConvertOption = GetOrInitObsFileFormatOption();
            var form = new ObsFileConvertOptionForm(ObsFileConvertOption);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ObsFileConvertOption = form.Option;
            }
        }
        public ObsFileConvertOption GetOrInitObsFileFormatOption()
        {
            if (ObsFileConvertOption == null) { ObsFileConvertOption = new ObsFileConvertOption(); }

            ObsFileConvertOption.OutputDirectory = this.OutputDirectory;
            return ObsFileConvertOption;
        }

        private void ShowInfo(string info)
        {
            this.Invoke(new Action(() =>
            {
                this.label_info.Text = info;
            }));
        }
         
        private void ObsFileConvertForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy && Utils.FormUtil.ShowYesNoMessageBox("Please wait, the background is working. Still quit?") == DialogResult.No) { e.Cancel = true; }
            else { IsCancel = true; }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Run();
        }

        private void Run()
        {
            var InputRawPathes = this.fileOpenControl1.FilePathes;
            ObsFileConvertOption opt = GetOrInitObsFileFormatOption();

            this.Invoke(new Action(() =>
            {
                this.progressBar1.Maximum = InputRawPathes.Length;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Value = 0;
                this.progressBar1.Step = 1;
            }));

            if (IsParallel)
            {
                var count = namedIntControl_processCount.GetValue();
                ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = count };
                Parallel.ForEach(InputRawPathes, parallelOptions, (inputPath, state) =>
                {
                    if (IsCancel || this.backgroundWorker1.CancellationPending) { ShowInfo("Canceled at :" + inputPath); state.Stop(); }

                    string subDir = Gdp.Utils.PathUtil.GetSubDirectory(InputRawPathes, inputPath);

                    Process(subDir, opt, inputPath);
                });
            }
            else
            {
                foreach (var inputPath in InputRawPathes)
                {
                    if (IsCancel)
                    {
                        ShowInfo("Canceled at :" + inputPath);
                        break;
                    }
                    string subDir = Gdp.Utils.PathUtil.GetSubDirectory(InputRawPathes, inputPath);

                    Process(subDir, opt, inputPath);
                }

                Complete();

            }
        }

        List<ObsFileFormater> CurrentRunners = new List<ObsFileFormater>();
        private void Process(string subDir, ObsFileConvertOption opt, string inputPath)
        {
            try
            {
                ShowInfo("processing :" + inputPath);

                ObsFileFormater ObsFileFormater = new ObsFileFormater(opt, inputPath);
                CurrentRunners.Add(ObsFileFormater);
                ObsFileFormater.SubDirectory = subDir;
                ObsFileFormater.Init();
                ObsFileFormater.Run();
                CurrentRunners.Remove(ObsFileFormater);
                this.Invoke(new Action(() =>
                {
                    this.progressBar1.PerformStep();
                    this.progressBar1.Refresh();
                }));
            }catch(Exception ex)
            {
                log.Error("转换出错：\t" + inputPath + "\t, "+ ex.Message );
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
                button_Run.Enabled = true;
                var span = DateTime.Now - startTime;
                var m = span.TotalMinutes;
                var seconds = span.TotalSeconds;
                int FileCount = this.fileOpenControl1.FileCount;

                string info = "Completed, the total number is " + FileCount + ". The time consumption is " + m.ToString("0.000") + "m=" + seconds.ToString("0.00") + "s, with an average of " + (seconds / FileCount).ToString("0.000") + "s each file";

                Utils.FormUtil.ShowOkAndOpenDirectory(OutputDirectory, info + ", open it?");
                ShowInfo(info);
                log.Info(info);
                this.progressBar1.Value = this.progressBar1.Maximum;
            }
        }

        private void FileOpenControl1_FilePathSetted(object sender, EventArgs e)
        {
            if (!File.Exists(this.directorySelectionControl1.Path))
            {
                this.directorySelectionControl1.Path = Path.Combine(Path.GetDirectoryName(this.fileOpenControl1.FilePath), "Formated");
            }
            ShowInfo(fileOpenControl1.FileCount + " files selected");
        }

        private void ObsFileConvertForm_Load(object sender, EventArgs e)
        {
            this.fileOpenControl1.FilePath = Setting.SampleOFile;
            namedIntControl_processCount.SetValue(Environment.ProcessorCount);
        }
    }
}
