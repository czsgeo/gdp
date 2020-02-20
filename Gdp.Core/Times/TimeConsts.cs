using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Gdp
{
    /// <summary>
    /// 时间常用常数
    /// </summary>
    public class TimeConsts
    { 

        #region 常用常数
        public const long TicksPerDay = 864000000000;
        //
        // 摘要: 
        //     表示 1 小时的刻度数。此字段为常数。
        public const long TicksPerHour = 36000000000;
        //
        // 摘要: 
        //     表示 1 毫秒的刻度数。此字段为常数。
        public const long TicksPerMillisecond = 10000;
        //
        // 摘要: 
        //     表示 1 分钟的刻度数。此字段为常数。
        public const long TicksPerMinute = 600000000;
        //
        // 摘要: 
        //     表示 1 秒的刻度数。
        public const long TicksPerSecond = 10000000;
        /// <summary>
        /// 1  秒 转化为 系统时间 TICK 的乘法因子
        /// </summary>
        public const long TICKS_PER_SECOND = 10000000;
        /// <summary>
        /// 1 系统时间 TICK 转化为 秒的乘法因子
        /// </summary>
        public const decimal SECOND_PER_TICKS = 1E-7M;
        /// <summary>
        /// 一个小时所包含的秒数
        /// </summary>
        public const int SECOND_PER_HOUR = 3600;
        /// <summary>
        /// 一天所包含的秒数
        /// </summary>
        public const int SECOND_PER_DAY = 86400;
        /// <summary>
        /// 一周所包含的秒数
        /// </summary>
        public const int SECOND_PER_WEEK = 604800; 

        // Konstanten
        /// <summary>
        ///  GPS 起始时间（基准为简化儒略日）。
        ///  GPS-Wochenanfang MJD 44244.0 = 0 UT
        /// </summary>
        public const int GPS_ORIGIN_MJD_DAY = 44244;
        /// <summary>
        /// GPS 起始时间儒略日
        /// </summary>
        public const Decimal GPS_ORIGIN_JD_DAY = 2444244.5M;
        /// <summary>
        /// 北斗与GPS相差 1356周 14秒
        /// </summary>
        public const Decimal Beidou_ORIGIN_MJD_DAY = 44244 + 9492 +14M * SECOND_TO_DAY;

        /// <summary>
        /// 公元元年。
        /// </summary>
       // public const int YEAR_ONE_MJD = 100;
        /// <summary>
        /// 天 转化到 分钟 的乘法因子
        /// </summary>
        public const decimal DAY_TO_MINUTE = 1440;
        /// <summary>
        /// 分钟 转化到 天 的乘法因子
        /// </summary>
        public const decimal MINUTE_TO_DAY = 1.0M / DAY_TO_MINUTE;
        /// <summary>
        /// 天 转化到 秒 的乘法因子
        /// </summary>
        public const long DAY_TO_SECOND = 86400L;
        /// <summary>
        /// 秒 转化到 天 的乘法因子
        /// </summary>
        public const Decimal SECOND_TO_DAY = 1.0m / DAY_TO_SECOND;
        /// <summary>
        ///  天 转化到 毫秒 的乘法因子
        /// </summary>
        public const long DAY_TO_MILLISECOND = DAY_TO_SECOND * 1000;
        /// <summary>
        ///  天 转化到 微秒 的乘法因子
        /// </summary>
        public const long DAY_TO_MICROSECOND = DAY_TO_MILLISECOND * 1000;
        /// <summary>
        /// 毫秒 转化到 天 的乘法因子
        /// </summary>
        public const Decimal MILLISECOND_TO_DAY = 1.0m / DAY_TO_MILLISECOND;
        /// <summary>
        /// 平儒略日 转换到 儒略日 的加法因子
        /// </summary>
        public const Decimal MJD_TO_JD = 2400000.5m;
        /// <summary>
        /// 儒略日 转换到 平儒略日的 加法因子
        /// </summary>
        public const Decimal JD_TO_MJD = -2400000.5m;

         /// <summary>
        /// 天 转化到 分钟 的乘法因子
         /// </summary>
        public const double DAY_TO_MINUTE_DOUBLE = 1440;
        /// <summary>
        /// 分钟 转化到  天 的乘法因子
        /// </summary>
        public const double MINUTE_TO_DAY_DOUBLE = 1.0 / DAY_TO_MINUTE_DOUBLE;
        /// <summary>
        /// 天 转化到 秒 的乘法因子
        /// </summary>
        public const double DAY_TO_SECOND_DOUBLE = 86400;
        /// <summary>
        /// 秒 转化到  天 的乘法因子
        /// </summary>
        public const double SECOND_TO_DAY_DOUBLE = 1.0 / DAY_TO_SECOND_DOUBLE;
        /// <summary>
        /// 天 转化到 毫秒 的乘法因子
        /// </summary>
        public const double DAY_TO_MILLISECOND_DOUBLE = 1000.0 * 3600.0 * 24.0;
        /// <summary>
        /// 毫秒 转化到  天 的乘法因子
        /// </summary>
        public const double MILLISECOND_TO_DAY_DOUBLE = 1.0 / DAY_TO_MILLISECOND_DOUBLE;
        /// <summary>
        /// 平儒略日 转换到 儒略日 的加法因子
        /// </summary>
        public const double MJD_TO_JD_DOUBLE = 2400000.5;
        /// <summary>
        /// 儒略日 转换到 平儒略日的 加法因子
        /// </summary>
        public const double JD_TO_MJD_DOUBLE = -2400000.5;
        #endregion
    }
}