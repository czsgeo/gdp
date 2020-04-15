//2018.04.16, czs, edit in hmx, 将99年，改为1999.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gdp.Utils; 


namespace Gdp
{
    /// <summary>
    /// 时间常用常数
    /// </summary>
    public static class TimeIoUtil
    {
        /// <summary>
        /// 2013 10 27 00 00 00
        /// </summary>
        /// <param name="gpsTime"></param>
        public static string ToRinexV3String(Time gpsTime){
            //2013 10 27 00 00 00

            StringBuilder sb = new StringBuilder();
            sb.Append(gpsTime.Year);
            sb.Append(" ");
            sb.Append(gpsTime.Month.ToString("00"));
            sb.Append(" ");
            sb.Append(gpsTime.Day.ToString("00"));
            sb.Append(" ");
            sb.Append(gpsTime.Hour.ToString("00"));
            sb.Append(" ");
            sb.Append(gpsTime.Minute.ToString("00"));
            sb.Append(" ");
            sb.Append(gpsTime.Second.ToString("00"));
            return sb.ToString();
        }

        /// <summary>
        /// 解析以空格('-',':')分开的“年 月 日 时 分 秒”字符串，如 06 10 30  0  0  0.0000000 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Time Parse(string line)
        {
            //if (line.Trim().Length == 0)
            //    return new Time();
            try
            {
                string[] timeParams = line.Split(new char[] { ' ', '-', ':', '/' }, StringSplitOptions.RemoveEmptyEntries);
                return Parse(timeParams);
            }
            catch (Exception ex)
            {
                throw new Exception("时间解析错误：" + line + "，"+ ex.Message);
            }

        }

        public static Time Parse(string[] timeParams)
        {
            int len = timeParams.Length;
            int i = 0;
            int year = len > i ? int.Parse(timeParams[i++]) : 0;
            int month = len > i ? int.Parse(timeParams[i++]) : 1; ;
            int day = len > i ? int.Parse(timeParams[i++]) : 1;
            int hour = len > i ? int.Parse(timeParams[i++]) : 0;
            int minute = len > i ? int.Parse(timeParams[i++]) : 0;
            double seconds = len > i ? Double.Parse(timeParams[i++]) : 0;

            /// 80-99: 1980-1999，
            /// 00-79: 2000-2079
            if (year < 79 && year >= 0) year += 2000;
            if (year > 70 && year <= 99) year += 1900;



            while (seconds >= 60)
            {
                minute += 1;
                seconds -= 60;
            }

            while (minute >= 60)
            {
                hour += 1;
                minute -= 60;
            }
            while (hour >= 24)
            {
                day += 1;
                hour -= 24;
            }


            return new Time(year, month, day, hour, minute, seconds);
        }

        /// <summary>
        /// YY:DDD:SSSSS.
        /// </summary>
        /// <returns></returns>
        public static string ToYdsString(Time gpsTime)
        {
            return gpsTime.Year.ToString().Substring(gpsTime.Year.ToString().Length - 2)
                + ":"
                + StringUtil.FillZeroLeft(gpsTime.DayOfYear, 3)
                + ":"
                + StringUtil.FillZeroLeft((int)gpsTime.SecondsOfDay, 5);
        }
        /// <summary>
        ///  YY:DDD:SSSSS.
        /// </summary>
        /// <param name="degStr"></param>
        /// <returns></returns>
        public static Time ParseYds(string str)
        {
            string[] strs = str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            int year = 0;
            int y = int.Parse(strs[i++]);
            if (y <= 50) year = 2000 + y;
            else year = 1900 + y;

            int doy = int.Parse(strs[i++]);
            int sid = int.Parse(strs[i++]);

            DateTime time = new DateTime(year, 1, 1) + TimeSpan.FromDays(doy - 1) + TimeSpan.FromSeconds(sid);
            return new Time(time);
        } 
    }
}