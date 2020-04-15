
//2018.04.27, czs, edit in hmx, 增加了IRNSS、SBAS、QZSS系统的支持

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// GNSS 系统类型,采用易读的英文单词表示。
    /// 注意区别于 Rinex 格式的 SatelliteType，很多情况下二者可以互相转换。
    /// </summary>
    public enum GnssType
    {
        /// <summary>
        /// 未知系统
        /// </summary>
        Unkown,
        /// <summary>
        /// GPS
        /// </summary>
        GPS,
        /// <summary>
        /// 北斗
        /// </summary>
        BeiDou,
        /// <summary>
        /// 格洛纳斯
        /// </summary>
        GLONASS,
        /// <summary>
        /// 伽利略
        /// </summary>
        Galileo,
        /// <summary>
        /// IRNSS 印度的 NAVIC
        /// </summary>
        NAVIC,
        /// <summary>
        /// IRNSS 印度的 NAVIC
        /// </summary>
        IRNSS,
        /// <summary>
        /// QZSS 
        /// </summary>
        QZSS,
        /// <summary>
        /// SBAS
        /// </summary>
        SBAS

    }
}