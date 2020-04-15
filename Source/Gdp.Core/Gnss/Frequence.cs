// 2014.09.16, czs, refactor, 频率命名为：Frequency，值为：Value，波长不变。
//2018.06.03, lly & czs, edit in zz & hmx, 修改伽利略频率，按照RINEX的顺序 1 5 7 8 6

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 频段。 GNSS 波段和频率。
    /// </summary>
    public class Frequence : NumeralValue
    {
        static Log log = new Log(typeof(Frequence));
        /// <summary>
        /// 实例化一个 GNSS 频率。和波段
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="frequence">频率，单位为10的6次方</param>
        public Frequence(string name, double frequence)
        {
            this.Name = name; 
            this.Value = frequence;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 波长,单位 米
        /// </summary>
        public double WaveLength { get { return GeoConst.LIGHT_SPEED / Value / 1E6; } }

        /// <summary>
        /// 频率，单位是 10 的 6 次方。
        /// </summary>
        public override double Value { get; set; }

        /// <summary>
        /// 获取整周数对应的距离。
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        public double GetDistance(long cycle)
        {
            double len = WaveLength * cycle;
          //  len = 0.10695337814214670 * cycle;
            return len;
        }
        /// <summary>
        /// 获取整周数。取最小整数。
        /// </summary>
        /// <param name="distance">距离</param>
        /// <returns></returns>
        public long GetCycle(double distance)
        {
            double clycle =  Math.Floor((distance / WaveLength));
          //  clycle = Math.Floor((distance / 0.10695337814214670));
            return (long)clycle;
        }

        #region override
        /// <summary>
        /// 相等否
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Frequence o = obj as Frequence;
            if (o == null) return false;

            return Name == (o.Name) && Value == o.Value;
        }


        /// <summary>
        /// 啥系数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() + Value.GetHashCode();
        }
        /// <summary>
        /// 字符串表达
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name.ToString() + "(f=" + Value + ", λ=" + WaveLength + ")";
        }

        #endregion

        #region 静态方法
      
       
        /// <summary>
        /// 获取频率
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="frequenceType"></param>
        /// <param name="time">时间，历元，GLONASS或频分多址需要</param>
        /// <returns></returns>
        public static Frequence GetFrequence(SatelliteNumber prn, FrequenceType frequenceType, Time time = default(Time))
        {
            return GetFrequence(prn.SatelliteType, frequenceType, prn.PRN, time);
        }
        /// <summary>
        /// 获取已知卫星的频率带宽
        /// </summary>
        /// <param name="type"></param>
        /// <param name="freqtType"></param>
        /// <param name="satNumber">卫星编号，GLONASS需要</param>
        /// <returns></returns>
        public static Frequence GetFrequence(SatelliteType type, FrequenceType freqtType, int satNumber = -1, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(type), freqtType, satNumber, time);
        }
        /// <summary>
        /// 获取已知的频率带宽
        /// </summary>
        /// <param name="prn">卫星编号，如果是频分多址则需要指定卫星序号，如Glonass</param>
        /// <param name="rinexFreqNum">RINEX 频率编号</param>
        /// <param name="time">时间，历元，GLONASS或频分多址需要</param>
        /// <returns></returns>
        public static Frequence GetFrequence(SatelliteNumber prn, int rinexFreqNum, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(prn.SatelliteType), rinexFreqNum, prn.PRN, time);
        }
        /// <summary>
        /// 获取第一频率，推荐方法
        /// </summary>
        /// <param name="prn">卫星编号</param>
        /// <param name="time">时间，GLONASS需要</param>
        /// <returns></returns>
        public static Frequence GetFrequenceA(SatelliteNumber prn, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(prn.SatelliteType), FrequenceType.A, prn.PRN, time);
        }

        /// <summary>
        /// 获取第2频率，推荐方法
        /// </summary>
        /// <param name="prn">卫星编号</param>
        /// <param name="time">时间，GLONASS需要</param>
        /// <returns></returns>
        public static Frequence GetFrequenceB(SatelliteNumber prn, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(prn.SatelliteType), FrequenceType.B, prn.PRN, time);
        }

        /// <summary>
        /// 获取第3频率，推荐方法
        /// </summary>
        /// <param name="prn">卫星编号</param>
        /// <param name="time">时间，GLONASS需要</param>
        /// <returns></returns>
        public static Frequence GetFrequenceC(SatelliteNumber prn, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(prn.SatelliteType), FrequenceType.C, prn.PRN, time);
        }
        /// <summary>
        /// 获取系统第二频率
        /// </summary>
        /// <param name="type"></param>
        /// <param name="satNumber">卫星编号，GLONASS需要</param>
        /// <param name="time">时间，历元，GLONASS需要</param>
        /// <returns></returns>
        public static Frequence GetFrequenceA(SatelliteType type, int satNumber = -1, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(type), FrequenceType.A, satNumber, time); 
        }
        /// <summary>
        /// 获取系统第一频率
        /// </summary>
        /// <param name="type"></param>
        /// <param name="satNumber"></param>
        /// <returns></returns>
        public static Frequence GetFrequenceB(SatelliteType type, int satNumber = -1, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(type), FrequenceType.B, satNumber, time); 
        }
        /// <summary>
        /// 获取系统第3频率
        /// </summary>
        /// <param name="type"></param>
        /// <param name="satNumber"></param>
        /// <param name="time">时间，历元，GLONASS需要</param>
        /// <returns></returns>
        public static Frequence GetFrequenceC(SatelliteType type, int satNumber = -1, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(type), FrequenceType.C, satNumber, time);
        }
        /// <summary>
        /// 获取系统第二频率
        /// </summary>
        /// <param name="type"></param>
        /// <param name="satNumber"></param>
        /// <param name="time">时间，历元</param>
        /// <returns></returns>
        public static Frequence GetFrequenceA(GnssType type, int satNumber = -1, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(type, FrequenceType.A, satNumber, time) ;
        }
        /// <summary>
        /// 获取系统第一频率
        /// </summary>
        /// <param name="type"></param>
        /// <param name="satNumber"></param>
        /// <param name="time">时间，历元，GLONASS需要</param>
        /// <returns></returns>
        public static Frequence GetFrequenceB(GnssType type, int satNumber = -1, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(type, FrequenceType.B, satNumber,time);
        }
        /// <summary>
        /// 获取频率带宽。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rinexNum"></param>
        /// <param name="satNumber"></param>
        /// <param name="time">时间，历元</param>
        /// <returns></returns>
        public static Frequence GetFrequence(SatelliteType type, int rinexNum, int satNumber = -1, Time time = default(Time))
        {
            return ObsCodeConvert.GetFrequenceBand(GnssSystem.GetGnssType(type), rinexNum, satNumber,time);
        }
        /// <summary>
        /// 获取频率
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rinexNum"></param>
        /// <param name="satNumber"></param>
        /// <param name="time">时间，历元，GLONASS或频分多址需要</param>
        /// <returns></returns>
        public static Frequence GetFrequence(GnssType type, int rinexNum, int satNumber = -1, Time time = default(Time))
        {
            return   ObsCodeConvert.GetFrequenceBand(type,rinexNum,satNumber ,time );
        }
        #endregion

        #region 常用
        /// <summary>
        /// 默认采用GPSL1
        /// </summary>
        public static Frequence Default { get => GpsL1; }

        /// <summary>
        /// 获取宽项
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Frequence GetWideBand(Frequence A, Frequence B)
        {
            return new Frequence("WideBand", A.Value - B.Value);
        }
        /// <summary>
        /// 获取窄项
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Frequence GetNarrowBand(Frequence A, Frequence B)
        {
            return new Frequence("NarrowBand", A.Value + B.Value);
        }

        /// <summary>
        /// 通过载波作差求取的电离层与硬件延迟的系数。f2^2/(f1^2-f2^2)
        /// </summary>
        /// <param name="satelliteType"></param>
        /// <returns></returns>
        internal static double GetIonoAndDcbOfL1CoeffL1L2(SatelliteType satelliteType)
        {
            var f1 = Frequence.GetFrequenceA(satelliteType);
            var f2 = Frequence.GetFrequenceB(satelliteType);
            double f1f1 = f1.Value * f1.Value;
            double f2f2 = f2.Value * f2.Value;
            var val = 1.0 * f2f2 / (f1f1 - f2f2);
            return val;
        }
        /// <summary>
        /// 通过载波作差求取的电离层与硬件延迟的系数。f1^2/(f1^2-f2^2)
        /// </summary>
        /// <param name="satelliteType"></param>
        /// <returns></returns>
        internal static double GetIonoAndDcbOfL2CoeffL1L2(SatelliteType satelliteType)
        {
            var f1 = Frequence.GetFrequenceA(satelliteType);
            var f2 = Frequence.GetFrequenceB(satelliteType);
            double f1f1 = f1.Value * f1.Value;
            double f2f2 = f2.Value * f2.Value;
            var val = 1.0 * f1f1 / (f1f1 - f2f2);
            return val;
        }

        #region GPS
        /// <summary>
        /// GPS 宽项组合
        /// </summary>
        public static Frequence GpsWideBand { get { return GetWideBand(GpsL1, GpsL2); } }


        /// <summary>
        /// GPS z窄项组合
        /// </summary>
        public static Frequence GpsNarrowBand { get { return GetNarrowBand(GpsL1, GpsL2); } }

        /// <summary>
        /// GPS SBAS L1. 1575.42 波长 0.19029
        /// </summary>
        static public Frequence GpsL1= new Frequence("L1", 1575.42); 
        /// <summary>
        /// GPS L2. 1227.60波长0.24421
        /// </summary>
        static public Frequence GpsL2 = new Frequence("L2", 1227.60);
        /// <summary>
        /// GPS MW 频率
        /// </summary>
        static public Frequence GpsMw = new Frequence("Mw", GpsL1.Value - GpsL2.Value);
        /// <summary>
        /// GPS SBAS L5
        /// </summary>
        static public Frequence GpsL5 { get { return new Frequence("L5", 1176.45); } }
        /// <summary>
        /// 指定系统MW组合频率
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="time">时间，历元</param>
        /// <returns></returns>
        static public Frequence GetMwFrequence(SatelliteNumber prn, Time time)
        {
            var f1 = GetFrequenceA(prn, time);
            var f2 = GetFrequenceB(prn, time);
            var f = f1.Value - f2.Value;

            return GetCompositFreqence(1, f1, -1, f2, "MwOf" + prn);
            //return new Frequence("MwOf" + satType, f);
        }
        /// <summary>
        /// 指定系统窄巷组合频率
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="time">时间，历元</param>
        /// <returns></returns>
        static public Frequence GetNarrowLaneFrequence(SatelliteNumber prn, Time time)
        {
            var f1 = GetFrequenceA(prn, time);
            var f2 = GetFrequenceB(prn, time);
            //var f = f1.Value + f2.Value;

            return GetCompositFreqence(1, f1, 1, f2, "NarrowLaneOf" + prn);
        }
        /// <summary>
        /// 两个频率组成新的频率。简单的线性组合
        /// 频率和载波相位都可以直接相加，而距离需要转换。
        /// </summary>
        /// <param name="factorA">系数A</param>
        /// <param name="bandA">频率A</param>
        /// <param name="factorB">系数B</param>
        /// <param name="bandB">频率B</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static Frequence GetCompositFreqence(double factorA, Frequence bandA, double factorB, Frequence bandB, string name = null)
        {
            double frequence = factorA * bandA.Value+ factorB* bandB.Value;
            name = name ?? bandA.Name + "(" + factorA.ToString("0.00") + ")" + "_" + bandB.Name + "(" + factorA.ToString("0.00") + ")";
            Frequence band = new Frequence(name, frequence);
            return band;
        }

        #endregion

        #region SBAS
        /// <summary>
        /// GPS SBAS L1. 1575.42 波长 0.19029
        /// </summary>
        static public Frequence SbasL1 { get { return new Frequence("L1", 1575.42); } } 
        /// <summary>
        /// GPS SBAS L5
        /// </summary>
        static public Frequence SbasL5 { get { return new Frequence("L5", 1176.45); } }
        #endregion

        #region Glonass

        /**
         * 
         *  24 R01  1 R02 -4 R03  5 R04  6 R05  1 R06 -4 R07  5 R08  6 GLONASS SLOT / FRQ #
         *     R09 -2 R10 -7 R11  0 R12 -1 R13 -2 R14 -7 R15  0 R16 -1 GLONASS SLOT / FRQ #
         *     R17  4 R18 -3 R19  3 R20  2 R21  4 R22 -3 R23  3 R24  2 GLONASS SLOT / FRQ #
         */

        /// <summary>
        /// GLONASS G1  
        /// </summary>
        /// <param name="k">1-24卫星编号</param>
        /// <returns></returns>
        static public Frequence GetGlonassG1(int k)
        {
          //  Frequence g1 = new Frequence("G1", 1602 + k * 9 / 16);
            Frequence g1 = new Frequence("G1Of" + k, 1602.5625 +(k-1) * 0.5625);
            return g1;
        }
        /// <summary>
        /// GLONASS G2  k= -7...+12
        /// </summary>
        /// <param name="k">1-24卫星编号</param>
        /// <returns></returns>
        static public Frequence GetGlonassG2(int k)
        {
            // 7/16
            Frequence g2 = new Frequence("G2Of" + k, 1246.4375 + (k-1) * 0.4375);
            return g2;
        }
        /// <summary>
        /// GLONASS G3
        /// </summary>
        static public Frequence GlonassG3 { get { return new Frequence("G3", 1202.025); } }

        #endregion

        #region Galileo
        /// <summary>
        /// Galileo E1
        /// </summary>
        static public Frequence GalileoE1 { get { return new Frequence("E1", 1575.42); } }

        /// <summary>
        /// Galileo E5a
        /// </summary>
        static public Frequence GalileoE5a { get { return new Frequence("E5a", 1176.45); } }

        /// <summary>
        /// Galileo E5b
        /// </summary>
        static public Frequence GalileoE5b { get { return new Frequence("E5b", 1207.140); } }
        /// <summary>
        /// Galileo E5 (E5a+E5b)
        /// </summary>
        static public Frequence GalileoE5 { get { return new Frequence("E5", 1191.795); } }

        /// <summary>
        /// Galileo QZSS E6
        /// </summary>
        static public Frequence GalileoE6 { get { return new Frequence("E6", 1278.75); } }
        /// <summary>
        /// Galileo QZSS E6
        /// </summary>
        static public Frequence QzssL6 { get { return new Frequence("L6", 1278.75); } }
        #endregion

        #region CompassE1
        /// <summary>
        /// Compass E1 ??? 这个待定的
        /// </summary>
      //  static public Frequence CompassE1 { get { return new Frequence("E1", 1589.74); } }

        /// <summary>
        /// 1、2, Compass B1
        /// </summary>
        static public Frequence CompassB1 { get { return new Frequence("B1", 1561.098); } } 
        /// <summary>
        /// 5, Compass B2
        /// </summary>
        static public Frequence CompassB2a { get { return new Frequence("B2a", 1176.45); } }
        /// <summary>
        /// 7, Compass B2
        /// </summary>
        static public Frequence CompassB2 { get { return new Frequence("B2", 1207.140); } }
        /// <summary>
        /// 8
        /// </summary>
        static public Frequence CompassB2ab { get { return new Frequence("B2ab", 1191.795); } }

        /// <summary>
        /// 6, Compass B2
        /// </summary>
        static public Frequence CompassB3 { get { return new Frequence("B3", 1268.52); } }

        #endregion

        #region NAVIC
        /// <summary>
        /// NAVIC L5
        /// </summary>
        static public Frequence NavicL5 { get { return new Frequence("L5", 1176.45); } }
        #endregion


        #endregion
    }

}
