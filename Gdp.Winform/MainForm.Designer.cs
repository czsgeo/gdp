namespace Gdp.Winform
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.button_convert = new System.Windows.Forms.Button();
            this.button_select = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_view = new System.Windows.Forms.Button();
            this.button_download = new System.Windows.Forms.Button();
            this.linkLabel_gnsserSite = new System.Windows.Forms.LinkLabel();
            this.linkLabel_163mail = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_extractSiteInfo = new System.Windows.Forms.Button();
            this.button_tableView = new System.Windows.Forms.Button();
            this.button_openLog = new System.Windows.Forms.Button();
            this.button_testobs = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_convert
            // 
            this.button_convert.BackColor = System.Drawing.Color.SkyBlue;
            this.button_convert.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_convert.Location = new System.Drawing.Point(206, 89);
            this.button_convert.Name = "button_convert";
            this.button_convert.Size = new System.Drawing.Size(116, 82);
            this.button_convert.TabIndex = 1;
            this.button_convert.Text = "Formator";
            this.button_convert.UseVisualStyleBackColor = false;
            this.button_convert.Click += new System.EventHandler(this.Button_convert_Click);
            // 
            // button_select
            // 
            this.button_select.BackColor = System.Drawing.Color.SkyBlue;
            this.button_select.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_select.Location = new System.Drawing.Point(339, 89);
            this.button_select.Name = "button_select";
            this.button_select.Size = new System.Drawing.Size(116, 82);
            this.button_select.TabIndex = 2;
            this.button_select.Text = "Selector";
            this.button_select.UseVisualStyleBackColor = false;
            this.button_select.Click += new System.EventHandler(this.Button_select_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(22, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(395, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Welcome to use GDP - GNSS Data Preprocessor";
            // 
            // button_view
            // 
            this.button_view.BackColor = System.Drawing.Color.SkyBlue;
            this.button_view.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_view.Location = new System.Drawing.Point(469, 89);
            this.button_view.Name = "button_view";
            this.button_view.Size = new System.Drawing.Size(116, 82);
            this.button_view.TabIndex = 3;
            this.button_view.Text = "Viewer && Editor && Analyzer";
            this.button_view.UseVisualStyleBackColor = false;
            this.button_view.Click += new System.EventHandler(this.Button_view_Click);
            // 
            // button_download
            // 
            this.button_download.BackColor = System.Drawing.Color.SkyBlue;
            this.button_download.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_download.Location = new System.Drawing.Point(76, 89);
            this.button_download.Name = "button_download";
            this.button_download.Size = new System.Drawing.Size(116, 82);
            this.button_download.TabIndex = 0;
            this.button_download.Text = "Downloader";
            this.button_download.UseVisualStyleBackColor = false;
            this.button_download.Click += new System.EventHandler(this.Button_download_Click);
            // 
            // linkLabel_gnsserSite
            // 
            this.linkLabel_gnsserSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel_gnsserSite.AutoSize = true;
            this.linkLabel_gnsserSite.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel_gnsserSite.Font = new System.Drawing.Font("宋体", 12F);
            this.linkLabel_gnsserSite.ForeColor = System.Drawing.SystemColors.ControlText;
            this.linkLabel_gnsserSite.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabel_gnsserSite.Location = new System.Drawing.Point(108, -1);
            this.linkLabel_gnsserSite.Name = "linkLabel_gnsserSite";
            this.linkLabel_gnsserSite.Size = new System.Drawing.Size(120, 16);
            this.linkLabel_gnsserSite.TabIndex = 9;
            this.linkLabel_gnsserSite.TabStop = true;
            this.linkLabel_gnsserSite.Text = "www.gnsser.com";
            this.linkLabel_gnsserSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_gnsserSite_LinkClicked);
            // 
            // linkLabel_163mail
            // 
            this.linkLabel_163mail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel_163mail.AutoSize = true;
            this.linkLabel_163mail.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel_163mail.Font = new System.Drawing.Font("宋体", 12F);
            this.linkLabel_163mail.ForeColor = System.Drawing.SystemColors.ControlText;
            this.linkLabel_163mail.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabel_163mail.Location = new System.Drawing.Point(558, -1);
            this.linkLabel_163mail.Name = "linkLabel_163mail";
            this.linkLabel_163mail.Size = new System.Drawing.Size(120, 16);
            this.linkLabel_163mail.TabIndex = 11;
            this.linkLabel_163mail.TabStop = true;
            this.linkLabel_163mail.Text = "gnsser@163.com";
            this.linkLabel_163mail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_163mail_LinkClicked);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 10F);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 14);
            this.label2.TabIndex = 8;
            this.label2.Text = "Available at:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("宋体", 10F);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(440, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 14);
            this.label3.TabIndex = 10;
            this.label3.Text = "Email address:";
            // 
            // button_extractSiteInfo
            // 
            this.button_extractSiteInfo.BackColor = System.Drawing.Color.SkyBlue;
            this.button_extractSiteInfo.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_extractSiteInfo.Location = new System.Drawing.Point(72, 198);
            this.button_extractSiteInfo.Name = "button_extractSiteInfo";
            this.button_extractSiteInfo.Size = new System.Drawing.Size(116, 82);
            this.button_extractSiteInfo.TabIndex = 4;
            this.button_extractSiteInfo.Text = "Extractor";
            this.button_extractSiteInfo.UseVisualStyleBackColor = false;
            this.button_extractSiteInfo.Click += new System.EventHandler(this.button_extractSiteInfo_Click);
            // 
            // button_tableView
            // 
            this.button_tableView.BackColor = System.Drawing.Color.SkyBlue;
            this.button_tableView.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_tableView.Location = new System.Drawing.Point(206, 198);
            this.button_tableView.Name = "button_tableView";
            this.button_tableView.Size = new System.Drawing.Size(116, 82);
            this.button_tableView.TabIndex = 5;
            this.button_tableView.Text = "Table Viewer";
            this.button_tableView.UseVisualStyleBackColor = false;
            this.button_tableView.Click += new System.EventHandler(this.button_tableView_Click);
            // 
            // button_openLog
            // 
            this.button_openLog.BackColor = System.Drawing.Color.SkyBlue;
            this.button_openLog.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_openLog.Location = new System.Drawing.Point(339, 198);
            this.button_openLog.Name = "button_openLog";
            this.button_openLog.Size = new System.Drawing.Size(116, 82);
            this.button_openLog.TabIndex = 6;
            this.button_openLog.Text = "Log";
            this.button_openLog.UseVisualStyleBackColor = false;
            this.button_openLog.Click += new System.EventHandler(this.button_openLog_Click);
            // 
            // button_testobs
            // 
            this.button_testobs.BackColor = System.Drawing.Color.SkyBlue;
            this.button_testobs.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_testobs.Location = new System.Drawing.Point(469, 198);
            this.button_testobs.Name = "button_testobs";
            this.button_testobs.Size = new System.Drawing.Size(116, 82);
            this.button_testobs.TabIndex = 6;
            this.button_testobs.Text = "Test";
            this.button_testobs.UseVisualStyleBackColor = false;
            this.button_testobs.Click += new System.EventHandler(this.button_testobs_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.linkLabel_163mail);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.linkLabel_gnsserSite);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 447);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(688, 16);
            this.panel1.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Gdp.Winform.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(688, 463);
            this.Controls.Add(this.button_testobs);
            this.Controls.Add(this.button_openLog);
            this.Controls.Add(this.button_tableView);
            this.Controls.Add(this.button_extractSiteInfo);
            this.Controls.Add(this.button_download);
            this.Controls.Add(this.button_view);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_select);
            this.Controls.Add(this.button_convert);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GDP - GNSS Data Preprocessor v1.0";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_convert;
        private System.Windows.Forms.Button button_select;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_view;
        private System.Windows.Forms.Button button_download;
        private System.Windows.Forms.LinkLabel linkLabel_gnsserSite;
        private System.Windows.Forms.LinkLabel linkLabel_163mail;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_extractSiteInfo;
        private System.Windows.Forms.Button button_tableView;
        private System.Windows.Forms.Button button_openLog;
        private System.Windows.Forms.Button button_testobs;
        private System.Windows.Forms.Panel panel1;
    }
}