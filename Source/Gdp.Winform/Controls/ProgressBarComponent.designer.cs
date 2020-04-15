namespace Gdp.Winform.Controls
{
	partial class ProgressBarComponent
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label_progressInfo = new System.Windows.Forms.Label();
            this.label_classfyProcessInfo = new System.Windows.Forms.Label();
            this.label_progressPersent = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_classfiyInfo = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(0, 15);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(468, 19);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 0;
            // 
            // label_progressInfo
            // 
            this.label_progressInfo.AutoSize = true;
            this.label_progressInfo.BackColor = System.Drawing.Color.Transparent;
            this.label_progressInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_progressInfo.Location = new System.Drawing.Point(375, 0);
            this.label_progressInfo.Name = "label_progressInfo";
            this.label_progressInfo.Size = new System.Drawing.Size(53, 12);
            this.label_progressInfo.TabIndex = 1;
            this.label_progressInfo.Text = "Progress";
            // 
            // label_classfyProcessInfo
            // 
            this.label_classfyProcessInfo.AutoSize = true;
            this.label_classfyProcessInfo.BackColor = System.Drawing.Color.Transparent;
            this.label_classfyProcessInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_classfyProcessInfo.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_classfyProcessInfo.Location = new System.Drawing.Point(0, 0);
            this.label_classfyProcessInfo.Name = "label_classfyProcessInfo";
            this.label_classfyProcessInfo.Size = new System.Drawing.Size(61, 12);
            this.label_classfyProcessInfo.TabIndex = 2;
            this.label_classfyProcessInfo.Text = "Progress";
            // 
            // label_progressPersent
            // 
            this.label_progressPersent.AutoSize = true;
            this.label_progressPersent.BackColor = System.Drawing.Color.Transparent;
            this.label_progressPersent.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_progressPersent.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_progressPersent.Location = new System.Drawing.Point(428, 0);
            this.label_progressPersent.Name = "label_progressPersent";
            this.label_progressPersent.Size = new System.Drawing.Size(40, 12);
            this.label_progressPersent.TabIndex = 3;
            this.label_progressPersent.Text = "0/100";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_classfiyInfo);
            this.panel1.Controls.Add(this.label_progressInfo);
            this.panel1.Controls.Add(this.label_classfyProcessInfo);
            this.panel1.Controls.Add(this.label_progressPersent);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(468, 15);
            this.panel1.TabIndex = 4;
            // 
            // label_classfiyInfo
            // 
            this.label_classfiyInfo.AutoSize = true;
            this.label_classfiyInfo.BackColor = System.Drawing.Color.Transparent;
            this.label_classfiyInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_classfiyInfo.Location = new System.Drawing.Point(61, 0);
            this.label_classfiyInfo.Name = "label_classfiyInfo";
            this.label_classfiyInfo.Size = new System.Drawing.Size(29, 12);
            this.label_classfiyInfo.TabIndex = 4;
            this.label_classfiyInfo.Text = "Info";
            // 
            // ProgressBarComponent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel1);
            this.Name = "ProgressBarComponent";
            this.Size = new System.Drawing.Size(468, 34);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label_progressInfo;
        private System.Windows.Forms.Label label_classfyProcessInfo;
        private System.Windows.Forms.Label label_progressPersent;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_classfiyInfo;
	}
}
