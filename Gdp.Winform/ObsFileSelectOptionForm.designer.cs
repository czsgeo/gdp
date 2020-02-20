namespace Gdp.Winform
{
    partial class ObsFileSelectOptionForm
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
            this.button_ok = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.multiGnssSystemSelectControl1 = new Gdp.Winform.Controls.MultiGnssSystemSelectControl();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_includeSiteNames = new System.Windows.Forms.TextBox();
            this.checkBox_enableincludeSiteNames = new System.Windows.Forms.CheckBox();
            this.textBox_excludeSites = new System.Windows.Forms.TextBox();
            this.checkBox_enableExcludeSiteNames = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox_enableSatelliteTypes = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.timePeriodControl1 = new Gdp.Winform.Controls.TimePeriodControl();
            this.checkBox_enableTimePeriod = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_centerXyz = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_radiusFromCenter = new System.Windows.Forms.TextBox();
            this.checkBox_enableRegionFilter = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1MinFileSizeMB = new System.Windows.Forms.TextBox();
            this.checkBox1MinFileSizeMB = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1MinEpochCount = new System.Windows.Forms.TextBox();
            this.checkBox1MinEpochCount = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_obsCodes = new System.Windows.Forms.TextBox();
            this.checkBox_enableCode = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.enabledFloatControl_mp2Factor = new Gdp.Winform.Controls.EnabledFloatControl();
            this.enabledFloatControl_mp1Factor = new Gdp.Winform.Controls.EnabledFloatControl();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox2MinSatCountRatio = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox1MinSatCount = new System.Windows.Forms.TextBox();
            this.checkBox1MinSatCount = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox1MinFrequencyCount = new System.Windows.Forms.TextBox();
            this.checkBox1MinFrequencyCount = new System.Windows.Forms.CheckBox();
            this.checkBox_IsNavCopy = new System.Windows.Forms.CheckBox();
            this.button_reset = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_ok
            // 
            this.button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ok.Location = new System.Drawing.Point(411, 526);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 30);
            this.button_ok.TabIndex = 0;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(564, 517);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.multiGnssSystemSelectControl1);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.checkBox_enableSatelliteTypes);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(556, 491);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Basic Options";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // multiGnssSystemSelectControl1
            // 
            this.multiGnssSystemSelectControl1.Location = new System.Drawing.Point(6, 7);
            this.multiGnssSystemSelectControl1.Margin = new System.Windows.Forms.Padding(2);
            this.multiGnssSystemSelectControl1.Name = "multiGnssSystemSelectControl1";
            this.multiGnssSystemSelectControl1.Size = new System.Drawing.Size(456, 39);
            this.multiGnssSystemSelectControl1.TabIndex = 9;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.textBox_includeSiteNames);
            this.groupBox4.Controls.Add(this.checkBox_enableincludeSiteNames);
            this.groupBox4.Controls.Add(this.textBox_excludeSites);
            this.groupBox4.Controls.Add(this.checkBox_enableExcludeSiteNames);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(6, 174);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(538, 132);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Stations";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(33, 109);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(329, 23);
            this.label12.TabIndex = 4;
            this.label12.Text = "Note: Support for line breaks, commas, spaces, etc.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Include: ";
            // 
            // textBox_includeSiteNames
            // 
            this.textBox_includeSiteNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_includeSiteNames.Location = new System.Drawing.Point(71, 14);
            this.textBox_includeSiteNames.Multiline = true;
            this.textBox_includeSiteNames.Name = "textBox_includeSiteNames";
            this.textBox_includeSiteNames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_includeSiteNames.Size = new System.Drawing.Size(385, 44);
            this.textBox_includeSiteNames.TabIndex = 1;
            // 
            // checkBox_enableincludeSiteNames
            // 
            this.checkBox_enableincludeSiteNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_enableincludeSiteNames.AutoSize = true;
            this.checkBox_enableincludeSiteNames.Location = new System.Drawing.Point(469, 16);
            this.checkBox_enableincludeSiteNames.Name = "checkBox_enableincludeSiteNames";
            this.checkBox_enableincludeSiteNames.Size = new System.Drawing.Size(60, 16);
            this.checkBox_enableincludeSiteNames.TabIndex = 3;
            this.checkBox_enableincludeSiteNames.Text = "Enable";
            this.checkBox_enableincludeSiteNames.UseVisualStyleBackColor = true;
            // 
            // textBox_excludeSites
            // 
            this.textBox_excludeSites.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_excludeSites.Location = new System.Drawing.Point(71, 64);
            this.textBox_excludeSites.Multiline = true;
            this.textBox_excludeSites.Name = "textBox_excludeSites";
            this.textBox_excludeSites.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_excludeSites.Size = new System.Drawing.Size(385, 42);
            this.textBox_excludeSites.TabIndex = 1;
            // 
            // checkBox_enableExcludeSiteNames
            // 
            this.checkBox_enableExcludeSiteNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_enableExcludeSiteNames.AutoSize = true;
            this.checkBox_enableExcludeSiteNames.Location = new System.Drawing.Point(469, 64);
            this.checkBox_enableExcludeSiteNames.Name = "checkBox_enableExcludeSiteNames";
            this.checkBox_enableExcludeSiteNames.Size = new System.Drawing.Size(60, 16);
            this.checkBox_enableExcludeSiteNames.TabIndex = 3;
            this.checkBox_enableExcludeSiteNames.Text = "Enable";
            this.checkBox_enableExcludeSiteNames.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Exclude: ";
            // 
            // checkBox_enableSatelliteTypes
            // 
            this.checkBox_enableSatelliteTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_enableSatelliteTypes.AutoSize = true;
            this.checkBox_enableSatelliteTypes.Location = new System.Drawing.Point(472, 21);
            this.checkBox_enableSatelliteTypes.Name = "checkBox_enableSatelliteTypes";
            this.checkBox_enableSatelliteTypes.Size = new System.Drawing.Size(60, 16);
            this.checkBox_enableSatelliteTypes.TabIndex = 2;
            this.checkBox_enableSatelliteTypes.Text = "Enable";
            this.checkBox_enableSatelliteTypes.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.timePeriodControl1);
            this.groupBox3.Controls.Add(this.checkBox_enableTimePeriod);
            this.groupBox3.Location = new System.Drawing.Point(6, 104);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(538, 51);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Time Period Filter";
            // 
            // timePeriodControl1
            // 
            this.timePeriodControl1.Location = new System.Drawing.Point(3, 20);
            this.timePeriodControl1.Margin = new System.Windows.Forms.Padding(2);
            this.timePeriodControl1.Name = "timePeriodControl1";
            this.timePeriodControl1.Size = new System.Drawing.Size(449, 24);
            this.timePeriodControl1.TabIndex = 4;
            this.timePeriodControl1.TimeFrom = new System.DateTime(2016, 10, 28, 13, 13, 20, 890);
            this.timePeriodControl1.TimeStringFormat = "yyyy-MM-dd HH:mm";
            this.timePeriodControl1.TimeTo = new System.DateTime(2016, 10, 28, 13, 13, 20, 899);
            this.timePeriodControl1.Title = "Time period:";
            // 
            // checkBox_enableTimePeriod
            // 
            this.checkBox_enableTimePeriod.AutoSize = true;
            this.checkBox_enableTimePeriod.Location = new System.Drawing.Point(469, 23);
            this.checkBox_enableTimePeriod.Name = "checkBox_enableTimePeriod";
            this.checkBox_enableTimePeriod.Size = new System.Drawing.Size(60, 16);
            this.checkBox_enableTimePeriod.TabIndex = 3;
            this.checkBox_enableTimePeriod.Text = "Enable";
            this.checkBox_enableTimePeriod.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_centerXyz);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBox_radiusFromCenter);
            this.groupBox2.Controls.Add(this.checkBox_enableRegionFilter);
            this.groupBox2.Location = new System.Drawing.Point(9, 312);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(535, 77);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Central Region";
            // 
            // textBox_centerXyz
            // 
            this.textBox_centerXyz.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_centerXyz.Location = new System.Drawing.Point(130, 20);
            this.textBox_centerXyz.Name = "textBox_centerXyz";
            this.textBox_centerXyz.Size = new System.Drawing.Size(399, 21);
            this.textBox_centerXyz.TabIndex = 1;
            this.textBox_centerXyz.Text = "1000,10000,1000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Center coordinates:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Radius (m):";
            // 
            // textBox_radiusFromCenter
            // 
            this.textBox_radiusFromCenter.Location = new System.Drawing.Point(130, 47);
            this.textBox_radiusFromCenter.Name = "textBox_radiusFromCenter";
            this.textBox_radiusFromCenter.Size = new System.Drawing.Size(100, 21);
            this.textBox_radiusFromCenter.TabIndex = 1;
            // 
            // checkBox_enableRegionFilter
            // 
            this.checkBox_enableRegionFilter.AutoSize = true;
            this.checkBox_enableRegionFilter.Location = new System.Drawing.Point(246, 52);
            this.checkBox_enableRegionFilter.Name = "checkBox_enableRegionFilter";
            this.checkBox_enableRegionFilter.Size = new System.Drawing.Size(60, 16);
            this.checkBox_enableRegionFilter.TabIndex = 2;
            this.checkBox_enableRegionFilter.Text = "Enable";
            this.checkBox_enableRegionFilter.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.textBox1MinFileSizeMB);
            this.groupBox6.Controls.Add(this.checkBox1MinFileSizeMB);
            this.groupBox6.Location = new System.Drawing.Point(9, 440);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(529, 41);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Minimum File Size";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(244, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "MB";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "Size:";
            // 
            // textBox1MinFileSizeMB
            // 
            this.textBox1MinFileSizeMB.Location = new System.Drawing.Point(77, 13);
            this.textBox1MinFileSizeMB.Name = "textBox1MinFileSizeMB";
            this.textBox1MinFileSizeMB.Size = new System.Drawing.Size(153, 21);
            this.textBox1MinFileSizeMB.TabIndex = 1;
            this.textBox1MinFileSizeMB.Text = "0.5";
            this.textBox1MinFileSizeMB.TextChanged += new System.EventHandler(this.TextBox1MinFileSizeMB_TextChanged);
            // 
            // checkBox1MinFileSizeMB
            // 
            this.checkBox1MinFileSizeMB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1MinFileSizeMB.AutoSize = true;
            this.checkBox1MinFileSizeMB.Location = new System.Drawing.Point(451, 15);
            this.checkBox1MinFileSizeMB.Name = "checkBox1MinFileSizeMB";
            this.checkBox1MinFileSizeMB.Size = new System.Drawing.Size(60, 16);
            this.checkBox1MinFileSizeMB.TabIndex = 2;
            this.checkBox1MinFileSizeMB.Text = "Enable";
            this.checkBox1MinFileSizeMB.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.textBox1MinEpochCount);
            this.groupBox5.Controls.Add(this.checkBox1MinEpochCount);
            this.groupBox5.Location = new System.Drawing.Point(11, 393);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(533, 41);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Minimum Number of Epochs";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "Number:";
            // 
            // textBox1MinEpochCount
            // 
            this.textBox1MinEpochCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1MinEpochCount.Location = new System.Drawing.Point(77, 13);
            this.textBox1MinEpochCount.Name = "textBox1MinEpochCount";
            this.textBox1MinEpochCount.Size = new System.Drawing.Size(370, 21);
            this.textBox1MinEpochCount.TabIndex = 1;
            this.textBox1MinEpochCount.Text = "1000";
            // 
            // checkBox1MinEpochCount
            // 
            this.checkBox1MinEpochCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1MinEpochCount.AutoSize = true;
            this.checkBox1MinEpochCount.Location = new System.Drawing.Point(461, 15);
            this.checkBox1MinEpochCount.Name = "checkBox1MinEpochCount";
            this.checkBox1MinEpochCount.Size = new System.Drawing.Size(60, 16);
            this.checkBox1MinEpochCount.TabIndex = 2;
            this.checkBox1MinEpochCount.Text = "Enable";
            this.checkBox1MinEpochCount.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox_obsCodes);
            this.groupBox1.Controls.Add(this.checkBox_enableCode);
            this.groupBox1.Location = new System.Drawing.Point(6, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(540, 41);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Observation Codes Filter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Observation Codes:";
            // 
            // textBox_obsCodes
            // 
            this.textBox_obsCodes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_obsCodes.Location = new System.Drawing.Point(125, 14);
            this.textBox_obsCodes.Name = "textBox_obsCodes";
            this.textBox_obsCodes.Size = new System.Drawing.Size(331, 21);
            this.textBox_obsCodes.TabIndex = 1;
            this.textBox_obsCodes.Text = "P1,P2";
            // 
            // checkBox_enableCode
            // 
            this.checkBox_enableCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_enableCode.AutoSize = true;
            this.checkBox_enableCode.Location = new System.Drawing.Point(471, 17);
            this.checkBox_enableCode.Name = "checkBox_enableCode";
            this.checkBox_enableCode.Size = new System.Drawing.Size(66, 16);
            this.checkBox_enableCode.TabIndex = 2;
            this.checkBox_enableCode.Text = "Include";
            this.checkBox_enableCode.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.enabledFloatControl_mp2Factor);
            this.tabPage2.Controls.Add(this.enabledFloatControl_mp1Factor);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox8);
            this.tabPage2.Controls.Add(this.checkBox_IsNavCopy);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(556, 491);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Other Options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // enabledFloatControl_mp2Factor
            // 
            this.enabledFloatControl_mp2Factor.Location = new System.Drawing.Point(17, 178);
            this.enabledFloatControl_mp2Factor.Name = "enabledFloatControl_mp2Factor";
            this.enabledFloatControl_mp2Factor.Size = new System.Drawing.Size(426, 23);
            this.enabledFloatControl_mp2Factor.TabIndex = 6;
            this.enabledFloatControl_mp2Factor.Title = "Name:";
            this.enabledFloatControl_mp2Factor.Value = 0.1D;
            this.enabledFloatControl_mp2Factor.Visible = false;
            // 
            // enabledFloatControl_mp1Factor
            // 
            this.enabledFloatControl_mp1Factor.Location = new System.Drawing.Point(17, 149);
            this.enabledFloatControl_mp1Factor.Name = "enabledFloatControl_mp1Factor";
            this.enabledFloatControl_mp1Factor.Size = new System.Drawing.Size(426, 23);
            this.enabledFloatControl_mp1Factor.TabIndex = 6;
            this.enabledFloatControl_mp1Factor.Title = "Name:";
            this.enabledFloatControl_mp1Factor.Value = 0.1D;
            this.enabledFloatControl_mp1Factor.Visible = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label9);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.textBox2MinSatCountRatio);
            this.groupBox7.Controls.Add(this.label10);
            this.groupBox7.Controls.Add(this.textBox1MinSatCount);
            this.groupBox7.Controls.Add(this.checkBox1MinSatCount);
            this.groupBox7.Location = new System.Drawing.Point(6, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(439, 41);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Minimum Number of Satellites";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(150, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 12);
            this.label9.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(173, 17);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "Ratio:";
            // 
            // textBox2MinSatCountRatio
            // 
            this.textBox2MinSatCountRatio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2MinSatCountRatio.Location = new System.Drawing.Point(222, 14);
            this.textBox2MinSatCountRatio.Name = "textBox2MinSatCountRatio";
            this.textBox2MinSatCountRatio.Size = new System.Drawing.Size(94, 21);
            this.textBox2MinSatCountRatio.TabIndex = 1;
            this.textBox2MinSatCountRatio.Text = "0.5";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "Number: ";
            this.label10.Click += new System.EventHandler(this.Label10_Click);
            // 
            // textBox1MinSatCount
            // 
            this.textBox1MinSatCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1MinSatCount.Location = new System.Drawing.Point(66, 15);
            this.textBox1MinSatCount.Name = "textBox1MinSatCount";
            this.textBox1MinSatCount.Size = new System.Drawing.Size(93, 21);
            this.textBox1MinSatCount.TabIndex = 1;
            this.textBox1MinSatCount.Text = "4";
            // 
            // checkBox1MinSatCount
            // 
            this.checkBox1MinSatCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1MinSatCount.AutoSize = true;
            this.checkBox1MinSatCount.Location = new System.Drawing.Point(377, 15);
            this.checkBox1MinSatCount.Name = "checkBox1MinSatCount";
            this.checkBox1MinSatCount.Size = new System.Drawing.Size(60, 16);
            this.checkBox1MinSatCount.TabIndex = 2;
            this.checkBox1MinSatCount.Text = "Enable";
            this.checkBox1MinSatCount.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label13);
            this.groupBox8.Controls.Add(this.textBox1MinFrequencyCount);
            this.groupBox8.Controls.Add(this.checkBox1MinFrequencyCount);
            this.groupBox8.Location = new System.Drawing.Point(6, 50);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(437, 41);
            this.groupBox8.TabIndex = 5;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Minimum Number of Frequencies";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 17);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "Number: ";
            // 
            // textBox1MinFrequencyCount
            // 
            this.textBox1MinFrequencyCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1MinFrequencyCount.Location = new System.Drawing.Point(66, 16);
            this.textBox1MinFrequencyCount.Name = "textBox1MinFrequencyCount";
            this.textBox1MinFrequencyCount.Size = new System.Drawing.Size(243, 21);
            this.textBox1MinFrequencyCount.TabIndex = 1;
            this.textBox1MinFrequencyCount.Text = "2";
            // 
            // checkBox1MinFrequencyCount
            // 
            this.checkBox1MinFrequencyCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1MinFrequencyCount.AutoSize = true;
            this.checkBox1MinFrequencyCount.Location = new System.Drawing.Point(375, 18);
            this.checkBox1MinFrequencyCount.Name = "checkBox1MinFrequencyCount";
            this.checkBox1MinFrequencyCount.Size = new System.Drawing.Size(60, 16);
            this.checkBox1MinFrequencyCount.TabIndex = 2;
            this.checkBox1MinFrequencyCount.Text = "Enable";
            this.checkBox1MinFrequencyCount.UseVisualStyleBackColor = true;
            // 
            // checkBox_IsNavCopy
            // 
            this.checkBox_IsNavCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_IsNavCopy.AutoSize = true;
            this.checkBox_IsNavCopy.Location = new System.Drawing.Point(21, 115);
            this.checkBox_IsNavCopy.Name = "checkBox_IsNavCopy";
            this.checkBox_IsNavCopy.Size = new System.Drawing.Size(288, 16);
            this.checkBox_IsNavCopy.TabIndex = 2;
            this.checkBox_IsNavCopy.Text = "Navigation files are copied together if have";
            this.checkBox_IsNavCopy.UseVisualStyleBackColor = true;
            // 
            // button_reset
            // 
            this.button_reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_reset.Location = new System.Drawing.Point(492, 526);
            this.button_reset.Name = "button_reset";
            this.button_reset.Size = new System.Drawing.Size(75, 30);
            this.button_reset.TabIndex = 2;
            this.button_reset.Text = "Reset";
            this.button_reset.UseVisualStyleBackColor = true;
            this.button_reset.Click += new System.EventHandler(this.button_reset_Click);
            // 
            // ObsFileSelectOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 561);
            this.Controls.Add(this.button_reset);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_ok);
            this.Name = "ObsFileSelectOptionForm";
            this.ShowIcon = false;
            this.Text = "Selection Option";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBox_radiusFromCenter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_centerXyz;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_obsCodes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_enableRegionFilter;
        private System.Windows.Forms.CheckBox checkBox_enableCode;
        private System.Windows.Forms.TextBox textBox_excludeSites;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_includeSiteNames;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_enableExcludeSiteNames;
        private System.Windows.Forms.CheckBox checkBox_enableincludeSiteNames;
        private System.Windows.Forms.Button button_reset;
        private Gdp.Winform.Controls.TimePeriodControl timePeriodControl1;
        private System.Windows.Forms.CheckBox checkBox_enableTimePeriod;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1MinEpochCount;
        private System.Windows.Forms.CheckBox checkBox1MinEpochCount;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1MinFileSizeMB;
        private System.Windows.Forms.CheckBox checkBox1MinFileSizeMB;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox2MinSatCountRatio;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox1MinSatCount;
        private System.Windows.Forms.CheckBox checkBox1MinSatCount;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox1MinFrequencyCount;
        private System.Windows.Forms.CheckBox checkBox1MinFrequencyCount;
        private System.Windows.Forms.TabPage tabPage2;
        private Gdp.Winform.Controls.EnabledFloatControl enabledFloatControl_mp1Factor;
        private Gdp.Winform.Controls.EnabledFloatControl enabledFloatControl_mp2Factor;
        private System.Windows.Forms.Label label12;
        private Controls.MultiGnssSystemSelectControl multiGnssSystemSelectControl1;
        private System.Windows.Forms.CheckBox checkBox_enableSatelliteTypes;
        private System.Windows.Forms.CheckBox checkBox_IsNavCopy;
    }
}