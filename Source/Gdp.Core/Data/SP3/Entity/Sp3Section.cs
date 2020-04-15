//2015.03.19, cy, sp3c格式读取文件，除了卫星坐标，还有坐标的sdev需要读取

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq; 

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// 一个历元 的 Sp3 记录列表
    /// </summary>
    public class Sp3Section : BaseDictionary<SatelliteNumber, Ephemeris>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sp3Section() { }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="Time"></param>
        public Sp3Section(Time Time) {
            this.Time = Time;
        }
        //Sp3 LINE 23 (epoch header clk)
        /// <summary>
        /// 起始时间
        /// </summary>
        public Time Time { get; set; }
        /// <summary>
        /// 移除其它系统
        /// </summary>
        /// <param name="satTypes"></param>
        public void RemoveOther(List<SatelliteType> satTypes)
        {
            List<SatelliteNumber> tobeRemoved = new List<SatelliteNumber>();
            foreach (var item in satTypes)
            {
                foreach (var eph in this)
                {
                    if (!satTypes.Contains(eph.Prn.SatelliteType))
                    {
                        tobeRemoved.Remove(eph.Prn);
                    } 
                } 
            }

            this.Remove(tobeRemoved);
        }

        /// <summary>
        /// 记录部分
        /// </summary>
        //public List<Sp3Record> satData { get; set; }

    }

    /// <summary>
    /// 不同系统存储一个时刻的改正数。
    /// </summary>
    public class InstantSp3Section : BaseDictionary<SatelliteType, Sp3Section>
    {

        /// <summary>
        /// 获取最新时间。
        /// </summary>
        public Time GetMaxTime()
        {
            return data.Values.Max(m => m.Time);
        }
        /// <summary>
        /// 获取最老时间。
        /// </summary>
        public Time GetMinTime()
        {
            return data.Values.Min(m => m.Time);
        }


    }
}
