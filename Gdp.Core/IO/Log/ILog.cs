//2015.12.18, czs, create in hongqing, 建立日志接口，去除log4net依赖

using System;

namespace Gdp.IO
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="info"></param>
        void Debug(string info);
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exception"></param>
        void Debug(string info, Exception exception);
        /// <summary>
        /// 出错信息
        /// </summary>
        /// <param name="info"></param>
        void Error(string info);
        /// <summary>
        /// 出错信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exception"></param>
        void Error(string info, Exception exception);
        /// <summary>
        /// 致命的重要性！必须显示。
        /// </summary>
        /// <param name="info"></param>
        void Fatal(string info);
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="info"></param>
        void Info(string info);
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="info"></param>
        void Warn(string info);
    }
}
