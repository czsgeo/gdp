//2018.07.18, czs, edit in HMX, 正式大量启用 ObservationCode


using Gdp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// GNSSer 内部码类型(CA, P, D, L),一个载波频率分为多个码传输数据，或有多种类型的数据。
    /// 主要用于表征精度信息,与 RINEX 2.0 相容。
    /// </summary>
    public enum GnssCodeType
    {
        /// <summary>
        /// 未指定，或未知。
        /// </summary>
        UnKnown,
        /// <summary>
        /// 伪距码，粗捕获码，民用码。
        /// </summary>
        CA,
        /// <summary>
        /// 伪距码，军用码，精码
        /// </summary>
        P,
        /// <summary>
        /// 多普勒频率观测值
        /// </summary>
        D,
        /// <summary>
        /// 载波相位观测值
        /// </summary>
        L
    }
    /// <summary>
    /// RINEX 观测值类型。observation type C、 L、 D、 S
    /// t :observation type C = pseudorange, L = carrier phase, D = doppler, S = signal    strength)
    /// </summary>
    public enum ObservationType
    {
        /// <summary>
        /// Unkonwn
        /// </summary>
        U = 0,
        /// <summary>
        /// X
        /// </summary>
        X = 1,
        /// <summary>
        /// = pseudorange
        /// </summary>
        C,
        /// <summary>
        /// = carrier phase
        /// </summary>
        L,
        /// <summary>
        /// doppler
        /// </summary>
        D,
        /// <summary>
        /// signal strength
        /// </summary>
        S
    }
    /// <summary>
    /// 帮助算法
    /// </summary>
    public class ObservationTypeHelper
    {
        /// <summary>
        /// 获取GNSS类型
        /// </summary>
        /// <param name="obsCode"></param>
        /// <returns></returns>
        public static GnssCodeType GetGnssCodeType(ObservationCode obsCode)
        {
            switch (obsCode.ObservationType)
            {
                case ObservationType.C:
                    if (obsCode.Attribute == "C") { return GnssCodeType.CA; }
                    return GnssCodeType.P;
                case ObservationType.L:
                    return GnssCodeType.L;
                case ObservationType.D:
                    return GnssCodeType.D; 
                default:
                    return GnssCodeType.UnKnown; 
            }
            return GnssCodeType.CA;
        }
        public static GnssCodeType GetGnssCodeType(string str)
        {
            var first = str.Trim().Substring(0, 1);
            if (first == "P")
            {
                return GnssCodeType.P;
            }
            if (first == "C")
            {
                return GnssCodeType.CA;
            }
            if (first == "L")
            {
                return GnssCodeType.L;
            }
            if (first == "D")
            {
                return GnssCodeType.D;
            }
            return GnssCodeType.UnKnown;
        }
        /// <summary>
        /// RINEX 转换为观测码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ObservationType RinexCodeToObservationType(string str)
        {
            var first = str.Trim().Substring(0, 1);
            if (first == "P" || first == "C")
            {
                return ObservationType.C;
            }

            return EnumUtil.TryParse<ObservationType>( first, ObservationType.U);
        }
    }


    /// The new signal structures for GPS and Galileo make it possible to generate obsCode and phase observations
    ///based on one or a combination of several channels: Two-channel signals are composed of I and Q components,
    ///three-channel signals of A, B, and C components. Moreover a wideband tracking of a combined
    ///E5a + E5b frequency tracking is possible. In order to keep the observation codes short but still allow for a
    ///detailed characterization of the actual signal generation the length of the codes is increased from dayServices (Version
    ///1 and 2) to three by adding a signal generation attribute:
    ///The observation obsCode tna consists of three parts:
    ///- t : observation type: C = pseudorange, L = carrier phase, D = doppler, S = signal strength)
    ///- n : band / frequency: 1, 2,…,8
    ///- a : attribute: tracking mode or channel, e.g., I, Q, etc
    ///Examples:
    ///- L1C: C/A obsCode-derived L1 carrier phase (GPS, GLONASS)
    ///Carrier phase on E2-L1-E1 derived from C channel (Galileo)
    ///- C2L: L2C pseudorange derived from the L channel (GPS)

    /// <summary>
    /// RINEX 2- 3.02 观测值的组合。
    /// </summary>
    public class ObservationCode
    {
        /// <summary>
        /// 构造函数。创建一个观测组合类型。
        /// </summary>
        /// <param name="code">观测值类型</param>  
        public ObservationCode(string code)
        {
            var c = Parse(code);
            this.ObservationType = c.ObservationType;
            this.BandOrFrequency = c.BandOrFrequency;
            this.Attribute = c.Attribute;
        }
        /// <summary>
        /// 构造函数。创建一个观测组合类型。
        /// </summary>
        /// <param name="observationType">观测值类型</param>
        /// <param name="bandOrFrequency">波段或频率</param>
        /// <param name="attribute">属性</param>
        public ObservationCode(ObservationType observationType, int bandOrFrequency, string attribute = null)
        {
            this.ObservationType = observationType;
            this.BandOrFrequency = bandOrFrequency;
            if (String.IsNullOrWhiteSpace(attribute))
            {
                attribute = "C";
            }

            this.Attribute = attribute;
        }

        /// <summary>
        /// 观测值类型。 C、 L、 D、 S
        /// </summary>
        public ObservationType ObservationType { get; set; }
        /// <summary>
        /// 波段或频率
        /// </summary>
        public int BandOrFrequency { get; set; }
        /// <summary>
        /// attribute，属性。
        /// </summary>
        public string Attribute { get; set; }
        /// <summary>
        /// P码的默认属性标识
        /// </summary>
        public const String DefaultAttributeOfP = "W";
        /// <summary>
        /// 获取RINEX观测码
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public string GetRinexCode(double version)
        {
            var rinexCodeV3 = ToString();
            if (version < 3)
            {
                if (this.ObservationType == ObservationType.C)
                {
                    if (Attribute == DefaultAttributeOfP || Attribute == "P" || Attribute == "X" || Attribute == "M")
                    {
                        return "P" + BandOrFrequency;
                    }
                }
                return rinexCodeV3.Substring(0, 2);
            }

            return rinexCodeV3;
        }


        #region override
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ObservationCode)) return false;
            ObservationCode o = (ObservationCode)obj;

            return BandOrFrequency == o.BandOrFrequency
                && Attribute == o.Attribute
                && ObservationType == o.ObservationType;
        }
        public static List<ObservationCode> GetObsCodes(List<string> codes)
        {
            var result = new List<ObservationCode>();

            foreach (var item in codes)
            {
                result.Add(new ObservationCode(item));
            }
            return result;
        }
        /// <summary>
        /// V2 转换为 V3 格式，列表。
        /// </summary>
        /// <param name="codeV2"></param>
        /// <returns></returns>
        public static List<string> GetCodesV3(string codeV2)
        {
            if (codeV2.Length <= 1)
            {
                throw new Exception("RINEX 代码长度小于 2 ！");
            }

            if (codeV2.Length == 3) { return new List<string>() { codeV2 }; }

            var type = ObservationTypeHelper.RinexCodeToObservationType(codeV2);
            int num = Gdp.Utils.StringUtil.GetNumber(codeV2);

            var twoCode = type + "" + num.ToString().Substring(0, 1);
            return new List<string>()
            {
                twoCode + "C",
                twoCode + "W",
                twoCode + "X",
                twoCode + "P",
            };
        }

        /// <summary>
        /// 哈希数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return BandOrFrequency.GetHashCode() ^ 7 + Attribute.GetHashCode() ^ 3 + ObservationType.GetHashCode() * 3;
        }

        /// <summary>
        /// keyPrev
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ObservationType.ToString() +
            this.BandOrFrequency + "" +
            this.Attribute;
        }

        #endregion

        /// <summary>
        /// 解析字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public ObservationCode Parse(string str)
        {
            ObservationType type = ObservationTypeHelper.RinexCodeToObservationType(str);
            //(ObservationType)Enum.Parse(typeof(ObservationType), str.Substring(0, 1), true);
            int band = int.Parse(str.Trim().Substring(1, 1));
            string attribute = null;
            //三个则直接提取
            if (str.Length > 2) { attribute = str.Substring(2, 1); }
            else if (type == ObservationType.C)//否则转换P1，P2为C1X,C2W
            {
                var fisrtChar = str.Trim().Substring(0, 1);
                if (fisrtChar == "P")
                {
                    attribute = "W";
                }
                else
                {
                    attribute = "C";
                }
            }

            return new ObservationCode(type, band, attribute);
        }
        /// <summary>
        /// CA 码修改为P码
        /// </summary>
        /// <param name="rinexCode"></param>
        /// <returns></returns>
        internal static void ChagngeCaToP(ref ObservationCode rinexCode)
        {
            rinexCode.Attribute = DefaultAttributeOfP;
        }

        #region 常用
        /// <summary>
        /// 空置
        /// </summary>
        public static ObservationCode Empty { get { return ObservationCode.Parse("S00"); } }
        /// <summary>
        /// L1C: C/A obsCode-derived L1 carrier phase (GPS, GLONASS)
        /// </summary>
        public static ObservationCode L1C { get { return ObservationCode.Parse("L1C"); } }

        public static ObservationCode C1C { get { return ObservationCode.Parse("C1C"); } }

        public static ObservationCode D1C { get { return ObservationCode.Parse("D1C"); } }
        public static ObservationCode S1C { get { return ObservationCode.Parse("S1C"); } }
        public static ObservationCode C1S { get { return ObservationCode.Parse("C1S"); } }
        public static ObservationCode L1S { get { return ObservationCode.Parse("L1S"); } }
        public static ObservationCode D1S { get { return ObservationCode.Parse("D1S"); } }
        public static ObservationCode S1S { get { return ObservationCode.Parse("S1S"); } }
        public static ObservationCode C1L { get { return ObservationCode.Parse("C1L"); } }
        public static ObservationCode L1L { get { return ObservationCode.Parse("L1L"); } }
        public static ObservationCode D1L { get { return ObservationCode.Parse("D1L"); } }
        public static ObservationCode S1L { get { return ObservationCode.Parse("S1L"); } }
        public static ObservationCode C1X { get { return ObservationCode.Parse("C1X"); } }
        public static ObservationCode L1X { get { return ObservationCode.Parse("L1X"); } }
        public static ObservationCode D1X { get { return ObservationCode.Parse("D1X"); } }
        public static ObservationCode S1X { get { return ObservationCode.Parse("S1X"); } }
        public static ObservationCode C1P { get { return ObservationCode.Parse("C1P"); } }
        public static ObservationCode L1P { get { return ObservationCode.Parse("L1P"); } }
        public static ObservationCode D1P { get { return ObservationCode.Parse("D1P"); } }
        public static ObservationCode S1P { get { return ObservationCode.Parse("S1P"); } }
        public static ObservationCode C1W { get { return ObservationCode.Parse("C1W"); } }
        public static ObservationCode L1W { get { return ObservationCode.Parse("L1W"); } }
        public static ObservationCode D1W { get { return ObservationCode.Parse("D1W"); } }
        public static ObservationCode S1W { get { return ObservationCode.Parse("S1W"); } }
        public static ObservationCode C1Y { get { return ObservationCode.Parse("C1Y"); } }
        public static ObservationCode L1Y { get { return ObservationCode.Parse("L1Y"); } }
        public static ObservationCode D1Y { get { return ObservationCode.Parse("D1Y"); } }
        public static ObservationCode S1Y { get { return ObservationCode.Parse("S1Y"); } }
        public static ObservationCode C1M { get { return ObservationCode.Parse("C1M"); } }
        public static ObservationCode L1M { get { return ObservationCode.Parse("L1M"); } }
        public static ObservationCode D1M { get { return ObservationCode.Parse("D1M"); } }
        public static ObservationCode S1M { get { return ObservationCode.Parse("S1M"); } }
        public static ObservationCode L1N { get { return ObservationCode.Parse("L1N"); } }
        public static ObservationCode D1N { get { return ObservationCode.Parse("D1N"); } }
        public static ObservationCode S1N { get { return ObservationCode.Parse("S1N"); } }
        public static ObservationCode C2C { get { return ObservationCode.Parse("C2C"); } }
        public static ObservationCode L2C { get { return ObservationCode.Parse("L2C"); } }
        public static ObservationCode D2C { get { return ObservationCode.Parse("D2C"); } }
        public static ObservationCode S2C { get { return ObservationCode.Parse("S2C"); } }
        public static ObservationCode C2D { get { return ObservationCode.Parse("C2D"); } }
        public static ObservationCode L2D { get { return ObservationCode.Parse("L2D"); } }
        public static ObservationCode D2D { get { return ObservationCode.Parse("D2D"); } }
        public static ObservationCode S2D { get { return ObservationCode.Parse("S2D"); } }
        public static ObservationCode C2S { get { return ObservationCode.Parse("C2S"); } }
        public static ObservationCode L2S { get { return ObservationCode.Parse("L2S"); } }
        public static ObservationCode D2S { get { return ObservationCode.Parse("D2S"); } }
        public static ObservationCode S2S { get { return ObservationCode.Parse("S2S"); } }
        /// <summary>
        ///  C2L: L2C pseudorange derived from the L channel (GPS)
        /// </summary>
        public static ObservationCode C2L { get { return ObservationCode.Parse("C2L"); } }
        public static ObservationCode L2L { get { return ObservationCode.Parse("L2L"); } }
        public static ObservationCode D2L { get { return ObservationCode.Parse("D2L"); } }
        public static ObservationCode S2L { get { return ObservationCode.Parse("S2L"); } }
        public static ObservationCode C2X { get { return ObservationCode.Parse("C2X"); } }
        public static ObservationCode L2X { get { return ObservationCode.Parse("L2X"); } }
        public static ObservationCode D2X { get { return ObservationCode.Parse("D2X"); } }
        public static ObservationCode S2X { get { return ObservationCode.Parse("S2X"); } }
        public static ObservationCode C2P { get { return ObservationCode.Parse("C2P"); } }
        public static ObservationCode L2P { get { return ObservationCode.Parse("L2P"); } }
        public static ObservationCode D2P { get { return ObservationCode.Parse("D2P"); } }
        public static ObservationCode S2P { get { return ObservationCode.Parse("S2P"); } }
        public static ObservationCode C2W { get { return ObservationCode.Parse("C2W"); } }
        public static ObservationCode L2W { get { return ObservationCode.Parse("L2W"); } }
        public static ObservationCode D2W { get { return ObservationCode.Parse("D2W"); } }
        public static ObservationCode S2W { get { return ObservationCode.Parse("S2W"); } }
        public static ObservationCode C2Y { get { return ObservationCode.Parse("C2Y"); } }
        public static ObservationCode L2Y { get { return ObservationCode.Parse("L2Y"); } }
        public static ObservationCode D2Y { get { return ObservationCode.Parse("D2Y"); } }
        public static ObservationCode S2Y { get { return ObservationCode.Parse("S2Y"); } }
        public static ObservationCode C2M { get { return ObservationCode.Parse("C2M"); } }
        public static ObservationCode L2M { get { return ObservationCode.Parse("L2M"); } }
        public static ObservationCode D2M { get { return ObservationCode.Parse("D2M"); } }
        public static ObservationCode S2M { get { return ObservationCode.Parse("S2M"); } }
        public static ObservationCode L2N { get { return ObservationCode.Parse("L2N"); } }
        public static ObservationCode D2N { get { return ObservationCode.Parse("D2N"); } }
        public static ObservationCode S2N { get { return ObservationCode.Parse("S2N"); } }


        public static ObservationCode C5I { get { return ObservationCode.Parse("C5I"); } }
        public static ObservationCode L5I { get { return ObservationCode.Parse("L5I"); } }
        public static ObservationCode D5I { get { return ObservationCode.Parse("D5I"); } }
        public static ObservationCode S5I { get { return ObservationCode.Parse("S5I"); } }
        public static ObservationCode C5Q { get { return ObservationCode.Parse("C5Q"); } }
        public static ObservationCode L5Q { get { return ObservationCode.Parse("L5Q"); } }
        public static ObservationCode D5Q { get { return ObservationCode.Parse("D5Q"); } }
        public static ObservationCode S5Q { get { return ObservationCode.Parse("S5Q"); } }
        public static ObservationCode C5X { get { return ObservationCode.Parse("C5X"); } }
        public static ObservationCode L5X { get { return ObservationCode.Parse("L5X"); } }
        public static ObservationCode D5X { get { return ObservationCode.Parse("D5X"); } }
        public static ObservationCode S5X { get { return ObservationCode.Parse("S5X"); } }
        //public static ObservationCode C1C { get { return ObservationCode.Parse("C1C"); } }
        //public static ObservationCode L1C { get { return ObservationCode.Parse("L1C"); } }
        //public static ObservationCode D1C { get { return ObservationCode.Parse("D1C"); } }
        //public static ObservationCode S1C { get { return ObservationCode.Parse("S1C"); } }
        //public static ObservationCode C1P { get { return ObservationCode.Parse("C1P"); } }
        //public static ObservationCode L1P { get { return ObservationCode.Parse("L1P"); } }
        //public static ObservationCode D1P { get { return ObservationCode.Parse("D1P"); } }
        //public static ObservationCode S1P { get { return ObservationCode.Parse("S1P"); } }
        //public static ObservationCode C2C { get { return ObservationCode.Parse("C2C"); } }
        //public static ObservationCode L2C { get { return ObservationCode.Parse("L2C"); } }
        //public static ObservationCode D2C { get { return ObservationCode.Parse("D2C"); } }
        //public static ObservationCode S2C { get { return ObservationCode.Parse("S2C"); } }
        //public static ObservationCode C2P { get { return ObservationCode.Parse("C2P"); } }
        //public static ObservationCode L2P { get { return ObservationCode.Parse("L2P"); } }
        //public static ObservationCode D2P { get { return ObservationCode.Parse("D2P"); } }
        //public static ObservationCode S2P { get { return ObservationCode.Parse("S2P"); } }
        public static ObservationCode C1A { get { return ObservationCode.Parse("C1A"); } }
        public static ObservationCode L1A { get { return ObservationCode.Parse("L1A"); } }
        public static ObservationCode D1A { get { return ObservationCode.Parse("D1A"); } }
        public static ObservationCode S1A { get { return ObservationCode.Parse("S1A"); } }
        public static ObservationCode C1B { get { return ObservationCode.Parse("C1B"); } }
        public static ObservationCode L1B { get { return ObservationCode.Parse("L1B"); } }
        public static ObservationCode D1B { get { return ObservationCode.Parse("D1B"); } }
        public static ObservationCode S1B { get { return ObservationCode.Parse("S1B"); } }
        //public static ObservationCode C1C { get { return ObservationCode.Parse("C1C"); } }
        //public static ObservationCode L1C { get { return ObservationCode.Parse("L1C"); } }
        //public static ObservationCode D1C { get { return ObservationCode.Parse("D1C"); } }
        //public static ObservationCode S1C { get { return ObservationCode.Parse("S1C"); } }
        //public static ObservationCode C1X { get { return ObservationCode.Parse("C1X"); } }
        //public static ObservationCode L1X { get { return ObservationCode.Parse("L1X"); } }
        //public static ObservationCode D1X { get { return ObservationCode.Parse("D1X"); } }
        //public static ObservationCode S1X { get { return ObservationCode.Parse("S1X"); } }
        public static ObservationCode C1Z { get { return ObservationCode.Parse("C1Z"); } }
        public static ObservationCode L1Z { get { return ObservationCode.Parse("L1Z"); } }
        public static ObservationCode D1Z { get { return ObservationCode.Parse("D1Z"); } }
        public static ObservationCode S1Z { get { return ObservationCode.Parse("S1Z"); } }
        //public static ObservationCode C5I { get { return ObservationCode.Parse("C5I"); } }
        //public static ObservationCode L5I { get { return ObservationCode.Parse("L5I"); } }
        //public static ObservationCode D5I { get { return ObservationCode.Parse("D5I"); } }
        //public static ObservationCode S5I { get { return ObservationCode.Parse("S5I"); } }
        //public static ObservationCode C5Q { get { return ObservationCode.Parse("C5Q"); } }
        //public static ObservationCode L5Q { get { return ObservationCode.Parse("L5Q"); } }
        //public static ObservationCode D5Q { get { return ObservationCode.Parse("D5Q"); } }
        //public static ObservationCode S5Q { get { return ObservationCode.Parse("S5Q"); } }
        //public static ObservationCode C5X { get { return ObservationCode.Parse("C5X"); } }
        //public static ObservationCode L5X { get { return ObservationCode.Parse("L5X"); } }
        //public static ObservationCode D5X { get { return ObservationCode.Parse("D5X"); } }
        //public static ObservationCode S5X { get { return ObservationCode.Parse("S5X"); } }
        public static ObservationCode C7I { get { return ObservationCode.Parse("C7I"); } }
        public static ObservationCode L7I { get { return ObservationCode.Parse("L7I"); } }
        public static ObservationCode D7I { get { return ObservationCode.Parse("D7I"); } }
        public static ObservationCode S7I { get { return ObservationCode.Parse("S7I"); } }
        public static ObservationCode C7Q { get { return ObservationCode.Parse("C7Q"); } }
        public static ObservationCode L7Q { get { return ObservationCode.Parse("L7Q"); } }
        public static ObservationCode D7Q { get { return ObservationCode.Parse("D7Q"); } }
        public static ObservationCode S7Q { get { return ObservationCode.Parse("S7Q"); } }
        public static ObservationCode C7X { get { return ObservationCode.Parse("C7X"); } }
        public static ObservationCode L7X { get { return ObservationCode.Parse("L7X"); } }
        public static ObservationCode D7X { get { return ObservationCode.Parse("D7X"); } }
        public static ObservationCode S7X { get { return ObservationCode.Parse("S7X"); } }
        public static ObservationCode C8I { get { return ObservationCode.Parse("C8I"); } }
        public static ObservationCode L8I { get { return ObservationCode.Parse("L8I"); } }
        public static ObservationCode D8I { get { return ObservationCode.Parse("D8I"); } }
        public static ObservationCode S8I { get { return ObservationCode.Parse("S8I"); } }
        public static ObservationCode C8Q { get { return ObservationCode.Parse("C8Q"); } }
        public static ObservationCode L8Q { get { return ObservationCode.Parse("L8Q"); } }
        public static ObservationCode D8Q { get { return ObservationCode.Parse("D8Q"); } }
        public static ObservationCode S8Q { get { return ObservationCode.Parse("S8Q"); } }
        public static ObservationCode C8X { get { return ObservationCode.Parse("C8X"); } }
        public static ObservationCode L8X { get { return ObservationCode.Parse("L8X"); } }
        public static ObservationCode D8X { get { return ObservationCode.Parse("D8X"); } }
        public static ObservationCode S8X { get { return ObservationCode.Parse("S8X"); } }
        public static ObservationCode C6A { get { return ObservationCode.Parse("C6A"); } }
        public static ObservationCode L6A { get { return ObservationCode.Parse("L6A"); } }
        public static ObservationCode D6A { get { return ObservationCode.Parse("D6A"); } }
        public static ObservationCode S6A { get { return ObservationCode.Parse("S6A"); } }
        public static ObservationCode C6B { get { return ObservationCode.Parse("C6B"); } }
        public static ObservationCode L6B { get { return ObservationCode.Parse("L6B"); } }
        public static ObservationCode D6B { get { return ObservationCode.Parse("D6B"); } }
        public static ObservationCode S6B { get { return ObservationCode.Parse("S6B"); } }
        public static ObservationCode C6C { get { return ObservationCode.Parse("C6C"); } }
        public static ObservationCode L6C { get { return ObservationCode.Parse("L6C"); } }
        public static ObservationCode D6C { get { return ObservationCode.Parse("D6C"); } }
        public static ObservationCode S6C { get { return ObservationCode.Parse("S6C"); } }
        public static ObservationCode C6X { get { return ObservationCode.Parse("C6X"); } }
        public static ObservationCode L6X { get { return ObservationCode.Parse("L6X"); } }
        public static ObservationCode D6X { get { return ObservationCode.Parse("D6X"); } }
        public static ObservationCode S6X { get { return ObservationCode.Parse("S6X"); } }
        public static ObservationCode C6Z { get { return ObservationCode.Parse("C6Z"); } }
        public static ObservationCode L6Z { get { return ObservationCode.Parse("L6Z"); } }
        public static ObservationCode D6Z { get { return ObservationCode.Parse("D6Z"); } }
        public static ObservationCode S6Z { get { return ObservationCode.Parse("S6Z"); } }
        //public static ObservationCode C1C { get { return ObservationCode.Parse("C1C"); } }
        //public static ObservationCode L1C { get { return ObservationCode.Parse("L1C"); } }
        //public static ObservationCode D1C { get { return ObservationCode.Parse("D1C"); } }
        //public static ObservationCode S1C { get { return ObservationCode.Parse("S1C"); } }
        //public static ObservationCode C5I { get { return ObservationCode.Parse("C5I"); } }
        //public static ObservationCode L5I { get { return ObservationCode.Parse("L5I"); } }
        //public static ObservationCode D5I { get { return ObservationCode.Parse("D5I"); } }
        //public static ObservationCode S5I { get { return ObservationCode.Parse("S5I"); } }
        //public static ObservationCode C5Q { get { return ObservationCode.Parse("C5Q"); } }
        //public static ObservationCode L5Q { get { return ObservationCode.Parse("L5Q"); } }
        //public static ObservationCode D5Q { get { return ObservationCode.Parse("D5Q"); } }
        //public static ObservationCode S5Q { get { return ObservationCode.Parse("S5Q"); } }
        //public static ObservationCode C5X { get { return ObservationCode.Parse("C5X"); } }
        //public static ObservationCode L5X { get { return ObservationCode.Parse("L5X"); } }
        //public static ObservationCode D5X { get { return ObservationCode.Parse("D5X"); } }
        //public static ObservationCode S5X { get { return ObservationCode.Parse("S5X"); } }
        public static ObservationCode C2I { get { return ObservationCode.Parse("C2I"); } }
        public static ObservationCode L2I { get { return ObservationCode.Parse("L2I"); } }
        public static ObservationCode D2I { get { return ObservationCode.Parse("D2I"); } }
        public static ObservationCode S2I { get { return ObservationCode.Parse("S2I"); } }
        public static ObservationCode C2Q { get { return ObservationCode.Parse("C2Q"); } }
        public static ObservationCode L2Q { get { return ObservationCode.Parse("L2Q"); } }
        public static ObservationCode D2Q { get { return ObservationCode.Parse("D2Q"); } }
        public static ObservationCode S2Q { get { return ObservationCode.Parse("S2Q"); } }
        //public static ObservationCode C2X { get { return ObservationCode.Parse("C2X"); } }
        //public static ObservationCode L2X { get { return ObservationCode.Parse("L2X"); } }
        //public static ObservationCode D2X { get { return ObservationCode.Parse("D2X"); } }
        //public static ObservationCode S2X { get { return ObservationCode.Parse("S2X"); } } 
        //public static ObservationCode C7I { get { return ObservationCode.Parse("C7I"); } }
        //public static ObservationCode L7I { get { return ObservationCode.Parse("L7I"); } }
        //public static ObservationCode D7I { get { return ObservationCode.Parse("D7I"); } }
        //public static ObservationCode S7I { get { return ObservationCode.Parse("S7I"); } }
        //public static ObservationCode C7Q { get { return ObservationCode.Parse("C7Q"); } }
        //public static ObservationCode L7Q { get { return ObservationCode.Parse("L7Q"); } }
        //public static ObservationCode D7Q { get { return ObservationCode.Parse("D7Q"); } }
        //public static ObservationCode S7Q { get { return ObservationCode.Parse("S7Q"); } }
        //public static ObservationCode C7X { get { return ObservationCode.Parse("C7X"); } }
        //public static ObservationCode L7X { get { return ObservationCode.Parse("L7X"); } }
        //public static ObservationCode D7X { get { return ObservationCode.Parse("D7X"); } }
        //public static ObservationCode S7X { get { return ObservationCode.Parse("S7X"); } }
        public static ObservationCode C6I { get { return ObservationCode.Parse("C6I"); } }
        public static ObservationCode L6I { get { return ObservationCode.Parse("L6I"); } }
        public static ObservationCode D6I { get { return ObservationCode.Parse("D6I"); } }
        public static ObservationCode S6I { get { return ObservationCode.Parse("S6I"); } }
        public static ObservationCode C6Q { get { return ObservationCode.Parse("C6Q"); } }
        public static ObservationCode L6Q { get { return ObservationCode.Parse("L6Q"); } }
        public static ObservationCode D6Q { get { return ObservationCode.Parse("D6Q"); } }
        public static ObservationCode S6Q { get { return ObservationCode.Parse("S6Q"); } }
        //public static ObservationCode C6X { get { return ObservationCode.Parse("C6X"); } }
        //public static ObservationCode L6X { get { return ObservationCode.Parse("L6X"); } }
        //public static ObservationCode D6X { get { return ObservationCode.Parse("D6X"); } }
        //public static ObservationCode S6X { get { return ObservationCode.Parse("S6X"); } }

        #endregion

    }

}
