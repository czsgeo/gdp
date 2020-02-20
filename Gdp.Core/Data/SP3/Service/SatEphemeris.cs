//2018.03.13, czs, create in hmx, 单独记录一颗卫星的星历列表。

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq; 

namespace Gdp.Data.Rinex
{
    /// <summary>
    ///  单独记录一颗卫星的星历列表，与SP3格式无关
    /// </summary>
    public class SatEphemeris //: BaseDictionary<SatelliteNumber, Sp3Record>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SatEphemeris() { }


        /// <summary>
        /// 起始时间
        /// </summary>
        public Time StartTime { get; set; } 

        /// <summary>
        /// 采样间隔，单位：秒
        /// </summary>
        public double Interval { get; set; }






    }
     
}
