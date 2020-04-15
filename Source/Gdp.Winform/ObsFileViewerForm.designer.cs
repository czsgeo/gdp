namespace Gdp.Winform
{
    partial class ObsFileViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button_viewInOneTable = new System.Windows.Forms.Button();
            this.button_viewSatVisibility = new System.Windows.Forms.Button();
            this.button_drawnVisibility = new System.Windows.Forms.Button();
            this.fileOpenControl_ofilePath = new Gdp.Winform.Controls.FileOpenControl();
            this.button_read = new System.Windows.Forms.Button();
            this.button_saveTo = new System.Windows.Forms.Button();
            this.checkBox_show1Only = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button_mp1Table = new System.Windows.Forms.Button();
            this.checkBox_enableSp3 = new System.Windows.Forms.CheckBox();
            this.button_litable = new System.Windows.Forms.Button();
            this.button_rangeError = new System.Windows.Forms.Button();
            this.button_mwTable = new System.Windows.Forms.Button();
            this.button_mwCycleSlipDetect = new System.Windows.Forms.Button();
            this.button_csDetect = new System.Windows.Forms.Button();
            this.namedFloatControl_k2 = new Gdp.Winform.Controls.NamedFloatControl();
            this.namedFloatControl_k1 = new Gdp.Winform.Controls.NamedFloatControl();
            this.namedFloatControl_mwthreshold = new Gdp.Winform.Controls.NamedFloatControl();
            this.namedFloatControl_lithreshold = new Gdp.Winform.Controls.NamedFloatControl();
            this.fileOpenControl_sp3 = new Gdp.Winform.Controls.FileOpenControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip_data = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除行RToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.对象表中打开OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBoxControl_info = new Gdp.Winform.Controls.RichTextBoxControl();
            this.contextMenuStrip_prn = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除此星DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除历元不全的卫星AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.contextMenuStrip_Sattype = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button_mp2Table = new System.Windows.Forms.Button();
            this.button_mp3Table = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip_data.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.contextMenuStrip_prn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.contextMenuStrip_Sattype.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(823, 513);
            this.splitContainer1.SplitterDistance = 170;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(823, 170);
            this.tabControl2.TabIndex = 32;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button_viewInOneTable);
            this.tabPage3.Controls.Add(this.button_viewSatVisibility);
            this.tabPage3.Controls.Add(this.button_drawnVisibility);
            this.tabPage3.Controls.Add(this.fileOpenControl_ofilePath);
            this.tabPage3.Controls.Add(this.button_read);
            this.tabPage3.Controls.Add(this.button_saveTo);
            this.tabPage3.Controls.Add(this.checkBox_show1Only);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(815, 144);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Basic";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button_viewInOneTable
            // 
            this.button_viewInOneTable.Location = new System.Drawing.Point(173, 39);
            this.button_viewInOneTable.Name = "button_viewInOneTable";
            this.button_viewInOneTable.Size = new System.Drawing.Size(132, 23);
            this.button_viewInOneTable.TabIndex = 2;
            this.button_viewInOneTable.Text = "View in one table";
            this.button_viewInOneTable.UseVisualStyleBackColor = true;
            this.button_viewInOneTable.Click += new System.EventHandler(this.Button_viewInOneTable_Click);
            // 
            // button_viewSatVisibility
            // 
            this.button_viewSatVisibility.Location = new System.Drawing.Point(311, 39);
            this.button_viewSatVisibility.Name = "button_viewSatVisibility";
            this.button_viewSatVisibility.Size = new System.Drawing.Size(120, 23);
            this.button_viewSatVisibility.TabIndex = 30;
            this.button_viewSatVisibility.Text = "View Visibility";
            this.button_viewSatVisibility.UseVisualStyleBackColor = true;
            this.button_viewSatVisibility.Click += new System.EventHandler(this.button_viewSatVisibility_Click);
            // 
            // button_drawnVisibility
            // 
            this.button_drawnVisibility.Location = new System.Drawing.Point(438, 39);
            this.button_drawnVisibility.Name = "button_drawnVisibility";
            this.button_drawnVisibility.Size = new System.Drawing.Size(120, 23);
            this.button_drawnVisibility.TabIndex = 34;
            this.button_drawnVisibility.Text = "Draw Visibility";
            this.button_drawnVisibility.UseVisualStyleBackColor = true;
            this.button_drawnVisibility.Click += new System.EventHandler(this.button_drawnVisibility_Click);
            // 
            // fileOpenControl_ofilePath
            // 
            this.fileOpenControl_ofilePath.AllowDrop = true;
            this.fileOpenControl_ofilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileOpenControl_ofilePath.FilePath = "";
            this.fileOpenControl_ofilePath.FilePathes = new string[0];
            this.fileOpenControl_ofilePath.Filter = "文本文件|*.txt|所有文件|*.*";
            this.fileOpenControl_ofilePath.FirstPath = "";
            this.fileOpenControl_ofilePath.IsMultiSelect = false;
            this.fileOpenControl_ofilePath.LabelName = "Obs File:";
            this.fileOpenControl_ofilePath.Location = new System.Drawing.Point(13, 11);
            this.fileOpenControl_ofilePath.Name = "fileOpenControl_ofilePath";
            this.fileOpenControl_ofilePath.Size = new System.Drawing.Size(629, 22);
            this.fileOpenControl_ofilePath.TabIndex = 0;
            this.fileOpenControl_ofilePath.FilePathSetted += new System.EventHandler(this.fileOpenControl_ofilePath_FilePathSetted);
            // 
            // button_read
            // 
            this.button_read.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_read.Location = new System.Drawing.Point(648, 11);
            this.button_read.Name = "button_read";
            this.button_read.Size = new System.Drawing.Size(75, 23);
            this.button_read.TabIndex = 1;
            this.button_read.Text = "Read";
            this.button_read.UseVisualStyleBackColor = true;
            this.button_read.Click += new System.EventHandler(this.button_read_Click);
            // 
            // button_saveTo
            // 
            this.button_saveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_saveTo.Location = new System.Drawing.Point(729, 11);
            this.button_saveTo.Name = "button_saveTo";
            this.button_saveTo.Size = new System.Drawing.Size(75, 23);
            this.button_saveTo.TabIndex = 2;
            this.button_saveTo.Text = "Save";
            this.button_saveTo.UseVisualStyleBackColor = true;
            this.button_saveTo.Click += new System.EventHandler(this.button_saveTo_Click);
            // 
            // checkBox_show1Only
            // 
            this.checkBox_show1Only.AutoSize = true;
            this.checkBox_show1Only.Location = new System.Drawing.Point(76, 46);
            this.checkBox_show1Only.Name = "checkBox_show1Only";
            this.checkBox_show1Only.Size = new System.Drawing.Size(66, 16);
            this.checkBox_show1Only.TabIndex = 29;
            this.checkBox_show1Only.Text = "L1 Only";
            this.checkBox_show1Only.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button_mp3Table);
            this.tabPage4.Controls.Add(this.button_mp2Table);
            this.tabPage4.Controls.Add(this.button_mp1Table);
            this.tabPage4.Controls.Add(this.checkBox_enableSp3);
            this.tabPage4.Controls.Add(this.button_litable);
            this.tabPage4.Controls.Add(this.button_rangeError);
            this.tabPage4.Controls.Add(this.button_mwTable);
            this.tabPage4.Controls.Add(this.button_mwCycleSlipDetect);
            this.tabPage4.Controls.Add(this.button_csDetect);
            this.tabPage4.Controls.Add(this.namedFloatControl_k2);
            this.tabPage4.Controls.Add(this.namedFloatControl_k1);
            this.tabPage4.Controls.Add(this.namedFloatControl_mwthreshold);
            this.tabPage4.Controls.Add(this.namedFloatControl_lithreshold);
            this.tabPage4.Controls.Add(this.fileOpenControl_sp3);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(815, 144);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Analyzer";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button_mp1Table
            // 
            this.button_mp1Table.Location = new System.Drawing.Point(496, 34);
            this.button_mp1Table.Name = "button_mp1Table";
            this.button_mp1Table.Size = new System.Drawing.Size(100, 23);
            this.button_mp1Table.TabIndex = 36;
            this.button_mp1Table.Text = "Mp1 Table";
            this.button_mp1Table.UseVisualStyleBackColor = true;
            this.button_mp1Table.Click += new System.EventHandler(this.button_mp1Table_Click);
            // 
            // checkBox_enableSp3
            // 
            this.checkBox_enableSp3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_enableSp3.AutoSize = true;
            this.checkBox_enableSp3.Location = new System.Drawing.Point(659, 11);
            this.checkBox_enableSp3.Name = "checkBox_enableSp3";
            this.checkBox_enableSp3.Size = new System.Drawing.Size(60, 16);
            this.checkBox_enableSp3.TabIndex = 33;
            this.checkBox_enableSp3.Text = "Enable";
            this.checkBox_enableSp3.UseVisualStyleBackColor = true;
            this.checkBox_enableSp3.CheckedChanged += new System.EventHandler(this.checkBox_enableSp3_CheckedChanged);
            // 
            // button_litable
            // 
            this.button_litable.Location = new System.Drawing.Point(346, 34);
            this.button_litable.Name = "button_litable";
            this.button_litable.Size = new System.Drawing.Size(100, 23);
            this.button_litable.TabIndex = 31;
            this.button_litable.Text = "Li Table";
            this.button_litable.UseVisualStyleBackColor = true;
            this.button_litable.Click += new System.EventHandler(this.button_litable_Click);
            // 
            // button_rangeError
            // 
            this.button_rangeError.Location = new System.Drawing.Point(346, 96);
            this.button_rangeError.Name = "button_rangeError";
            this.button_rangeError.Size = new System.Drawing.Size(100, 23);
            this.button_rangeError.TabIndex = 31;
            this.button_rangeError.Text = "Range Error";
            this.button_rangeError.UseVisualStyleBackColor = true;
            this.button_rangeError.Click += new System.EventHandler(this.button_rangeError_Click);
            // 
            // button_mwTable
            // 
            this.button_mwTable.Location = new System.Drawing.Point(346, 63);
            this.button_mwTable.Name = "button_mwTable";
            this.button_mwTable.Size = new System.Drawing.Size(100, 23);
            this.button_mwTable.TabIndex = 31;
            this.button_mwTable.Text = "MW Table";
            this.button_mwTable.UseVisualStyleBackColor = true;
            this.button_mwTable.Click += new System.EventHandler(this.button_mwTable_Click);
            // 
            // button_mwCycleSlipDetect
            // 
            this.button_mwCycleSlipDetect.Location = new System.Drawing.Point(191, 63);
            this.button_mwCycleSlipDetect.Name = "button_mwCycleSlipDetect";
            this.button_mwCycleSlipDetect.Size = new System.Drawing.Size(136, 23);
            this.button_mwCycleSlipDetect.TabIndex = 31;
            this.button_mwCycleSlipDetect.Text = "Mw Cycle slip detect";
            this.button_mwCycleSlipDetect.UseVisualStyleBackColor = true;
            this.button_mwCycleSlipDetect.Click += new System.EventHandler(this.button_mwCycleSlipDetect_Click);
            // 
            // button_csDetect
            // 
            this.button_csDetect.Location = new System.Drawing.Point(191, 34);
            this.button_csDetect.Name = "button_csDetect";
            this.button_csDetect.Size = new System.Drawing.Size(136, 23);
            this.button_csDetect.TabIndex = 31;
            this.button_csDetect.Text = "Li Cycle slip detect";
            this.button_csDetect.UseVisualStyleBackColor = true;
            this.button_csDetect.Click += new System.EventHandler(this.button_csDetect_Click);
            // 
            // namedFloatControl_k2
            // 
            this.namedFloatControl_k2.Location = new System.Drawing.Point(180, 98);
            this.namedFloatControl_k2.Name = "namedFloatControl_k2";
            this.namedFloatControl_k2.Size = new System.Drawing.Size(153, 23);
            this.namedFloatControl_k2.TabIndex = 35;
            this.namedFloatControl_k2.Title = "Threshold of k2:";
            this.namedFloatControl_k2.Value = 60D;
            // 
            // namedFloatControl_k1
            // 
            this.namedFloatControl_k1.Location = new System.Drawing.Point(8, 96);
            this.namedFloatControl_k1.Name = "namedFloatControl_k1";
            this.namedFloatControl_k1.Size = new System.Drawing.Size(155, 23);
            this.namedFloatControl_k1.TabIndex = 35;
            this.namedFloatControl_k1.Title = "Threshold of k1: ";
            this.namedFloatControl_k1.Value = 30D;
            // 
            // namedFloatControl_mwthreshold
            // 
            this.namedFloatControl_mwthreshold.Location = new System.Drawing.Point(8, 63);
            this.namedFloatControl_mwthreshold.Name = "namedFloatControl_mwthreshold";
            this.namedFloatControl_mwthreshold.Size = new System.Drawing.Size(177, 23);
            this.namedFloatControl_mwthreshold.TabIndex = 35;
            this.namedFloatControl_mwthreshold.Title = "Threshold of MW:";
            this.namedFloatControl_mwthreshold.Value = 5D;
            // 
            // namedFloatControl_lithreshold
            // 
            this.namedFloatControl_lithreshold.Location = new System.Drawing.Point(8, 34);
            this.namedFloatControl_lithreshold.Name = "namedFloatControl_lithreshold";
            this.namedFloatControl_lithreshold.Size = new System.Drawing.Size(177, 23);
            this.namedFloatControl_lithreshold.TabIndex = 35;
            this.namedFloatControl_lithreshold.Title = "Threshold of LI: ";
            this.namedFloatControl_lithreshold.Value = 5D;
            // 
            // fileOpenControl_sp3
            // 
            this.fileOpenControl_sp3.AllowDrop = true;
            this.fileOpenControl_sp3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileOpenControl_sp3.Enabled = false;
            this.fileOpenControl_sp3.FilePath = "";
            this.fileOpenControl_sp3.FilePathes = new string[0];
            this.fileOpenControl_sp3.Filter = "Sp3|*.Sp3|文本文件|*.txt|所有文件|*.*";
            this.fileOpenControl_sp3.FirstPath = "";
            this.fileOpenControl_sp3.IsMultiSelect = false;
            this.fileOpenControl_sp3.LabelName = "Sp3File:";
            this.fileOpenControl_sp3.Location = new System.Drawing.Point(8, 6);
            this.fileOpenControl_sp3.Name = "fileOpenControl_sp3";
            this.fileOpenControl_sp3.Size = new System.Drawing.Size(629, 22);
            this.fileOpenControl_sp3.TabIndex = 32;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(823, 339);
            this.splitContainer2.SplitterDistance = 234;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(234, 339);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(585, 339);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(577, 313);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Content";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip_data;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 28);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(571, 282);
            this.dataGridView1.TabIndex = 0;
            // 
            // contextMenuStrip_data
            // 
            this.contextMenuStrip_data.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除行RToolStripMenuItem,
            this.对象表中打开OToolStripMenuItem});
            this.contextMenuStrip_data.Name = "contextMenuStrip1";
            this.contextMenuStrip_data.Size = new System.Drawing.Size(167, 48);
            // 
            // 删除行RToolStripMenuItem
            // 
            this.删除行RToolStripMenuItem.Name = "删除行RToolStripMenuItem";
            this.删除行RToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.删除行RToolStripMenuItem.Text = "删除行(&R)";
            this.删除行RToolStripMenuItem.Click += new System.EventHandler(this.DeleteRowToolStripMenuItem_Click);
            // 
            // 对象表中打开OToolStripMenuItem
            // 
            this.对象表中打开OToolStripMenuItem.Name = "对象表中打开OToolStripMenuItem";
            this.对象表中打开OToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.对象表中打开OToolStripMenuItem.Text = "对象表中打开(&O)";
            this.对象表中打开OToolStripMenuItem.Click += new System.EventHandler(this.OpeninTableOToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(571, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(31, 22);
            this.toolStripLabel1.Text = "Info";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBoxControl_info);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(577, 313);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "File Info";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBoxControl_info
            // 
            this.richTextBoxControl_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxControl_info.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxControl_info.MaxAppendLineCount = 5000;
            this.richTextBoxControl_info.Name = "richTextBoxControl_info";
            this.richTextBoxControl_info.Size = new System.Drawing.Size(571, 307);
            this.richTextBoxControl_info.TabIndex = 0;
            this.richTextBoxControl_info.Text = "";
            // 
            // contextMenuStrip_prn
            // 
            this.contextMenuStrip_prn.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除此星DToolStripMenuItem,
            this.删除历元不全的卫星AToolStripMenuItem});
            this.contextMenuStrip_prn.Name = "contextMenuStrip_prn";
            this.contextMenuStrip_prn.Size = new System.Drawing.Size(298, 48);
            // 
            // 删除此星DToolStripMenuItem
            // 
            this.删除此星DToolStripMenuItem.Name = "删除此星DToolStripMenuItem";
            this.删除此星DToolStripMenuItem.Size = new System.Drawing.Size(297, 22);
            this.删除此星DToolStripMenuItem.Text = "Delete this sat(&D)";
            this.删除此星DToolStripMenuItem.Click += new System.EventHandler(this.DeleteThisSatelliteToolStripMenuItem_Click);
            // 
            // 删除历元不全的卫星AToolStripMenuItem
            // 
            this.删除历元不全的卫星AToolStripMenuItem.Name = "删除历元不全的卫星AToolStripMenuItem";
            this.删除历元不全的卫星AToolStripMenuItem.Size = new System.Drawing.Size(297, 22);
            this.删除历元不全的卫星AToolStripMenuItem.Text = "Delete sats with incomplete epochs(&A)";
            this.删除历元不全的卫星AToolStripMenuItem.Click += new System.EventHandler(this.DeleteSatellitesWithoutFullEpochesAToolStripMenuItem_Click);
            // 
            // contextMenuStrip_Sattype
            // 
            this.contextMenuStrip_Sattype.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeThisToolStripMenuItem});
            this.contextMenuStrip_Sattype.Name = "contextMenuStrip_Sattype";
            this.contextMenuStrip_Sattype.Size = new System.Drawing.Size(148, 26);
            // 
            // removeThisToolStripMenuItem
            // 
            this.removeThisToolStripMenuItem.Name = "removeThisToolStripMenuItem";
            this.removeThisToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.removeThisToolStripMenuItem.Text = "Remove this";
            this.removeThisToolStripMenuItem.Click += new System.EventHandler(this.removeThisToolStripMenuItem_Click);
            // 
            // button_mp2Table
            // 
            this.button_mp2Table.Location = new System.Drawing.Point(496, 63);
            this.button_mp2Table.Name = "button_mp2Table";
            this.button_mp2Table.Size = new System.Drawing.Size(100, 23);
            this.button_mp2Table.TabIndex = 37;
            this.button_mp2Table.Text = "Mp2 Table";
            this.button_mp2Table.UseVisualStyleBackColor = true;
            this.button_mp2Table.Click += new System.EventHandler(this.button_mp2Table_Click);
            // 
            // button_mp3Table
            // 
            this.button_mp3Table.Location = new System.Drawing.Point(496, 96);
            this.button_mp3Table.Name = "button_mp3Table";
            this.button_mp3Table.Size = new System.Drawing.Size(100, 23);
            this.button_mp3Table.TabIndex = 38;
            this.button_mp3Table.Text = "Mp3 Table";
            this.button_mp3Table.UseVisualStyleBackColor = true;
            this.button_mp3Table.Click += new System.EventHandler(this.button_mp3Table_Click);
            // 
            // ObsFileViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 513);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ObsFileViewerForm";
            this.ShowIcon = false;
            this.Text = "Obs File Viewer / Editer";
            this.Load += new System.EventHandler(this.ObsFileEditorForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip_data.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.contextMenuStrip_prn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.contextMenuStrip_Sattype.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button_read;
        private Gdp.Winform.Controls.FileOpenControl fileOpenControl_ofilePath;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_data;
        private System.Windows.Forms.ToolStripMenuItem 删除行RToolStripMenuItem;
        private System.Windows.Forms.Button button_saveTo;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox checkBox_show1Only;
        private System.Windows.Forms.ToolStripMenuItem 对象表中打开OToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_prn;
        private System.Windows.Forms.ToolStripMenuItem 删除此星DToolStripMenuItem;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ToolStripMenuItem 删除历元不全的卫星AToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.Button button_viewInOneTable;
        private System.Windows.Forms.Button button_viewSatVisibility;
        private System.Windows.Forms.TabPage tabPage2;
        private Controls.RichTextBoxControl richTextBoxControl_info;
        private System.Windows.Forms.Button button_csDetect;
        private System.Windows.Forms.Button button_mwCycleSlipDetect;
        private System.Windows.Forms.Button button_mwTable;
        private System.Windows.Forms.Button button_litable;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private Controls.FileOpenControl fileOpenControl_sp3;
        private System.Windows.Forms.CheckBox checkBox_enableSp3;
        private System.Windows.Forms.Button button_drawnVisibility;
        private Controls.NamedFloatControl namedFloatControl_mwthreshold;
        private Controls.NamedFloatControl namedFloatControl_lithreshold;
        private Controls.NamedFloatControl namedFloatControl_k2;
        private Controls.NamedFloatControl namedFloatControl_k1;
        private System.Windows.Forms.Button button_rangeError;
        private System.Windows.Forms.Button button_mp1Table;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Sattype;
        private System.Windows.Forms.ToolStripMenuItem removeThisToolStripMenuItem;
        private System.Windows.Forms.Button button_mp3Table;
        private System.Windows.Forms.Button button_mp2Table;
    }
}