using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Gdp.Winform;

namespace Gdp.Utils
{
    /// <summary>
    /// 窗口工具类
    /// </summary>
    public static class FormUtil
    { 

        #region 信息框
        /// <summary>
        /// 提示警告
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowWarningMessageBox(string msg)
        {
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// 错误提示窗口
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowErrorMessageBox(string msg)
        {
            MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } 
        /// <summary>
        /// OK提示
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowOkMessageBox(string msg = "操作成功！")
        {
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 提示是否打开指定的文件或文件夹。
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="path"></param>
        public static void ShowIfOpenDirMessageBox(string path, string msg = "操作完成！\r\n是否打开")
        {
            if (msg == "操作完成！\r\n是否打开") msg = "操作完成！\r\n是否打开 " + path + " ？";
            if (ShowYesNoMessageBox(msg) == DialogResult.Yes)
                FileUtil.OpenFileOrDirectory(path);  
        }

        /// <summary>
        /// 提示选择
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static DialogResult ShowYesNoMessageBox(string msg)
        {
            return MessageBox.Show(msg, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }
        #endregion

        #region 输入框
       
        /// <summary>
        /// 在第一条插入行,异常也不会报错！
        /// </summary>
        /// <param name="textBoxBase"></param>
        /// <param name="info"></param>
        /// <param name="maxAllowCount"></param>
        public static void InsertLineToTextBox(TextBoxBase textBoxBase, string info, int maxAllowCount = 5000)
        {
            try
            {
                if (textBoxBase == null || textBoxBase.IsDisposed)
                {
                    return;
                }
                textBoxBase.Invoke(new Action(delegate()
                {
                    var count = textBoxBase.Lines.Length;

                    if (count >= maxAllowCount)
                    {
                        List<string> lines = new List<string>(textBoxBase.Lines);
                        lines.RemoveAt(count - 1);
                        lines.Insert(0, info);
                        textBoxBase.Lines = lines.ToArray();
                    }
                    else
                    {
                        textBoxBase.Text = info + "\r\n" + textBoxBase.Text;
                    }

                }));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
        static DateTime lastNoticeTime = DateTime.Now;
        //最小显示时间间隔
        static int MinNoticeInterval = 1; 

        /// <summary>
        /// 显示通知，基于控件。此处具有时间控制，避免过于频繁。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="info"></param> 
        /// <param name="isControlFrequence"></param> 
        public static void ShowNotice(Control control, string info, bool isControlFrequence  =true)
        {
            var now = DateTime.Now;
            if (isControlFrequence && (now - lastNoticeTime).TotalSeconds < MinNoticeInterval)
            {
                return;
            }
            lastNoticeTime = now;
            SetText(control, info);
        }
        /// <summary>
        /// 直接设置文本，基于控件。不会报错。
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param> 
        public static void SetText(Control control, string text)
        {
            try
            {
                if (control.IsHandleCreated)
                {
                    control.Invoke(new Action(delegate()
                    {
                        control.Text = text;
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        } 
      
        #endregion
        

        #region 文件操作
      

        /// <summary>
        /// 弹出窗口，选择文件，返回文件路径，若无返回null。
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string ShowFormGetFilePath(string filter = "文本文件|*.txt|任何文件|*.*", string fileName = "")
        {
            OpenFileDialog d = new OpenFileDialog();
            d.FileName = fileName;
            d.Filter = filter;
            if (d.ShowDialog() == DialogResult.OK)
            {
                 return  (d.FileName);
            }

            return null;
        }
                
        /// <summary>
        /// 提示信息，并询问是否打开目录。如果输入的是文件，则打开所在目录。
        /// </summary>
        /// <param name="inDirPath"></param>
        /// <param name="msg"></param>
        public static void ShowOkAndOpenDirectory(string inDirPath, string msg = "执行完毕，是否打开所在目录？")
        {
            if (MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                == DialogResult.OK)
            {               
                FileUtil.OpenDirectory(inDirPath);
            }
        }
        /// <summary>
        /// 显示OK并打开文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="msg"></param>
        public static void ShowOkAndOpenFile(string filePath, string msg = "执行完毕，是否打开文件？")
        {
            if (MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                == DialogResult.OK)
            {
                FileUtil.OpenFile(filePath);
            }
        }
        /// <summary>
        /// 提示文件不存在。
        /// </summary>
        /// <param name="filePath"></param>
        public static void ShowFileNotExistBox(string filePath)
        {
            ShowErrorMessageBox("指定文件不存在！\r\n" + filePath);
        }
        #endregion



        /// <summary>
        /// 弹出一个文本输入框。
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static bool ShowInputForm(out string inputValue)
        {
            return ShowInputForm("请输入", out inputValue);
        }
        /// <summary>
        ///  显示输入数值的对话框，失败或默认返回0。
        /// </summary>
        /// <param name="formTitle"></param>
        /// <param name="inputValue"></param>
        /// <param name="initVal"></param>
        /// <returns></returns>
        public static bool ShowInputNumeralForm(string formTitle, out double inputValue, double initVal = 0)
        {
            string initValue = initVal + "";
            string inputValueS;
            if (ShowInputForm(formTitle, initValue, out inputValueS))
            {
                inputValue = Double.Parse(inputValueS);
                return true;
            }
            inputValue = 0;
            return false;
        }
        /// <summary>
        ///  显示输入对话框
        /// </summary>
        /// <param name="formTitle"></param>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static bool ShowInputForm(string formTitle, out string inputValue)
        {
            string initValue = "请在此输入";
            return ShowInputForm(formTitle, initValue, out inputValue);
        }
        /// <summary>
        /// 显示输入对话框
        /// </summary>
        /// <param name="formTitle"></param>
        /// <param name="initValue"></param>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static bool ShowInputForm(string formTitle, string initValue, out string inputValue)
        {
            List<string> canNotBeValues = new List<string>();
            return ShowInputForm(formTitle, initValue, canNotBeValues, out inputValue);
        }
        /// <summary>
        /// 显示输入对话框
        /// </summary>
        /// <param name="formTitle"></param>
        /// <param name="initValue"></param>
        /// <param name="canNotBeValues"></param>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static bool ShowInputForm(string formTitle, string initValue, List<string> canNotBeValues, out string inputValue)
        {
            return ShowInputForm(formTitle, initValue, canNotBeValues, "该名称已经存在，请换一个 ", out inputValue);
        }
        /// <summary>
        /// 显示输入对话框
        /// </summary>
        /// <param name="formTitle">窗口题目</param>
        /// <param name="initValue">初始值</param>
        /// <param name="canNotBeValues">不能为数据</param>
        /// <param name="canNotBeWarnMsg">不能为数据提示</param>
        /// <param name="inputValue">输入的数据</param>
        /// <returns></returns>
        public static bool ShowInputForm(string formTitle, string initValue, List<string> canNotBeValues, string canNotBeWarnMsg, out string inputValue)
        {
            Gdp.Utils.OneTextInputForm f = new Gdp.Utils.OneTextInputForm(initValue, canNotBeValues, canNotBeWarnMsg);
            f.Text = formTitle;
            if (f.ShowDialog() == DialogResult.OK)
            {
                inputValue = f.InputValue;
                return true;
            }
            inputValue = "";
            return false;
        }



        /// <summary>
        ///可能出错的执行。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="ignoreException"></param>
        public static  void TryExecute(Action action, bool ignoreException)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                if (!ignoreException) ShowErrorMessageBox("出错了：" + ex.Message);
            }
        }
         
        /// <summary>
        /// 等待线程并赋值
        /// </summary>
        /// <param name="textBox">显示的文本框</param>
        /// <param name="str">显示文本</param>
        /// <param name="isAppend">是否添加，否则直接赋值</param>
        public static void InvokeTextBoxSetText(TextBoxBase textBox, string str, bool isAppend = false)
        {
            try
            {
                textBox.Invoke(new Action(delegate()
                {
                    if (isAppend)
                        textBox.Text += str;
                    else textBox.Text = str;
                }));

            }
            catch (Exception ex) { }
        }  


        /// <summary>
        /// 检查文件存在性，如果不存在则提示，并返回false.
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool CheckExistOrShowWarningForm(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                Gdp.Utils.FormUtil.ShowWarningMessageBox("文件不存在，" + FilePath);
                return false;
            }
            return true;
        }
    }
}
