// 2014.09.23, czs, edit， 提升到系统顶层 Gnsser.Common， 卫星编号都采用Rienx格式描述。
// 2015.04.17, czs, edit in 那木, 将本类修改为 Struct 提升执行效率
//2016.09.29, czs, edit in hongqing,  修改解析算法，永不报错。错则返回 U00.
//2017.03.13,double add, 考虑到如果只有两位且首位为空则会出现解析错误的情况。


using System;
using System.Linq;
using System.Collections.Generic; 
using System.Text;
using Gdp.Utils;

namespace Gdp
{
    /**
     * 5.1 Satellite Numbers:

Version 2 has been prepared to contain GLONASS or other satellite systems'
observations. Therefore we have to be able to distinguish the satellites
of the different systems:  We precede the 2-digit satellite number with coefficient
system identifier.

        snn                  s:    satellite system identifier
                                   G or blank : GPS
                                   R          : GLONASS
                                   S          : Geostationary signal payload
                                   E          : Galileo                        |

                            nn:    - PRN (GPS, Galileo), slot number (GLONASS) |
                                   - PRN-100 (GEO)

        Note: G is mandatory in mixed GPS/GLONASS/Galileo files                |

                                   (blank default modified in April 1997)
     * 
     */
    /// <summary>
    /// 卫星编号。采用RINEX方式描述。
    /// </summary>
    public struct SatelliteNumber : IComparable<SatelliteNumber>, IEquatable<SatelliteNumber>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="type">请显示指定，避免错误</param>
        public SatelliteNumber(int prn, SatelliteType type)// = Gnsser.SatelliteType.U
        {
            this.PRN = prn;
            this.SatelliteType = type;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="type">请显示指定，避免错误</param>
        public SatelliteNumber(SatelliteType type, int prn)// = Gnsser.SatelliteType.U
        {
            this.PRN = prn;
            this.SatelliteType = type;
        }

        /// <summary>
        /// 卫星、卫星系统类型。
        /// </summary>
        public SatelliteType SatelliteType;// { get; set; }变量比属性效率高？？？2015.04.17, czs
        /// <summary>
        /// 整数编号。
        /// </summary>
        public int PRN;// { get; set; }
        /// <summary>
        /// 默认编号通常为 U00.
        /// </summary>
        public static SatelliteNumber Default = new SatelliteNumber(00, Gdp.SatelliteType.U);
        /// <summary>
        /// 形如 G01 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SatelliteType + PRN.ToString("00");// Utils.StringUtil.FillZero(PRN, 2);
        }

        #region 相等否
        /// <summary>
        /// 相等否
        /// </summary>
        /// <param name="lhs">左边</param>
        /// <param name="rhs">右边</param>
        /// <returns></returns>
        public static bool operator ==(SatelliteNumber lhs, SatelliteNumber rhs) { return lhs.Equals(rhs); }
        /// <summary>
        /// not相等否
        /// </summary>
        /// <param name="lhs">左边</param>
        /// <param name="rhs">右边</param>
        /// <returns></returns>
        public static bool operator !=(SatelliteNumber lhs, SatelliteNumber rhs) { return !lhs.Equals(rhs); }
        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">待比较目标</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is SatelliteNumber && Equals((SatelliteNumber)obj);
        }
        /// <summary>
        /// 相等否
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool Equals(SatelliteNumber s)
        {
            return PRN == s.PRN && SatelliteType == s.SatelliteType;
        }

        /// <summary>
        /// 首先按照类型排序，其次按照编号排序。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SatelliteNumber other)
        {
            if (this.SatelliteType == other.SatelliteType)
                return this.PRN - other.PRN;

            return this.SatelliteType - other.SatelliteType;
        }
        /// <summary>
        /// 哈希数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return PRN * 13 + (int)SatelliteType * 7; }
        #endregion
        /// <summary>
        /// 尝试解析卫星编号
        /// </summary>
        /// <param name="str"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static List<SatelliteNumber> TryParsePrns(string str, params string[] spliter)
        {
            List<SatelliteNumber> prns = new List<SatelliteNumber>();
            var strs = str.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in strs)
            {
                if (item.Length == 3)
                {
                    var prn = SatelliteNumber.TryParse(item);
                    if (prn == SatelliteNumber.Default) { continue; }
                    prns.Add(prn);
                }
            }
            return prns;
        }
        /// <summary>
        /// 1-3个字符。如"G01"," 01","  1","01","1"等。
        /// 如果失败，则返回 U00。
        /// /// </summary>
        /// <param name="snn"></param>
        /// <returns></returns>
        public static SatelliteNumber TryParse(string snn)
        {
            try
            {
                return Parse(snn);
            }
            catch (Exception ex)
            {
                new IO.Log(typeof(SatelliteNumber)).Error(ex.Message);
                return SatelliteNumber.Default;
            }
        }

        /// <summary>
        /// 按照分隔符切割，逐个解析，成功为止。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static SatelliteNumber TryParseFirst(string str, string[] spliter = null)
        {
            if (spliter == null)
            {
                spliter = new string[] { "-", "_" };
            }
            var strs = str.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in strs)
            {
                if (item.Length != 3) { continue; }

                var prn = Parse(item);
                if (prn != SatelliteNumber.Default && prn.PRN != 0) { return prn; }
            }
            return SatelliteNumber.Default;
        }


        /// <summary>
        /// 1-3个字符。如"G01"," 01","  1","01","1"等。
        /// 如果失败，则抛出异常。如果返回 U00，则表示解析失败。
        /// </summary>
        /// <param name="snn"></param>
        /// <returns></returns>
        public static SatelliteNumber Parse(string snn)
        {
            if (String.IsNullOrWhiteSpace(snn)) { return SatelliteNumber.Default; }

            if (snn.Length > 3)
            {
                snn = snn.Trim();
            }
            if (snn.Length > 3)
            {
                snn = snn.Substring(0,3);
            }


            //try
            //{
                SatelliteNumber s = new SatelliteNumber();
                s.SatelliteType = ParseType(snn);
                //解析编号
                var str = snn;
                if (snn.Length > 2) { str = (snn.Substring(1, 2)).Trim(); }//去除首字

                //if (str.Substring(0, 1) == " ")
                //    s.PRN = int.Parse(str.Substring(1, 1));//2017.03.13,double add, 考虑到如果只有两位且首位为空则会出现解析错误的情况。
                //else 
                if (Gdp.Utils.IntUtil.IsInt(str))//自动会忽略空格
                {
                    s.PRN = Utils.StringUtil.ParseInt(str);
                }
                return s;
            //}
            //catch (Exception ex)
            //{
            //    throw new FormatException("呵呵，好像我从不发生了。2016.09.29！解析卫星编号出错，" + ex.Message + ", " + snn);
            //}
        }

        private static SatelliteType  ParseType(string snn)
        {
            SatelliteType SatelliteType = SatelliteType.U;
            //无标识符默认为GPS
            if (snn.StartsWith(" ") || snn.Length == 2 || snn.Length == 1) SatelliteType = SatelliteType.G;
            else
            {
                var type = snn.Substring(0, 1).ToUpper();
                SatelliteType = Utils.EnumUtil.TryParse<SatelliteType>(type, SatelliteType.U);
            }

            return SatelliteType;
        }


        /// <summary>
        /// 解析 PRN 列表字符串。
        /// 默认长度为3，如G01G02G03.
        /// 也可以指定长度为1，则形如1235，会被解析成G01G02……
        /// </summary>
        /// <param name="satLine"></param>
        /// <param name="satNum"></param>
        /// <param name="prnLen"></param>
        /// <returns></returns>
        public static List<SatelliteNumber> ParsePRNs(string satLine, int satNum, int prnLen = 3)
        {
            List<SatelliteNumber> prns = new List<SatelliteNumber>();
            List<string> prnList = StringUtil.Split(satLine, prnLen, satNum);
            foreach (string prnstr in prnList)
            {
                prns.Add(SatelliteNumber.Parse(prnstr));
            }
            return prns;
        }
        /// <summary>
        /// 解析紧凑字符串
        /// </summary>
        /// <param name="satLine"></param>
        /// <returns></returns>
        public static List<SatelliteNumber> ParsePRNs(string satLine)
        {
            List<string> prnList = StringUtil.Split(satLine, 3);
            return Parse(prnList.ToArray());
        }

        /// <summary>
        /// 解析字符串如，G01,G02
        /// </summary>
        /// <param name="satLine"></param>
        /// <param name="splliter"></param>
        /// <returns></returns>
        public static List<SatelliteNumber> ParsePRNsBySplliter(string satLine, char[] splliter = null)
        {
            if (splliter == null)
            {
                splliter = new char[] { ' ', ',', '\t', ';', '，', '.' };
            }
            if (String.IsNullOrWhiteSpace(satLine))
                throw new ArgumentNullException(satLine);

            var strs = satLine.Split(splliter, StringSplitOptions.RemoveEmptyEntries);

            return Parse(strs);
        }
        /// <summary>
        /// 解析数组
        /// </summary>
        /// <param name="prnList"></param>
        /// <returns></returns>
        public static List<SatelliteNumber> Parse(string[] prnList)
        {
            List<SatelliteNumber> prns = new List<SatelliteNumber>();
            foreach (string prnstr in prnList)
            {
                prns.Add(SatelliteNumber.Parse(prnstr));
            }
            return prns;
        }


        public static SatelliteNumber Parse(string prn, SatelliteType satelliteSystem)
        {
            SatelliteNumber sn = SatelliteNumber.Parse(prn);
            if (satelliteSystem != SatelliteType.M)
                sn.SatelliteType = satelliteSystem;
            return sn;
        }
        /// <summary>
        /// 严格按照 三位数卫星编号判断。
        /// </summary>
        /// <param name="satelliteType">待解析字符串</param>
        /// <returns></returns>
        public static bool IsPrn(string prn)
        {
            if (prn.Length != 3) return false;
            if (!StringUtil.IsNumber(prn.Substring(1))) return false;
            // if (!Char.IsLetter(satelliteType[0])) return false;
            if (!Enum.IsDefined(typeof(SatelliteType), prn[0].ToString())) return false;
            return true;
        }
        /// <summary>
        ///  两个列表是否相等。
        /// </summary>
        /// <param name="prnsA">列表A</param>
        /// <param name="prnsB">列表B</param>
        /// <returns></returns>
        public static bool AreEqual(List<SatelliteNumber> prnsA, List<SatelliteNumber> prnsB)
        {
            if (prnsA.Count != prnsB.Count) return false;

            foreach (var item in prnsA)
            {
                if (!prnsB.Contains(item)) return false;
            }
            return true;
        }

        /// <summary>
        /// GPS 卫星编号。 G01-G32
        /// </summary>
        public static List<SatelliteNumber> GpsPrns
        {
            get
            {
                StringBuilder prns = new StringBuilder();
                for (int i = 1; i <= 32; i++)
                {
                    prns.Append("G" + i.ToString("00"));
                }

                return ParsePRNs(prns.ToString());
            }
        }

        public static bool IsNullOrDefault(SatelliteNumber prn)
        {
            return prn == null || prn == SatelliteNumber.Default;
        }

        #region 获取默认的卫星编号序列
        /// <summary>
        /// 返回默认32个GPS卫星编号
        /// </summary>
        public static List<SatelliteNumber> DefaultGpsPrns { get { return GetPrns(Gdp.SatelliteType.G, 32); } }
        /// <summary>
        /// 返回默认35个北斗卫星编号序列
        /// </summary>
        public static List<SatelliteNumber> DefaultBeidouPrns { get { return GetPrns(Gdp.SatelliteType.C, 35); } }
        /// <summary>
        /// 所有可能的卫星编号，四大系统
        /// </summary>
        public static List<SatelliteNumber> AllPrns = GetAllPrns();
        public static SatelliteNumber G01 = new SatelliteNumber(SatelliteType.G, 1);
        public static SatelliteNumber G02 = new SatelliteNumber(SatelliteType.G, 2);
        public static SatelliteNumber G03 = new SatelliteNumber(SatelliteType.G, 3);

        /// <summary>
        /// 所有可能的卫星编号，四大系统
        /// </summary>
        /// <returns></returns>
        private static List<SatelliteNumber> GetAllPrns()
        {
            var allPrns = new List<SatelliteNumber>();
            allPrns.AddRange(DefaultGpsPrns);
            allPrns.AddRange(DefaultBeidouPrns);
            allPrns.AddRange(GetPrns(Gdp.SatelliteType.R, 32));
            allPrns.AddRange(GetPrns(Gdp.SatelliteType.E, 36));
            return allPrns;
        }
        /// <summary>
        /// 获取一个系统指定的连续的卫星序列，编号默认从1 开始到指定的数字。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static List<SatelliteNumber> GetPrns(SatelliteType type, int count, int from = 1)
        {
            List<SatelliteNumber> prns = new List<SatelliteNumber>();
            int max = count + from;
            for (int i = from; i < max; i++)
            {
                prns.Add(new SatelliteNumber(i, type));
            }
            return prns;
        }
        /// <summary>
        /// 返回类型
        /// </summary>
        /// <param name="prns"></param>
        /// <returns></returns>
        static public List<SatelliteType> GetSatTypes(IEnumerable<SatelliteNumber> prns)
        {
            return prns.Select(m => m.SatelliteType).Distinct().OrderBy(m => m).ToList();
        }
        #endregion

    }
}
