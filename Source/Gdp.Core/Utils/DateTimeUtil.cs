//2014.12.03，czs, edit in jinxingliangmao shangliao, 增减注释，格式修改为24小时制

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp.Utils
{
    /// <summary>
    /// 时间处理工具类
    /// </summary>
    public class DateTimeUtil
    {
        /// <summary>
        /// 以时间为路径字符串 yyyy.MM.dd.HH.mm.ss
        /// </summary>
        /// <returns></returns>
        public static string GetDateTimePathStringNow()
        {
            return GetDateTimePathString(DateTime.Now);
        }
        /// <summary>
        /// 以时间为路径字符串 yyyy.MM.dd.HH.mm.ss
        /// </summary>
        /// <param name="dateTime">系统时间对象</param>
        /// <returns></returns>
        public static string GetDateTimePathString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy.MM.dd.HH.mm.ss");
        }
        /// <summary>
        /// 获取当前时间的字符串 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public static string GetFormatedDateTimeNow()
        {
            return GetFormatedDateTime(DateTime.Now);
        }
        /// <summary>
        /// 获取当前时间的字符串 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public static string GetFormatedDateTimeNowUtc()
        {
            return GetFormatedDateTime(DateTime.UtcNow);
        }
        /// <summary>
        /// 获取当前时间的字符串 HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public static string GetFormatedTimeNow(bool showMilliSecond = false)
        {
            return GetFormatedTime(DateTime.Now, showMilliSecond);
        }
        /// <summary>
        /// 获取时间的格式化字符串 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dateTime">系统时间对象</param>
        /// <returns></returns>
        public static string GetFormatedDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 获取时间的格式化字符串 HH:mm:ss
        /// </summary>
        /// <param name="dateTime">系统时间对象</param>
        /// <param name="showMilliSecond">是否显示毫秒</param>
        /// <returns></returns>
        public static string GetFormatedTime(DateTime dateTime, bool showMilliSecond = false)
        {
            if (showMilliSecond)
            {
                return dateTime.ToString("HH:mm:ss.fff");
            }

            return dateTime.ToString("HH:mm:ss");
        }
        /// <summary>
        /// 若解析不成功，则采用DateTime.MinValue
        /// </summary>
        /// <param name="timeString"></param>
        /// <returns></returns>
        public static DateTime TryParse(string timeString)
        {
            DateTime time;
            if (!DateTime.TryParse(timeString, out time)) time = DateTime.MinValue;
            return time;
        } 

        /// <summary>
        /// 尽力解析时间
        /// </summary>
        /// <param name="timeString">时间字符串</param>
        /// <returns></returns>
        public static DateTime TryParse(object timeString)
        {
            return TryParse(timeString+"");      
        }

        /// <summary>
        /// 判断时间间隔大小，返回合适的时间字符串，如 5.00 秒， 8.98分。
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static string GetFloatString(TimeSpan span)
        {
            if (span.TotalDays > 1) return span.TotalDays.ToString("0.000") + " 天";
            else if (span.TotalHours > 1) return span.TotalHours.ToString("0.000") + " 小时";
            else if (span.TotalMinutes > 1) return span.TotalMinutes.ToString("0.000") + " 分钟";
            else if (span.TotalSeconds > 1) return span.TotalSeconds.ToString("0.000") + " 秒";
            else if (span.TotalMilliseconds > 1) return span.TotalMilliseconds.ToString("0.000") + " 毫秒";

            return span.ToString();
        }

        /// <summary>
        /// 获取当前时间到毫秒，通常用于日志记录
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStringWithMiniSecondNow()
        {
            var time  =  DateTime.Now;
            return GetTimeStringWithMiniSecond(time);
        }
        /// <summary>
        /// 获取当前时间到毫秒，通常用于日志记录
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetTimeStringWithMiniSecond(DateTime time)
        {
            return time.ToString("HH:mm:ss.fff");
        }
    }
}
