//2015.04.26， czs, edit in namu, 原始观测码太多太复杂，需要转换为简单的几种类型。
//2018.04.27, czs, edit in hmx, 增加了IRNSS、SBAS、QZSS系统的支持，新增系统的频率有待确认
//2018.06.03, lly & czs, edit in zz & hmx, 修改伽利略频率，按照RINEX的顺序 1 5 7 8 6
//2018.09.25，czs, edit in hmx, 多系统支持重构，所有代码编号转换都放在这里


using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;

namespace Gdp
{
    /// <summary>
    /// 观测码转换。原始观测码太多太复杂，需要转换为简单的几种类型。
    /// </summary>
    public class ObsCodeConvert
    {
        /// <summary>
        /// 获取已知的频率带宽。
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="index">编号，1、2、3 分别代表各系统的频率如GPS的L1、L2、L5</param>
        /// <param name="satNumber">卫星编号，GLONASS 系统需要</param>
        public static Frequence GetFrequenceBand(SatelliteType type, int index = 1, int satNumber = -1)
        {
            return GetFrequenceBand(type, index, satNumber);
        }

        #region RINEX 频率在 GNSSer 中的顺序，及其相互转换 ,这是固定的，但可能会因为RINEX版本不同而不同

        /// <summary>
        /// 获取 RINEX 频率编号。
        /// </summary>
        /// <param name="satType"></param>
        /// <param name="frequenceType"></param>
        /// <returns></returns>
        public static List<int> GetRinexFrequenceNumber(SatelliteType satType, FrequenceType frequenceType )
        { 
            Dictionary<FrequenceType, List<int>> dic = GetRinexFreqIndexDic(satType);
            if (dic.ContainsKey(frequenceType))
            {
                return dic[frequenceType];
            }
            return new List<int>();
        }


        /// <summary>
        ///  根据对应关系获取频率编号，RINEX转换为GNSSer编号。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rinexNum"></param>
        /// <returns></returns>
        static public FrequenceType GetFrequenceType(SatelliteType type, int rinexNum)
        {
            Dictionary<FrequenceType, List<int>> dic = GetRinexFreqIndexDic(type);
            return GetFrequenceType(dic, rinexNum); 
        }
        /// <summary>
        ///  根据对应关系获取频率编号，RINEX转换为GNSSer编号。
        /// </summary>
        /// <param name="rinexSatFrequency"></param> 
        /// <returns></returns>
        static public FrequenceType GetFrequenceType(RinexSatFrequency rinexSatFrequency)
        {
            Dictionary<FrequenceType, List<int>> dic = GetRinexFreqIndexDic(rinexSatFrequency.SatelliteType);
            return GetFrequenceType(dic, rinexSatFrequency.RinexCarrierNumber); 
        }

        /// <summary>
        /// 根据对应关系获取频率编号，RINEX转换为GNSSer编号。
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="rinexNum"></param>
        /// <returns></returns>
        static public FrequenceType GetFrequenceType(Dictionary<FrequenceType, List<int>> dic, int rinexNum)
        {
            foreach (var kv in dic)
            {
                if (kv.Value.Contains(rinexNum)) { return kv.Key; } 
            }
            return FrequenceType.Unknown;
        }

        private static Dictionary<SatelliteType, Dictionary<FrequenceType, List<int>>> CasheOfRinexFreqIndexDics = new Dictionary<SatelliteType, Dictionary<FrequenceType, List<int>>>();
        private static object freqLocker = new object();
        /// <summary>
        /// 计算各频率对应的 RINEX 编号，有的是一对多的关系。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<FrequenceType, List<int>> GetRinexFreqIndexDic(SatelliteType type)
        {
            if(CasheOfRinexFreqIndexDics != null && CasheOfRinexFreqIndexDics.ContainsKey(type)) { return CasheOfRinexFreqIndexDics[type]; }
            lock (freqLocker)
            {
                if (CasheOfRinexFreqIndexDics != null && CasheOfRinexFreqIndexDics.ContainsKey(type)) { return CasheOfRinexFreqIndexDics[type]; }

                Dictionary<FrequenceType, List<int>> result = new Dictionary<FrequenceType, List<int>>();
                switch (type)
                {
                    case SatelliteType.S:
                    case SatelliteType.G:
                        result[FrequenceType.A] = new List<int>() { 1 };
                        result[FrequenceType.B] = new List<int>() { 2 };
                        result[FrequenceType.C] = new List<int>() { 5 };
                        break;
                    case SatelliteType.R:
                        result[FrequenceType.A] = new List<int>() { 1 };
                        result[FrequenceType.B] = new List<int>() { 2 };
                        result[FrequenceType.C] = new List<int>() { 3 };
                        break;
                    case SatelliteType.E:
                        result[FrequenceType.A] = new List<int>() { 1 };
                        result[FrequenceType.B] = new List<int>() { 5 };
                        result[FrequenceType.C] = new List<int>() { 7 };
                        result[FrequenceType.D] = new List<int>() { 8 };
                        result[FrequenceType.E] = new List<int>() { 6 };
                        break;
                    case SatelliteType.C: //215786
                        result[FrequenceType.A] = new List<int>() { 2 , 1};
                        result[FrequenceType.B] = new List<int>() { 7 };
                        result[FrequenceType.C] = new List<int>() { 6 };//
                       // result[FrequenceType.D] = new List<int>() { 6 }; 
                        break;
                    case SatelliteType.J:
                        result[FrequenceType.A] = new List<int>() { 1 };
                        result[FrequenceType.B] = new List<int>() { 2 };
                        result[FrequenceType.C] = new List<int>() { 5 };
                        result[FrequenceType.D] = new List<int>() { 6 };
                        break;
                    case SatelliteType.D:
                        break;
                    case SatelliteType.I:
                        break;
                    default:
                        result[FrequenceType.A] = new List<int>() { 1 };
                        result[FrequenceType.B] = new List<int>() { 2 };
                        result[FrequenceType.C] = new List<int>() { 5 };
                        break;
                }

                CasheOfRinexFreqIndexDics[type] = result;
                return result;
            }
        }
        #endregion

        #region 获取频率对象，如果是GLONASS，则频率和卫星Slot和时间相关
        /// <summary>
        /// 获取已知的频率带宽。
        /// </summary>
        /// <param name="satNumber">卫星编号， GLONASS 频分多址</param>
        /// <param name="type">类型</param>
        /// <param name="rinexNum">编号，1、2、3 分别代表各系统的频率如GPS的L1、L2、L5</param>
        /// <param name="time">时间，历元，GLONASS或频分多址需要</param>
        /// <returns></returns>
        public static Frequence GetFrequenceBand(GnssType type, int rinexNum = 1, int satNumber = -1, Time time = default(Time))
        {
            var satType = GnssSystem.GetSatelliteType(type);
            var freqType = GetFrequenceType(satType, rinexNum);
            return GetFrequenceBand(type, freqType, satNumber,time);
        }

        /// <summary>
        /// 获取频率带宽
        /// </summary>
        /// <param name="type">系统类型</param>
        /// <param name="freqType">频率类型</param>
        /// <param name="satNumber">卫星编号，GLONASS 系统需要</param>
        /// <param name="time">time，GLONASS 系统需要</param>
        /// <returns></returns>
        public static Frequence GetFrequenceBand(GnssType type, FrequenceType freqType, int satNumber = -1, Time time=default(Time))
        {
            switch (type)
            {
                case GnssType.GPS:
                case GnssType.SBAS:
                    switch (freqType)
                    {
                        case FrequenceType.A: return Frequence.GpsL1;
                        case FrequenceType.B: return Frequence.GpsL2;
                        case FrequenceType.C: return Frequence.GpsL5;
                        default:
                            return null;
                            // throw new ArgumentException("GPS 有三个频率。分别以编号1、2、3表示。");
                    }
                case GnssType.Galileo:
                    switch (freqType)
                    {
                        case FrequenceType.A: return Frequence.GalileoE1;
                        case FrequenceType.B: return Frequence.GalileoE5a;
                        case FrequenceType.C: return Frequence.GalileoE5b;
                        case FrequenceType.D: return Frequence.GalileoE5;
                        case FrequenceType.E: return Frequence.GalileoE6;
                        default:
                            return null;
                            //  throw new ArgumentException("Galileo 有5个频率。分别以编号 1-5 表示。");
                    }
                case GnssType.BeiDou: //215786
                    switch (freqType)
                    {
                        case FrequenceType.A: return Frequence.CompassB1;
                        case FrequenceType.B: return Frequence.CompassB2;
                        case FrequenceType.C: return Frequence.CompassB3; 
                        default:
                            return null;
                            //   throw new ArgumentException("BeiDou 有三个频率。分别以编号1、2、3表示。");
                    }
                case GnssType.GLONASS:
                    if (satNumber == -1)
                        throw new ArgumentException("GLONASS是频分多址，需要指定卫星编号，此处有待改进！！！！请联系开发人员。");
                    var prn = new SatelliteNumber(satNumber, SatelliteType.R);
                    var k = (int)GlobalGlonassSlotFreqService.Instance.Get(prn, time);
                  //   var k = Setting.GnsserConfig.GlonassSlotFrequences[prn];
                    
                    switch (freqType)
                    {
                        case FrequenceType.A: return Frequence.GetGlonassG1(k);
                        case FrequenceType.B: return Frequence.GetGlonassG2(k);
                        case FrequenceType.C: return Frequence.GlonassG3;
                        default:
                            return null;
                            //  throw new ArgumentException("GLONASS 有2个载波。分别以编号1、2表示。");
                    }
                case GnssType.QZSS:
                    switch (freqType)
                    {
                        case FrequenceType.A: return Frequence.GpsL1;
                        case FrequenceType.B: return Frequence.GpsL2;
                        case FrequenceType.C:return Frequence.GpsL5;
                        case FrequenceType.D: return Frequence.QzssL6;
                        default:
                            return null;
                    }
                case GnssType.NAVIC:
                    switch (freqType)
                    {
                        case FrequenceType.A: return Frequence.NavicL5;
                        default:
                            return null;
                    }
                default:

                    switch (freqType)
                    {
                        case FrequenceType.A: return Frequence.GpsL1;
                        case FrequenceType.B: return Frequence.GpsL2;
                        case FrequenceType.C: return Frequence.GpsL5;
                        default:
                            return null;
                    }
            }

            throw new ArgumentException(type + "尚不支持，请联系管理员。");

        }
        #endregion
    }
}
