// 2014.09.20， czs, edit, 加入 Frequency 使其参数化。
//2018.09.28, czs, edit in hmx, 简化，更名为 RinexSatFrequency

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 天线中采用的频率表示方法。G01表示 GPS L1。
    /// </summary>
    public class RinexSatFrequency
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="rinexNum"></param>
        public RinexSatFrequency(SatelliteNumber prn, int rinexNum)
          : this(prn.SatelliteType, rinexNum)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="SatelliteType">卫星类型</param>
        /// <param name="CarrierNumber">载波序号</param>
        public RinexSatFrequency(SatelliteType SatelliteType, int CarrierNumber)
        {
            this.SatelliteType = SatelliteType;
            this.RinexCarrierNumber = CarrierNumber;
        }
        /// <summary>
        /// 卫星类型
        /// </summary>
        public SatelliteType SatelliteType { get; set; }

        /// <summary>
        /// 载波序号
        /// </summary>
        public int RinexCarrierNumber { get; set; }

        /// <summary>
        /// 对应的频率
        /// </summary>
        /// <param name="satNum">卫星编号，如果码分多址则不需要，频分多址则需要，如GLONASS</param>
        /// <returns></returns>
        public Frequence GetFrequence(int satNum = -1, Time time = default(Time)) { return Frequence.GetFrequence(SatelliteType, RinexCarrierNumber, satNum, time); }

        /// <summary>
        /// 解析字符串，如G01，R02。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static RinexSatFrequency Parse(string str)
        {
            SatelliteType type = (SatelliteType)Enum.Parse(typeof(SatelliteType), str[0].ToString());
            int number = Int32.Parse(str.Substring(1, 2));
            return new RinexSatFrequency(type, number);
        }

        public static RinexSatFrequency GpsA = new RinexSatFrequency(SatelliteType.G, 1);
        public static RinexSatFrequency GpsB = new RinexSatFrequency(SatelliteType.G, 2);
        public static RinexSatFrequency GpsC = new RinexSatFrequency(SatelliteType.G, 5);

        public static RinexSatFrequency BdsA = new RinexSatFrequency(SatelliteType.C, 2);
        public static RinexSatFrequency BdsB = new RinexSatFrequency(SatelliteType.C, 7);
        public static RinexSatFrequency BdsC = new RinexSatFrequency(SatelliteType.C, 6);
        public static RinexSatFrequency BdsD = new RinexSatFrequency(SatelliteType.C, 8);

        public static RinexSatFrequency GalileoA = new RinexSatFrequency(SatelliteType.E, 1);
        public static RinexSatFrequency GalileoB = new RinexSatFrequency(SatelliteType.E, 5);
        public static RinexSatFrequency GalileoC = new RinexSatFrequency(SatelliteType.C, 7);
        public static RinexSatFrequency GalileoD = new RinexSatFrequency(SatelliteType.C, 8);
        public static RinexSatFrequency GalileoE = new RinexSatFrequency(SatelliteType.C, 6);


        #region override
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is RinexSatFrequency)) return false;
            RinexSatFrequency o = (RinexSatFrequency)obj;
            return SatelliteType == o.SatelliteType && RinexCarrierNumber == o.RinexCarrierNumber;
        }
        /// <summary>
        /// 哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)SatelliteType * 10 + RinexCarrierNumber;
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SatelliteType.ToString() + RinexCarrierNumber.ToString("00");
        }
        #endregion
    }
}
