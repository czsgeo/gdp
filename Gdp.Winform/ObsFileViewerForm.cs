//2018.09.21, czs, create in hmx, 观测文件编辑器
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Gdp.Utils;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Gdp.Data.Rinex;
using Gdp.Data;
using Gdp.IO;
using System.Windows.Forms.DataVisualization.Charting;


namespace Gdp.Winform
{
    public partial class ObsFileViewerForm : Form
    {
        Log log = new Log(typeof(ObsFileViewerForm));

        public ObsFileViewerForm()
        {
            InitializeComponent();

            fileOpenControl_ofilePath.Filter = Setting.RinexOFileFilter;
            fileOpenControl_ofilePath.FilePath = Setting.SampleOFile;
            isInitialized = true; 
        }

        bool isInitialized = false;
        bool IsEnableSp3=> checkBox_enableSp3.Checked;
        #region Property
        /// <summary>
        /// Observation file
        /// </summary>
        public Data.Rinex.RinexObsFile ObsFile { get; set; }
        /// <summary>
        /// File path
        /// </summary>
        public string ObsPath { get { return fileOpenControl_ofilePath.FilePath; } set { fileOpenControl_ofilePath.FilePath = value; } }
        /// <summary>
        /// Current satellite
        /// </summary>
        SatelliteNumber CurrrentPrn { get; set; }
        /// <summary>
        /// Current table
        /// </summary>
        ObjectTableStorage TableObjectStorage { get; set; }
        #endregion

        private void button_read_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ObsPath))
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("The file does not  exist!");
                return;
            }

            this.ObsFile = ObsFileUtil.ReadFile(ObsPath);

            //View file info 
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("FileName:\t" + Path.GetFileName(ObsPath));
            sb.AppendLine("MarkerName:\t" + this.ObsFile.Header.MarkerName);
            sb.AppendLine("Antenna:\t" + this.ObsFile.Header.SiteInfo.AntennaType + "  " + this.ObsFile.Header.SiteInfo.AntennaNumber);

            sb.AppendLine("TimePeriod:\t" + this.ObsFile.Header.TimePeriod);
            sb.AppendLine("Interval(s):\t" + this.ObsFile.Header.Interval);
            var fileSize = new FileInfo(ObsPath).Length;
            var mb = fileSize / 1024.0 / 1024.0;
            sb.AppendLine("FileSize:\t" + mb.ToString("0.00") + " MB");
            var prns = this.ObsFile.GetSatTypedPrns();
            int total = 0;
            foreach (var kv in prns)
            {
                sb.AppendLine(kv.Key + " (Count " + kv.Value.Count + "):\t" + Gdp.Utils.StringUtil.ToString(kv.Value, ","));
                total += kv.Value.Count;
            }
            sb.AppendLine("Total Sats :\t" + total);

            var codes = this.ObsFile.Header.ObsCodes;
            foreach (var kv in codes)
            {
                sb.AppendLine(kv.Key + " ObsCode(" + kv.Value.Count + "):\t" + Gdp.Utils.StringUtil.ToString(kv.Value, ","));
            }

            richTextBoxControl_info.Text = sb.ToString();


            EntityToUi();
        }
        public bool IsShowL1Only { get => checkBox_show1Only.Checked; }
        public void EntityToUi()
        {
            InitTreeView();

            CheckOrReadObsFile();
            SatelliteNumber prn = GetCurrentSelectedPrn();// (SatelliteNumber)   this.bindingSource_sat.Current;
            ShowData(prn);
        }

        public void ShowData(SatelliteNumber prn)
        {
            ObjectTableStorage table = this.ObsFile.BuildObjectTable(prn, IsShowL1Only);
            DataBind(table);
        }


        /// <summary>
        /// Data Binding
        /// </summary>
        /// <param name="table"></param>
        public void DataBind(ObjectTableStorage table)
        {
            var title = table.Name + "， Toatal " + table.ColCount + "Col × " + table.RowCount + "Row";
            log.Debug("Binding shows" + title);
            this.TableObjectStorage = table;
            var dataTable = table.GetDataTable();

            //bindingSource1.DataSource = dataTable;
            this.dataGridView1.DataSource = dataTable;
            tabPage1.Text = CurrrentPrn + "-Data content";
            ShowInfo("Epoch Count: " + dataTable.Rows.Count + ",  " + table.FirstIndex + "->" + table.LastIndex);
            //toolStripLabel_info.Text = title;
            //bindingSource1.DataSource = this.dataGridView1.DataSource;
            //bindingNavigator1.BindingSource = bindingSource1;
        }

        public void UiToEntity()
        {

        }

        public void ShowInfo(string info)
        {
            this.Invoke(new Action(delegate ()
            {
                toolStripLabel1.Text = info;
            }));
        }

        void InitTreeView()
        {
            CheckOrReadObsFile();
            this.treeView1.Nodes.Clear();

            var satTypes = ObsFile.Header.SatelliteTypes;
            var dic = ObsFile.GetSatTypedPrns();

            foreach (var kv in dic)
            {
                var satTypeNode = treeView1.Nodes.Add(kv.Key.ToString());
                satTypeNode.Tag = kv.Key;
                satTypeNode.ContextMenuStrip = this.contextMenuStrip_Sattype;// removeThisToolStripMenuItem;

                foreach (var prn in kv.Value)
                {
                    var satNode = satTypeNode.Nodes.Add(prn.ToString());
                    satNode.ContextMenuStrip = contextMenuStrip_prn;
                    satNode.Tag = prn;
                }
            }
            this.treeView1.ExpandAll();
        }

        private void ObsFileEditorForm_Load(object sender, EventArgs e)
        {
        }

        private void button_saveTo_Click(object sender, EventArgs e)
        {
            var toPath = Path.Combine(Setting.TempDirectory, Path.GetFileName(ObsPath));
            using (RinexObsFileWriter writer = new RinexObsFileWriter(toPath))
            {
                writer.Write(ObsFile);
            }
            Gdp.Utils.FormUtil.ShowOkAndOpenDirectory(Path.GetDirectoryName(toPath));
        }

        private void buttonViewOnChart_Click(object sender, EventArgs e)
        {
            //if(ObsFile == null) { Gdp.Utils.FormUtil.ShowWarningMessageBox("Pleae read!");  return; }
            //bool isDrawAllPhase = checkBox1ViewAllPhase.Checked; 
            //var isSort = checkBox_sortPrn.Checked;
            //var form = new ObsFileChartEditForm(this.ObsFile, ObsPath, true, isSort, isDrawAllPhase, true );
            //form.Show();
        }

        #region Data read

        /// <summary>
        /// Checking. If null, reading data.
        /// </summary>
        private bool CheckOrReadObsFile()
        {
            if (ObsFile == null) { this.ObsFile = ObsFileUtil.ReadFile(ObsPath); }
            if (ObsFile == null) return false;
            return true;
        }
        #endregion


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            this.CurrrentPrn = GetSelectedPrn(node);
            ShowData(CurrrentPrn);
        }
        private SatelliteNumber GetCurrentSelectedPrn()
        {
            var node = this.treeView1.SelectedNode;
            if (node == null) { node = this.treeView1.TopNode; }
            if (node == null) { return SatelliteNumber.Default; }
            this.CurrrentPrn = GetSelectedPrn(node);
            return CurrrentPrn;
        }


        private static SatelliteNumber GetSelectedPrn(TreeNode node)
        {
            if (node.Tag is SatelliteNumber) { return (SatelliteNumber)node.Tag; }
            foreach (TreeNode n in node.Nodes)
            {
                var prn = GetSelectedPrn(n);
                if (prn != SatelliteNumber.Default)
                {
                    return prn;
                }
            }
            return SatelliteNumber.Default;
        } 

        private void OpeninTableOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TableObjectStorage == null || TableObjectStorage.RowCount == 0) { Gdp.Utils.FormUtil.ShowOkMessageBox("数据表为空！"); return; }

            TableObjectViewForm form = new TableObjectViewForm(this.TableObjectStorage);
            form.Show();
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Time> epochs = new List<Time>();
            var rows = dataGridView1.SelectedRows;
            foreach (DataGridViewRow row in rows)
            {
                var epoch = Time.Parse(row.Cells["Epoch"].Value.ToString());
                epochs.Add(epoch);
                dataGridView1.Rows.Remove(row);
            }
            ObsFile.Remove(this.CurrrentPrn, epochs);
        }

        private void DeleteThisSatelliteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = this.treeView1.SelectedNode;// (TreeNode)sender;
            if (node == null) { return; }
            var prn = GetSelectedPrn(node);

            if (Gdp.Utils.FormUtil.ShowYesNoMessageBox(prn + " will be removed!") ==  DialogResult.Yes)
            { 
                ObsFile.Remove(prn); 
                this.EntityToUi();
            } 
        }

        private void DeleteSatellitesWithoutFullEpochesAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var obsCount = ObsFile.Count;
            var prns = ObsFile.GetPrns();
            foreach (var prn in prns)
            {
                if (ObsFile.GetEpochObservations(prn).Count != obsCount)
                {
                    ObsFile.Remove(prn);
                    log.Warn("Remove " + prn);
                }
            }
            this.EntityToUi();

        }

        private void fileOpenControl_ofilePath_FilePathSetted(object sender, EventArgs e)
        {
            if (isInitialized)
                if (Gdp.Utils.FormUtil.ShowYesNoMessageBox("Selected, read immediately?" + fileOpenControl_ofilePath.FilePath) == DialogResult.Yes)
                {
                    button_read_Click(null, null);
                }
        }

        private void Button_viewInOneTable_Click(object sender, EventArgs e)
        {
            var table = this.ObsFile.BuildObjectTable(!IsShowL1Only);
            new TableObjectViewForm(table).Show();
        }

        private void button_viewSatVisibility_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read file first.");
                return;
            }
            ObjectTableStorage table = ObsFileAnalyzer. GetSatVisiblityTable(ObsFile);
            new TableObjectViewForm(table).Show();
        }

       
        private void button_csDetect_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read file first.");
                return;
            } 
            ObjectTableStorage table = ObsFileAnalyzer.GetLiCycleSlipTable(ObsFile, namedFloatControl_lithreshold.GetValue());
            new TableObjectViewForm(table).Show();
        }


        private void button_mwCycleSlipDetect_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read file first.");
                return;
            }
            ObjectTableStorage table = ObsFileAnalyzer.GetMwCycleSlipTable(ObsFile, this.namedFloatControl_mwthreshold.GetValue());
            new TableObjectViewForm(table).Show();

        }

        private void button_litable_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read file first.");
                return;
            }
            var table = ObsFileAnalyzer.GetLiTable(this.ObsFile, FileEphemerisService);
            new TableObjectViewForm(table).Show();

        }

        FileEphemerisService FileEphemerisService
        {
            get
            {
                var path = this.fileOpenControl_sp3.FilePath;
                if(this.IsEnableSp3 && File.Exists(path)){

                    return new SingleSp3FileEphService(path);
                }
                return null;
            }
        }

        private void button_mwTable_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read file first.");
                return;
            }
            var table = ObsFileAnalyzer.GetMwCycleTable(this.ObsFile, FileEphemerisService);
            new TableObjectViewForm(table).Show();
        }

        private void checkBox_enableSp3_CheckedChanged(object sender, EventArgs e)
        {
            this.fileOpenControl_sp3.Enabled = checkBox_enableSp3.Checked;
        }

        private void button_drawnVisibility_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read file first.");
                return;
            }
            ChartForm form = new ChartForm();
            form.DrawVisibility(ObsFile,12);
            form.Show();
        }
         

        private void button_rangeError_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read file first.");
                return;
            }
            var table = ObsFileAnalyzer.GetRangeErrorTable(this.ObsFile,
                                                           this.namedFloatControl_k1.GetValue(),
                                                           this.namedFloatControl_k2.GetValue(),
                                                           FileEphemerisService);
            new TableObjectViewForm(table).Show();

        }

        private void button_mp1Table_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null )//|| this.FileEphemerisService == null //eph is not necesury
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read obs file first.");
                return;
            }
            var table = ObsFileAnalyzer.GetMp1Table(this.ObsFile, FileEphemerisService);
            new TableObjectViewForm(table).Show();

        }

        private void removeThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = this.treeView1.SelectedNode;// (TreeNode)sender;
            if (node == null) { return; }
            var prn = GetSelectedPrn(node);

            if (Gdp.Utils.FormUtil.ShowYesNoMessageBox(prn.SatelliteType + " will be removed!") == DialogResult.Yes)
            {
                ObsFile.Remove(new List<SatelliteType>() { prn.SatelliteType });
                this.EntityToUi();
            }
        }

        private void button_mp2Table_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)//|| this.FileEphemerisService == null //eph is not necesury
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read obs file first.");
                return;
            }
            var table = ObsFileAnalyzer.GetMp2Table(this.ObsFile, FileEphemerisService);
            new TableObjectViewForm(table).Show();
        }

        private void button_mp3Table_Click(object sender, EventArgs e)
        {
            if (this.ObsFile == null)//|| this.FileEphemerisService == null //eph is not necesury
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("Please read obs file first.");
                return;
            }
            var table = ObsFileAnalyzer.GetMp3Table(this.ObsFile, FileEphemerisService);
            new TableObjectViewForm(table).Show();
        }
    }
}
