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
    public partial class ObsFileSelectorForm : Form
    {
        Log log = new Log(typeof(ObsFileSelectorForm));
        public ObsFileSelectorForm()
        {
            InitializeComponent();
        }

        ObsFileSelectOption ObsFileSelectOption { get; set; }
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

        bool IsParallel => checkBox_parallel.Checked;

        private void Button_detailSetting_Click(object sender, EventArgs e)
        {
            this.ObsFileSelectOption = GetOrInitObsFileSelectOption();
            var form = new ObsFileSelectOptionForm(ObsFileSelectOption);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ObsFileSelectOption = form.Option;
            }
        }
        public ObsFileSelectOption GetOrInitObsFileSelectOption()
        {
            if (ObsFileSelectOption == null) { ObsFileSelectOption = new ObsFileSelectOption(); }

          //  ObsFileSelectOption.OutputDirectory = this.OutputDirectory;
            return ObsFileSelectOption;
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
        ObsFileSelector ObsFileSelector;
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var InputRawPathes = this.fileOpenControl1.FilePathes; 


            ShowInfo("Start execution");
            this.ObsFileSelectOption = GetOrInitObsFileSelectOption();
            SelectedCount = 0;


            this. ObsFileSelector = new ObsFileSelector(ObsFileSelectOption, OutputDirectory);
     
   
            this.Invoke(new Action(() =>
            {
                this.progressBar1.Maximum = InputRawPathes.Length;
                this.progressBar1.Minimum = 0;
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

                  Process(subDir, ObsFileSelectOption, inputPath);
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

                    Process(subDir, ObsFileSelectOption, inputPath);
                }
            }
        }

        int SelectedCount = 0;
        private void Process(string subDir, ObsFileSelectOption opt, string inputPath)
        {
            ShowInfo("processing :" + inputPath);

            if (ObsFileSelector.Select(inputPath, subDir))
            {
                SelectedCount++;
            }
             

            this.Invoke(new Action(() =>
            {
                this.progressBar1.PerformStep();
                this.progressBar1.Refresh();
            }));
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button_Run.Enabled = true;
            var span = DateTime.Now - startTime;
            var m = span.TotalMinutes;
            var seconds = span.TotalSeconds;
            int FileCount = this.fileOpenControl1.FileCount;

            string info = "Completed, the selected number is " + SelectedCount + ", and the total number is "+ FileCount + ", the total time consumption is " + m.ToString("0.000") + "m="+ seconds .ToString("0.00")+ "s, with an average of" + (seconds / FileCount).ToString("0.000") + " s each file";

            Utils.FormUtil.ShowOkAndOpenDirectory(OutputDirectory, info + ", access it or not?");
            ShowInfo(info);
            log.Info(info);
            this.progressBar1.Value = this.progressBar1.Maximum;
        }

        private void FileOpenControl1_FilePathSetted(object sender, EventArgs e)
        {
            if (!File.Exists(this.directorySelectionControl1.Path))
            {
                this.directorySelectionControl1.Path = Path.Combine(Path.GetDirectoryName(this.fileOpenControl1.FilePath), "Selected");
            }
            ShowInfo(fileOpenControl1.FileCount + " files selected");
        }

        private void ObsFileSelectorForm_Load(object sender, EventArgs e)
        {
            this.fileOpenControl1.FilePath = Setting.SampleOFile;

            namedIntControl_processCount.SetValue(Environment.ProcessorCount);
        }
    }
}
