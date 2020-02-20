//2014.05.20, czs, created, 日志处理基类
//2015.12.18, czs, edit in hongqing, 增加日志监听,去除log4net依赖，自写LogWriter
//2016.01.22, cy, add locker 解决多线程写入问题

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Text;


namespace Gdp.IO
{
    public delegate void LogMessageProducedEventHandler(string msg);
    /// <summary>
    /// 产生了日志信息
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="LogType"></param>
    public delegate void TypedLogMessageProducedEventHandler(string msg, LogType LogType, Type msgProducer);

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        Error,
        Info,
        Fatal,
        Debug,
        Warn
    }
    /// <summary>
    /// 日志写入器，采用单例模式
    /// </summary>
    public class LogWriter : IDisposable
    {
        /// <summary>
        /// 信息产生
        /// </summary>
        public event TypedLogMessageProducedEventHandler MsgProduced;
        private LogWriter()
        {
            try
            {
                IsWriteToConsole = true;
                BufferSize = 10000;
                IsRealTimeWriting = true;
                WritingIntervalInMiniSecond = 1000;
                Gdp.Utils.FileUtil.CheckOrCreateDirectory(LogDirectory);

                CheckOrTryMoveLogFilesAndInitWriter();
            }
            catch (Exception ex)
            {
                Console.WriteLine("日志写入器初始化出错！, " + ex.Message);
            }
        }
        /// <summary>
        ///字体缓存大小
        /// </summary>
        public int BufferSize { get; set; }
        /// <summary>
        /// 最大输出间隔
        /// </summary>
        public int WritingIntervalInMiniSecond { get; set; }
        /// <summary>
        /// 是否实时输出
        /// </summary>
        public bool IsRealTimeWriting { get; set; }

        private string CheckOrTryMoveLogFiles()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sys.log");
            CheckOrTryMoveFile(path);
            return path;
        }

        private void CheckOrTryMoveFile(string path)
        {

            try
            {
                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    if (info.LastWriteTime.Date != DateTime.Now.Date || info.Length >= 2 * 1024 * 1024)
                    {
                        var bakePath = Path.Combine(LogDirectory, "sys_" + info.CreationTime.ToString("yyyy-MM-dd") + ".log");

                        bakePath = Gdp.Utils.PathUtil.GetAvailableName(bakePath);

                        File.Move(path, bakePath);
                        Console.WriteLine("日志文件 " + path + " 移动到 " + bakePath);
                    }
                }
            }
            catch (Exception ex)
            { Console.Error.WriteLine("移动日志文件出错！" + path + ", " + ex.Message); }
        }

        #region 单例模式实现
        private static LogWriter writer = new LogWriter();
        /// <summary>
        /// 获取实例
        /// </summary>
        public static LogWriter Instance { get { return writer; } }
        #endregion

        /// <summary>
        /// 备份目录
        /// </summary>
        public string LogDirectory { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log"); } }

        /// <summary>
        /// 是否输出到控制台
        /// </summary>
        public bool IsWriteToConsole { get; set; }

        /// <summary>
        /// 文本写
        /// </summary>
        public TextWriter TextWriter { get; set; }
        /// <summary>
        /// 写入数量计数器
        /// </summary>
        long LoopLineCounter = 0;
        static object locker = new object();
        StringBuilder buffer = new System.Text.StringBuilder();
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logType"></param>
        /// <param name="msgProducer"></param>
        public void Write(string msg, LogType logType = LogType.Info, Type msgProducer = null)
        {
            if (MsgProduced != null) { MsgProduced(msg, logType, msgProducer); }
            // var thread = Thread.CurrentThread;
            string info = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + logType + "\t" + msg + "\t" + msgProducer.Name;


            if (IsRealTimeWriting)
            {
                WriteLine(info);
                LastWritingTime = DateTime.Now;
            }
            else
            {
                buffer.AppendLine(info);

                if (buffer.Length >= BufferSize)
                {
                    WriteBuffer();
                }
                if (DateTime.Now - LastWritingTime > TimeSpan.FromMilliseconds(WritingIntervalInMiniSecond))
                {
                    WriteBuffer();
                }
            }
            //  WriteLine(info);
        }

        private void WriteBuffer()
        {
            lock (locker)
            {
                TextWriter.WriteLine(buffer.ToString());
                LoopLineCounter++;
            }
            buffer.Clear();
            LastWritingTime = DateTime.Now;
        }

        DateTime LastWritingTime;

        private void WriteLine(string info)
        {

            if (IsWriteToConsole)
            {
                Console.WriteLine(info);
            }

            lock (locker)
            {
                if (TextWriter == null)
                {
                    return;
                }
                TextWriter.WriteLine(info);

                LoopLineCounter++;

                if (IsRealTimeWriting)
                {
                    TextWriter.Flush();
                }

                if (LoopLineCounter > 1000)
                {
                    LoopLineCounter = 0;
                    TextWriter.Flush();
                    TextWriter.Close();

                    CheckOrTryMoveLogFilesAndInitWriter();
                }
            }
        }

        private void CheckOrTryMoveLogFilesAndInitWriter()
        {
            var path = CheckOrTryMoveLogFiles();

            this.TextWriter = new StreamWriter(path, true);
        }


        /// <summary>
        /// 清除过期的日志,返回删除的日志名称。
        /// </summary>
        public string TryClearOutDateLogs()
        {
            try
            {
                var files = Directory.GetFiles(LogDirectory);

                StringBuilder sb = new StringBuilder();
                foreach (var item in files)
                {
                    var name = Path.GetFileName(item);
                    if (name.ToLower().Contains("log"))
                    {
                        sb.AppendLine(name);
                        System.IO.File.Delete(item);
                    }
                }
                var msg = sb.ToString();
                if (String.IsNullOrEmpty(msg))
                {
                    return "没有删除任何日志";
                }
                return "已删除：\r\n" + msg;
            }
            catch (Exception ex)
            {
                string msg = "清除过期日志失败！" + ex.Message;
                Write(msg, LogType.Error);
                return msg;
            }
        }


        public void Dispose()
        {
            if (TextWriter != null)
            {
                TextWriter.Flush();
                TextWriter.Close();
            }
        }
    }
}