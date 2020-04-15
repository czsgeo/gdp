namespace Gdp.Winform.Controls
{
    partial class MultiGnssSystemSelectControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBox_gps = new System.Windows.Forms.CheckBox();
            this.checkBox_galileo = new System.Windows.Forms.CheckBox();
            this.checkBox_beidou = new System.Windows.Forms.CheckBox();
            this.checkBox_glonass = new System.Windows.Forms.CheckBox();
            this.checkBox_NAVIC = new System.Windows.Forms.CheckBox();
            this.checkBox_SBAS = new System.Windows.Forms.CheckBox();
            this.checkBox_qzss = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.flowLayoutPanel1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(109, 158);
            this.groupBox2.TabIndex = 40;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Enabled Systems";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.checkBox_gps);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_galileo);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_beidou);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_glonass);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_NAVIC);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_SBAS);
            this.flowLayoutPanel1.Controls.Add(this.checkBox_qzss);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 16);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(105, 140);
            this.flowLayoutPanel1.TabIndex = 39;
            // 
            // checkBox_gps
            // 
            this.checkBox_gps.AutoSize = true;
            this.checkBox_gps.Checked = true;
            this.checkBox_gps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_gps.Location = new System.Drawing.Point(2, 2);
            this.checkBox_gps.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_gps.Name = "checkBox_gps";
            this.checkBox_gps.Size = new System.Drawing.Size(42, 16);
            this.checkBox_gps.TabIndex = 38;
            this.checkBox_gps.Text = "GPS";
            this.checkBox_gps.UseVisualStyleBackColor = true;
            // 
            // checkBox_galileo
            // 
            this.checkBox_galileo.AutoSize = true;
            this.checkBox_galileo.Location = new System.Drawing.Point(2, 22);
            this.checkBox_galileo.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_galileo.Name = "checkBox_galileo";
            this.checkBox_galileo.Size = new System.Drawing.Size(66, 16);
            this.checkBox_galileo.TabIndex = 39;
            this.checkBox_galileo.Text = "Galileo";
            this.checkBox_galileo.UseVisualStyleBackColor = true;
            // 
            // checkBox_beidou
            // 
            this.checkBox_beidou.AutoSize = true;
            this.checkBox_beidou.Location = new System.Drawing.Point(2, 42);
            this.checkBox_beidou.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_beidou.Name = "checkBox_beidou";
            this.checkBox_beidou.Size = new System.Drawing.Size(60, 16);
            this.checkBox_beidou.TabIndex = 37;
            this.checkBox_beidou.Text = "BeiDou";
            this.checkBox_beidou.UseVisualStyleBackColor = true;
            // 
            // checkBox_glonass
            // 
            this.checkBox_glonass.AutoSize = true;
            this.checkBox_glonass.Location = new System.Drawing.Point(2, 62);
            this.checkBox_glonass.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_glonass.Name = "checkBox_glonass";
            this.checkBox_glonass.Size = new System.Drawing.Size(66, 16);
            this.checkBox_glonass.TabIndex = 40;
            this.checkBox_glonass.Text = "GLONASS";
            this.checkBox_glonass.UseVisualStyleBackColor = true;
            // 
            // checkBox_NAVIC
            // 
            this.checkBox_NAVIC.AutoSize = true;
            this.checkBox_NAVIC.Location = new System.Drawing.Point(2, 82);
            this.checkBox_NAVIC.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_NAVIC.Name = "checkBox_NAVIC";
            this.checkBox_NAVIC.Size = new System.Drawing.Size(54, 16);
            this.checkBox_NAVIC.TabIndex = 41;
            this.checkBox_NAVIC.Text = "IRNSS";
            this.checkBox_NAVIC.UseVisualStyleBackColor = true;
            // 
            // checkBox_SBAS
            // 
            this.checkBox_SBAS.AutoSize = true;
            this.checkBox_SBAS.Location = new System.Drawing.Point(2, 102);
            this.checkBox_SBAS.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_SBAS.Name = "checkBox_SBAS";
            this.checkBox_SBAS.Size = new System.Drawing.Size(48, 16);
            this.checkBox_SBAS.TabIndex = 42;
            this.checkBox_SBAS.Text = "SBAS";
            this.checkBox_SBAS.UseVisualStyleBackColor = true;
            // 
            // checkBox_qzss
            // 
            this.checkBox_qzss.AutoSize = true;
            this.checkBox_qzss.Location = new System.Drawing.Point(54, 102);
            this.checkBox_qzss.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_qzss.Name = "checkBox_qzss";
            this.checkBox_qzss.Size = new System.Drawing.Size(48, 16);
            this.checkBox_qzss.TabIndex = 43;
            this.checkBox_qzss.Text = "QZSS";
            this.checkBox_qzss.UseVisualStyleBackColor = true;
            // 
            // MultiGnssSystemSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MultiGnssSystemSelectControl";
            this.Size = new System.Drawing.Size(109, 158);
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_beidou;
        private System.Windows.Forms.CheckBox checkBox_gps;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBox_galileo;
        private System.Windows.Forms.CheckBox checkBox_glonass;
        private System.Windows.Forms.CheckBox checkBox_NAVIC;
        private System.Windows.Forms.CheckBox checkBox_SBAS;
        private System.Windows.Forms.CheckBox checkBox_qzss;
    }
}
