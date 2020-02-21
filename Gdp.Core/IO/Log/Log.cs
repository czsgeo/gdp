//2014.05.20, czs, created, 日志处理基类
//2015.12.18, czs, edit in hongqing, 增加日志监听,去除log4net依赖，自写LogWriter

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection; 


namespace Gdp.IO
{
    
    /// <summary>
    /// 日志写入类。当前采用log4net实现。
    /// 使用方法：每个类可以实例化一个对象。
    /// (MethodBase.GetCurrentMethod().DeclaringType);          
    /// </summary>
    public class Log : Gdp.IO.ILog
    {
        LogWriter writer = LogWriter.Instance;

        /// <summary>
        /// 信息发生者
        /// </summary>
        public Type MsgProducer { get; set; }

        /// <summary>
        /// 以对象创建一个日志记录器。
        /// </summary>
        /// <param name="obj"></param>
        public Log(object obj) : this(obj.GetType()) { }
        /// <summary>
        /// 以类型创建一个日志记录器。
        /// </summary>
        public Log(Type classType)
        {
            this.MsgProducer = classType;
        }

        /// <summary>
        /// 获取日志实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILog GetLog(Type type)
        {
            return new Log(type);
        }
        /// <summary>
        /// 获取日志实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILog GetLog(object type)
        {
            return new Log(type);
        }

        #region 记录日志
        /// <summary>
        /// 调试中出现的警告问题。
        /// </summary>
        /// <param name="info"></param>
        public void Warn(string info)
        {
            if (Setting.IsShowWarning && Setting.IsShowFatal)
            {
                writer.Write(info, LogType.Warn, MsgProducer);
            }
        }
        /// <summary>
        /// 调试中出现的致命问题。
        /// </summary>
        /// <param name="info"></param>
        public void Fatal(string info)
        {
            writer.Write(info, LogType.Fatal, MsgProducer); 
        }
        /// <summary>
        /// 调试中出现的问题。
        /// </summary>
        /// <param name="info"></param>
        public void Debug(string info)
        {
            if (Setting.IsShowDebug && Setting.IsShowFatal)
            {
                writer.Write(info, LogType.Debug, MsgProducer);
            }
        }
        /// <summary>
        /// 调试中出现的问题
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exception"></param>
        public void Debug(string info, Exception exception)
        {
            if (Setting.IsShowDebug && Setting.IsShowFatal)
            {
                writer.Write(info, LogType.Debug, MsgProducer);
            }
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="info"></param>
        public void Error(string info)
        {
            if (Setting.IsShowFatal && Setting.IsShowError)
            writer.Write(info, LogType.Error, MsgProducer); 
        }
        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exception"></param>
        public void Error(string info, Exception exception)
        { 
            if (Setting.IsShowFatal && Setting.IsShowError)
            writer.Write(info, LogType.Error, MsgProducer); 
        }
        /// <summary>
        /// 显示一半的信息，有必要让用户知道
        /// </summary>
        /// <param name="info"></param>
        public void Info(string info)
        {
            if (Setting.IsShowInfo && Setting.IsShowFatal)
            writer.Write(info, LogType.Info, MsgProducer); 
        }
        #endregion
        
    }
}
