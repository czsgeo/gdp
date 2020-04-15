
// 2014.09.23, czs, edit， 提升到系统顶层 Gnsser.Common， 卫星编号都采用Rienx格式描述。

using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{

    /// <summary>
    /// 帮助工具
    /// </summary>
    public class SatelliteTypeHelper
    {
        /// <summary>
        /// 静态变量，只算一次，节省计算时间。
        /// </summary>
        public static List<string> SatTypes = SatelliteTypeHelper.GetSatelliteTypes();
        /// <summary>
        /// 获取所有名称
        /// </summary>
        /// <returns></returns>
        public static List<String> GetSatelliteTypes()
        {
            return new List<string>(Enum.GetNames(typeof(SatelliteType)));
        }
        /// <summary>
        /// 解析卫星类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static SatelliteType PareSatType(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return SatelliteType.U; }
            return (SatelliteType)Enum.Parse(typeof(SatelliteType), str.Trim().Substring(0, 1));
        }

        internal static string ToString(List<SatelliteType> list, string splitter = ",")
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in list)
            {
                if (i != 0) { sb.Append(splitter); }
                sb.Append(item);
                i++;
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 采用Rinex表达方法的卫星类型，GPS、GLONASS、Galileo ……
    /// </summary>
    [Serializable]
    public enum SatelliteType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        U = 0,
        /// <summary>
        /// G or blank : GPS
        /// </summary>
        G,
        /// <summary>
        ///  GLONASS          : GLONASS
        /// </summary>
        R,
        /// <summary>
        /// SBAS payload,          Geosync          : Geostationary signal payload
        /// </summary>
        S,
        /// <summary>
        ///  Galileo          : Galileo      
        /// </summary>
        E,
        /// <summary>
        /// 北斗
        /// </summary>
        C,
        /// <summary>
        /// Mixed
        /// </summary>
        M,
        /// <summary>
        /// QZSS 
        /// </summary>
        J,
        /// <summary>
        /// UserDefined
        /// </summary>
        D,
        /// <summary>
        /// IRNSS 或NAVIC
        /// </summary>
        I,

    }

}
