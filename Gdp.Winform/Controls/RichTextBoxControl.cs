//2017.11.03, czs, edit in hongqing, 增加日志记录追加，不同颜色表示

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gdp.Winform.Controls
{
    /// <summary>
    /// 具有右键菜单
    /// </summary>
    public partial class RichTextBoxControl : RichTextBox
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RichTextBoxControl()
        {
            InitializeComponent();
            this.ScrollBars = RichTextBoxScrollBars.Both;
            this.MaxAppendLineCount = 5000;
            this.smallerFont = new Font(this.Font.FontFamily, Font.Size-1, FontStyle.Italic);
            this.defaultFont = this.Font;
        }

        Font defaultFont = null;
        Font smallerFont = null;
        /// <summary>
        /// 实时追加的最大行数
        /// </summary>
        public int MaxAppendLineCount { get; set; }
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();//先获取焦点，防止点两下才运行
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Copy();
        }

        #region 日志记录、支持其他线程访问
        public delegate void LogAppendDelegate(Color color, string text, bool isSmaller=false);
        /// <summary> 
        /// 追加显示文本 
        /// </summary> 
        /// <param name="color">文本颜色</param> 
        /// <param name="text">显示文本</param> 
        public void LogAppend(Color color, string text, bool isSmaller=false)
        {
            if (!this.IsHandleCreated || this.IsDisposed) { return; }

            this.AppendText("\n");
            this.SelectionColor = color;
            if (isSmaller)
            {
                this.SelectionFont = smallerFont;
            }
            this.AppendText(text);
            var lines = this.Lines;
            //if (!  Gdp.Utils.ArrayUtil.CheckAndResizeArrayTo( ref lines, MaxAppendLineCount))
            //{
            //    this.Lines = lines;
            //}

            //ScrollToEnd();
            int lineCount = this.Lines.Length;
            if( Math.Abs( lineCount - LastScrollLineCount) > 20)
            {
                LastScrollLineCount = lineCount;
                ScrollToEnd();
            }
        }
        int LastScrollLineCount = 0;

        public void ScrollToStart()
        {
            this.SelectionStart = 0;
            this.ScrollToCaret();
        }
        public void ScrollToEnd()
        {
            this.SelectionStart = this.TextLength;
            this.ScrollToCaret();
        }
        /// <summary> 
        /// 显示错误日志 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogError(string text)
        {
            if (this.IsDisposed || !this.IsHandleCreated) { return; }

            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            this.Invoke(la, Color.Red, text,false);
        }
        /// <summary> 
        /// 显示警告信息 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogWarning(string text)
        {
            if (this.IsDisposed || !this.IsHandleCreated) { return; }

            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            this.Invoke(la, Color.FromArgb(255, 128, 0), text, false);
        }
        /// <summary> 
        /// 显示警告信息 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogGreen(string text)
        {
            if (this.IsDisposed || !this.IsHandleCreated) { return; }

            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            this.Invoke(la, Color.Green, text, false);
        }
        /// <summary> 
        /// 显示信息 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogMessage(string text)
        {
            if (this.IsDisposed || !this.IsHandleCreated) { return; }

            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            if(this.IsHandleCreated){ this.Invoke(la, Color.Black, text, false); }
        }
        public void LogSmaller(string text)
        {
            if (this.IsDisposed || !this.IsHandleCreated) { return; }

            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            this.Invoke(la, Color.Black, text, true);
        }
        #endregion

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Paste();
        }
        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Cut();
        }
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.SelectedText = "";
        }
        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.SelectAll();
        }
        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Undo();
        }
        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Redo();
        }

        void 全选并复制toolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.SelectAll();
            rtb.Copy();
        }
        void 全选并剪切toolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.SelectAll();
            rtb.Cut();
        }
        void 全选并粘贴toolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.SelectAll();
            rtb.Paste(); 
        }
        private void RichTextBoxControl_MouseUp(object sender, MouseEventArgs e)//控制右键菜单的显示
        {
            if (e.Button == MouseButtons.Right)
            {
                //if (this.CanRedo)//redo
                //    redo.Enabled = true;
                //else
                //    redo.Enabled = false;
                //if (this.CanUndo)//undo
                //    undo.Enabled = true;
                //else
                //    undo.Enabled = false;
                //if (this.SelectionLength > 0)
                //{
                //    copy.Enabled = true;
                //    clip.Enabled = true;
                //}
                //else
                //{
                //    copy.Enabled = false;
                //    clip.Enabled = false;
                //}
                //if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
                //    paste.Enabled = true;
                //else
                //    paste.Enabled = false;
                //contextMenuStrip1.Show(this, new Point(e.X, e.Y));
            }
        }

        private void RichTextBoxControl_LinkClicked(object sender, LinkClickedEventArgs e)
        {  // Call Process.Start method to open a browser, with link text as URL  
            System.Diagnostics.Process.Start(e.LinkText); // call default browser  
        }

    }
}
