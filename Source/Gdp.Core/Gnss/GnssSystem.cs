//2014.12.19, czs edit in namu, 增加了信号寻址方式属性，去掉了频率列表。
//2018.04.27, czs, edit in hmx, 增加了IRNSS、SBAS、QZSS系统的支持，新增系统的频率有待确认

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




namespace Gdp
{
    /// <summary>
    /// 信号寻址方式
    /// </summary>
    public enum AddressingType
    {
        /// <summary>
        /// 码分多址  Code Division Multiple Address
        /// </summary>
        CDMA,
        /// <summary>
        /// 频分多址 Frequency Division Multiple Address
        /// </summary>
        FDMA
    }

    /// <summary>
    /// GNSS 系统信息, 元数据信息。
    /// </summary>
    public class GnssSystem
    {
        /// <summary>
        /// 构造一个GNSS系统对象
        /// </summary>
        /// <param name="name">GNSS系统名称</param>
        public GnssSystem(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 寻址方式。GLONASS为频分多址。
        /// </summary>
        public AddressingType AddressingType { get; set; }
        /// <summary>
        /// 系统类型。
        /// </summary>
        public GnssType GnssType { get; set; }
        /// <summary>
        /// 参考椭球
        /// </summary>
        public Ellipsoid Ellipsoid { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
         
        /// <summary>
        /// 时间起始。
        /// </summary>
        public decimal OriginMjdDay { get; set; }
        /// <summary>
        /// 通用卫星（非静止轨道）到测站的大概距离，用于定权做乘法因子。
        /// </summary>
        public double ApproxRadius { get { return GetApproxSatRadius(GetSatelliteType(GnssType)); } }
        #region 类型转换
        /// <summary>
        /// 通过卫星类型获取GNSS系统类型
        /// </summary>
        /// <param name="types">卫星类型（采用RINEX格式描述）</param>
        /// <returns></returns>
        public static List<GnssType> GetGnssTypes(List<SatelliteType> types)
        {
            List<GnssType> gnssTypes = new List<GnssType>();
            foreach (var item in types)
            {
                gnssTypes.Add(GetGnssType(item));
            }
            return gnssTypes;
        }
        /// <summary>
        /// 通过卫星类型获取GNSS系统类型
        /// </summary>
        /// <param name="type">卫星类型（采用RINEX格式描述）</param>
        /// <returns></returns>
        public static GnssType GetGnssType(SatelliteType type)
        {
            switch (type)
            { 
                case SatelliteType.G:
                    return GnssType.GPS;
                case SatelliteType.C:
                    return GnssType.BeiDou;
                case SatelliteType.E:
                    return GnssType.Galileo;
                case SatelliteType.R:
                    return GnssType.GLONASS;
                case SatelliteType.I:
                    return GnssType.IRNSS;
                case SatelliteType.J:
                    return GnssType.QZSS;
                case SatelliteType.S:
                    return GnssType.SBAS;
                default:
                    break;
            }
            return GnssType.Unkown;

            throw new Exception("对不起，暂不支持其它的系统。");
            //return GnssSystem.Gps;
        }
        /// <summary>
        /// 获取卫星类型
        /// </summary>
        /// <param name="type">GNSS系统类型</param>
        /// <returns></returns>
        public static SatelliteType GetSatelliteType(GnssType type)
        {
            switch (type)
            {
                case GnssType.GPS:
                    return SatelliteType.G;
                case GnssType.BeiDou:
                    return SatelliteType.C;
                case GnssType.Galileo:
                    return SatelliteType.E;
                case GnssType.GLONASS:
                    return SatelliteType.R;
                case GnssType.IRNSS:
                    return SatelliteType.I;
                case GnssType.QZSS:
                    return SatelliteType.J;
                case GnssType.SBAS:
                    return SatelliteType.S; 
                default:
                    break;
            }
            throw new Exception("对不起，暂不支持其它的系统。");
            //return GnssSystem.Gps;
        }
        #endregion
        /// <summary>
        /// 通过卫星类型获取GNSS系统实例。
        /// </summary>
        /// <param name="type">卫星类型（采用RINEX格式描述）</param>
        /// <returns></returns>
        public static GnssSystem GetGnssSystem(SatelliteType type)
        {
            switch (type)
            {
                case SatelliteType.J:
                case SatelliteType.G:
                    return GnssSystem.Gps;
                case SatelliteType.C:
                    return GnssSystem.BeiDou;
                case SatelliteType.E:
                    return GnssSystem.Galileo;
                case SatelliteType.R:
                    return GnssSystem.Glonass;
                case SatelliteType.I:
                    return GnssSystem.IRNSS_NAVIC;
                default:
                    break;
            }
            throw new Exception("对不起，暂不支持其它的系统。");
            //return GnssSystem.Gps;
        }
        public static double  GetApproxSatRadius(SatelliteType type)
        {
            switch (type)
            {
                case SatelliteType.J:
                case SatelliteType.G:
                    return 22000000;
                case SatelliteType.C:
                    return 24000000;
                case SatelliteType.E:
                case SatelliteType.R: 
                    return 23000000;
                default:
                    break;

            }
            return 22000000;
            throw new Exception("对不起，暂不支持其它的系统。");
            //return GnssSystem.Gps;
        }

        #region 常用系统
        /// <summary>
        /// GPS 导航卫星系统
        /// </summary>
        public static GnssSystem Gps
        {
            get
            {
                GnssSystem gps = new GnssSystem("GPS")
                {
                    OriginMjdDay = TimeConsts.GPS_ORIGIN_MJD_DAY,
                    Ellipsoid = Ellipsoid.WGS84,
                    GnssType = GnssType.GPS,
                     AddressingType = AddressingType.CDMA
                     
                }; 
                return gps;
            }
        }
        /// <summary>
        /// 北斗导航卫星系统
        /// </summary>
        public static GnssSystem BeiDou
        {
            get
            {
                GnssSystem gps = new GnssSystem("BeiDou")
                {
                    OriginMjdDay = TimeConsts.Beidou_ORIGIN_MJD_DAY,
                    Ellipsoid = Ellipsoid.CGCS2000,
                    GnssType = GnssType.BeiDou,
                    AddressingType = AddressingType.CDMA
                };   
                return gps;
            }
        }
        /// <summary>
        /// 俄罗斯格洛纳斯导航卫星系统。
        /// </summary>
        public static GnssSystem Glonass
        {
            get
            {
                GnssSystem sys = new GnssSystem("Glonass")
                {
                    OriginMjdDay = TimeConsts.Beidou_ORIGIN_MJD_DAY ,
                    Ellipsoid = Ellipsoid.PZ90,
                    GnssType = GnssType.GLONASS,
                    AddressingType = AddressingType.FDMA
                }; 
                return sys;
            }
        }
        /// <summary>
        /// 欧洲伽利略导航卫星系统。
        /// 考虑到兰州数据格式按照其ABCD的顺序编排。
        /// </summary>
        public static GnssSystem Galileo
        {
            get
            {
                GnssSystem sys = new GnssSystem("Galileo")
                {
                    OriginMjdDay = TimeConsts.Beidou_ORIGIN_MJD_DAY,
                    GnssType = GnssType.Galileo,
                    Ellipsoid = Ellipsoid.WGS84,
                    AddressingType = AddressingType.CDMA           
                }; 
                return sys;
            }
        }

        #endregion

        public static GnssSystem IRNSS_NAVIC
        {
            get
            {
                GnssSystem sys = new GnssSystem("IRNSS_NAVIC")
                {
                    OriginMjdDay = TimeConsts.GPS_ORIGIN_MJD_DAY,
                    Ellipsoid = Ellipsoid.WGS84,
                    GnssType = GnssType.GPS,
                    AddressingType = AddressingType.CDMA
                };
                return sys;
            }
        }
    }
}
