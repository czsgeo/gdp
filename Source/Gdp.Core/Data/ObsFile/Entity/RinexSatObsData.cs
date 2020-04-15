//2014.06.24, czs, edit, 成为 GnssDataType 的遍历器
//2014.08.19, czs, edit, 格式修改，微调
//2015.05.09, czs, edit in namu, 增加移除观测类型的方法，类名 ObservationData 修改为 SatObsData 
//2018.07.18, czs, edit in HMX, 采用ObservationCode进行转换
//2018.09.08, czs, edit in hmx, 增加移除指定频率编号

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gdp.Utils;

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// 具有时间的历元数据。
    /// 存储一个卫星的观测值记录
    /// </summary>
    public class TimedRinexSatObsData 
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TimedRinexSatObsData() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="data"></param>
        public TimedRinexSatObsData(Time Time , RinexSatObsData data) {
            this.Time = Time;
            this.SatObsData = data;
        }
        /// <summary>
        /// 观测记录
        /// </summary>
        public RinexSatObsData SatObsData { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public Time Time { get; set; }
    }
    
    /// <summary>
    /// 存储一个卫星的观测值记录。
    /// 直接读取自RINEX文件，是原始的数据，不应该出现在计算当中。
    /// 是观测数据类型的遍历器。
    /// </summary>
    public class RinexSatObsData : IEnumerable<KeyValuePair<string, RinexObsValue>>
    {
        /// <summary>
        /// 初始化变量。
        /// </summary>
        public RinexSatObsData()
        {
            _values = new Dictionary<string, RinexObsValue>();
            RinexVersion = 3.02;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Prn"></param>
        public RinexSatObsData(SatelliteNumber Prn):this()
        {
            this.Prn = Prn;
            RinexVersion = 3.02;
        }

        /// <summary>
        /// RIENX 文件版本，或者直接引入 Header ？？？ //2018.09.25,czs, hmx
        /// </summary>
        public double RinexVersion { get; set; }
        #region 变量，属性
        public Time ReciverTime { get; set; }
        /// <summary>
        /// 观测数据原始记录。
        /// </summary>
        private Dictionary<string, RinexObsValue> _values;
        /// <summary>
        /// 卫星编号
        /// </summary>
        public SatelliteNumber Prn { get; set; }
        List<string> obsTypes = null;
        /// <summary>
        /// 观测类型
        /// </summary>
        public List<string> ObsTypes { get { if (obsTypes == null) obsTypes =new List<string>(_values.Keys); return obsTypes; } }

        /// <summary>
        /// 检索器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RinexObsValue this[string type] { get { return this._values[type]; } set { this._values[type] = value; } }
        #endregion
        /// <summary>
        /// L1 返回第一个匹配非0结果, 无则为null
        /// </summary>
        public RinexObsValue PhaseA { get { return GetRinexObsValue(FrequenceType.A, "L"); } }

        /// <summary>
        /// L2 返回第一个匹配非0结果, 无则为null
        /// </summary>
        public RinexObsValue PhaseB { get { return GetRinexObsValue(FrequenceType.B, "L"); } }

        /// <summary>
        /// L3 返回第一个匹配非0结果, 无则为null
        /// </summary>
        public RinexObsValue PhaseC { get { return GetRinexObsValue(FrequenceType.C, "L"); } }


        /// <summary>
        /// P1 or C1 返回第一个匹配非0结果, 无则为null
        /// </summary>
        public RinexObsValue RangeA { get { return GetRinexObsValue(FrequenceType.A, "P","C"); } }

        /// <summary> 
        /// P2 or C2 返回第一个匹配非0结果, 无则为null
        /// </summary>
        public RinexObsValue RangeB { get { return GetRinexObsValue(FrequenceType.B, "P", "C"); } }

        /// <summary> 
        /// P3 or C3 返回第一个匹配非0结果, 无则为null
        /// </summary>
        public RinexObsValue RangeC { get { return GetRinexObsValue(FrequenceType.C, "P", "C"); } }


        public RinexObsValue RangeA_CA => GetRinexObsValue(FrequenceType.A, "C");
        public RinexObsValue RangeA_P => GetRinexObsValue(FrequenceType.A, "P"); 

        public RinexObsValue FirstAvailable { get{
                if (PhaseA != null)
                {
                    return PhaseA; 
                }
                return PhaseB;
            }
        }
        /// <summary>
        /// 获取，优先匹配第一个
        /// </summary>
        /// <param name="frequenceType"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private RinexObsValue GetRinexObsValue(FrequenceType frequenceType, params string [] prefix)
        {
            List<int> builedFreq = ObsCodeConvert.GetRinexFrequenceNumber(Prn.SatelliteType, frequenceType);
            foreach (var item in this)
            {
                foreach (var freq in builedFreq)
                {
                    foreach (var pre in prefix)
                    {
                        var keyCode = pre + freq;
                        if (item.Key.Contains(keyCode) && item.Value.Value != 0)
                        {
                            return item.Value;
                        }
                    }
                }
            }
            return null;
        }

        #region 方法
        /// <summary>
        /// 是否可以组成无电离层组合
        /// </summary>
        public bool IsIonoFreeCombinationAvaliable
        {
            get => PhaseA != null && PhaseA.Value != 0
                && PhaseB != null && PhaseB.Value != 0
                && RangeA != null && RangeA.Value != 0
                && RangeB != null && RangeB.Value != 0;
        }

        /// <summary>
        /// 移除对于双频无电离层组合多余的观测量
        /// </summary>
        public void RemoveRedundantObsForIonoFree()
        {
            List<int> freqA = ObsCodeConvert.GetRinexFrequenceNumber(Prn.SatelliteType, FrequenceType.A);
            List<int> freqB = ObsCodeConvert.GetRinexFrequenceNumber(Prn.SatelliteType, FrequenceType.B);
          
            List<int> total = new List<int>(freqA);
            total.AddRange(freqB);

            this.RemoveOtherFrequences(total);
        }


        /// <summary>
        /// 获取频率列表
        /// </summary>
        /// <returns></returns>
        public List<int> GetFrequenceNums()
        {
            List<int> result = new List<int>();
            foreach (var item in this)
            {
                int freqNum = item.Value.ObservationCode.BandOrFrequency;
                if (!result.Contains(freqNum)) { result.Add(freqNum); }
            }
            return result;
        }

        /// <summary>
        /// 获取同一频率的观测量集合。
        /// </summary>
        /// <param name="freqNum"></param>
        /// <param name="ignoreZero">忽略0值,节约内存</param>
        /// <returns></returns>
        public List<RinexObsValue> GetObservations(int freqNum, bool ignoreZero = true)
        {
            List<RinexObsValue> result = new List<RinexObsValue>();
            foreach (var item in this)
            {
                var obs = item.Value;
                if(ignoreZero && obs.Value == 0) { continue; }

                if (obs.ObservationCode.BandOrFrequency == freqNum)
                {
                   result.Add(obs); 
                }
            }
            return result;
        }

        /// <summary>
        /// 移除指定的观测量。
        /// </summary>
        /// <param name="type"></param>
        public void Remove(string type)
        {
            if (Contains(type))
            {
                this._values.Remove(type);
            }
        } 
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="types"></param>
        public void Remove(List<string> types)
        {
            foreach (var item in types)
            {
                Remove(item);
            } 
        } 
        /// <summary>
        /// 添加一个观测类型和值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void Add(string type, RinexObsValue val)
        {
            if(val != null && val.Value != 0)//可以避免，重复键错误
            {
                this._values[type] = val;
            }
        }
        /// <summary>
        /// 添加一个观测类型和值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public void Add(string type, double val)
        {
            this._values.Add(type, new RinexObsValue(val,type));
        }
        /// <summary>
        /// 顺序遍历，选择第一个非0值的代码编号.如果没有找到，则返回null。
        /// </summary>
        /// <param name="candidateCodes"></param>
        /// <returns></returns>
        public string GetFirstValuableCode(List<string> candidateCodes)
        {
            foreach (var item in candidateCodes)
            {
                if (TryGetValue(item) != 0)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// 获取记录值,如果没有该记录，则返回 0 .
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public double TryGetValue(string type)
        {
            var val = TryGetObsValue(type);
            if (val != null) { return val.Value; } 

            return 0; ;
        }
        /// <summary>
        /// 返回原始观测，若无返回 null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RinexObsValue TryGetObsValue(string type)
        {
            if(ObsTypes == null || ObsTypes.Count == 0) { return null; }

            if (Contains(type)) { return _values[type]; }

            //若代码小于2，而数据代码为 3，则需要匹配
            if (type.Length < 3 && this.ObsTypes[0].Length == 3)
            {
                var codes = ObservationCode.GetCodesV3(type);

                foreach (var c in codes)
                {
                    if (Contains(c)) { return _values[c]; }
                }
            }

            //如果输入为 3，而数据代码为 2， 则需减少一个代码
            var code = new ObservationCode(type);
            var codeV2 = code.GetRinexCode(2.01);
            if (Contains(codeV2)) { return _values[codeV2]; }

             
            return null; ;
        }

        /// <summary>
        /// 移除不包括的频率编号
        /// </summary>
        /// <param name="frequenceNumToRemained"></param>
        internal void RemoveOtherFrequences(List<int> frequenceNumToRemained)
        {
            List<string> teberemoved = new List<string>();
            foreach (var item in this)
            {
                if(!frequenceNumToRemained.Contains(item.Value.ObservationCode.BandOrFrequency)) 
                { 
                   teberemoved.Add(item.Key); 
                }
            }
            this.Remove(teberemoved);
        }
        /// <summary>
        /// 移除指定频率编号
        /// </summary>
        /// <param name="frequenceNumToBeRemoved"></param>
        internal void RemoveFrequences(List<int> frequenceNumToBeRemoved)
        {
            List<string> teberemoved = new List<string>();
            foreach (var item in this)
            {
                foreach (var freqNum in frequenceNumToBeRemoved)
                {
                    if (item.Key.Contains(freqNum.ToString()))
                    {
                        teberemoved.Add(item.Key);
                        break;
                    }
                } 
            }
            this.Remove(teberemoved);
        }

        /// <summary>
        /// 获取值，若第一个非0，则直接返回。
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public double TryGetValue(List<string> types)
        {
            double val = 0;
            foreach (var type in types)
            {
                val = TryGetValue(type);
                if (val != 0) break;
            }
            return val;
        }
        /// <summary>
        /// 是否包含数据类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public bool Contains(string type) { return _values.ContainsKey(type); }
         
        #endregion

        #region Override
        /// <summary>
        /// 遍历器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, RinexObsValue>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }
        #endregion

        /// <summary>
        /// 移除其它。
        /// </summary>
        /// <param name="list"></param>
        public void RemoveOthers(List<string> list)
        {
            foreach (var item in ObsTypes.ToArray())
            {
                if (!list.Contains(item))
                { this.Remove(item); }
            }
        }

        #region 扩展方法
        /// <summary>
        /// Geometry_free
        /// </summary> 
        public double GfValue
        {
            get
            { 
                return PhaseRangeA - PhaseRangeB; 
            }
        }


        /// <summary>
        /// MultiPath FrequenceA (m)
        /// </summary>
        public double Mp1Value
        {    
            get
            {
                double m12 = (FrequenceA.Value * FrequenceA.Value + FrequenceB.Value * FrequenceB.Value) / (FrequenceA.Value * FrequenceA.Value - FrequenceB.Value * FrequenceB.Value);
                return RangeA.Value - m12 * PhaseRangeA + (m12 - 1) * PhaseRangeB;
            }
        }


        /// <summary>
        /// MultiPath FrequenceB (m)
        /// </summary>
        public double Mp2Value
        {
            get
            {
                //double constAfa = FrequenceA.Value * FrequenceA.Value / (FrequenceB.Value * FrequenceB.Value);
                //return RangeB.Value + PhaseRangeA * 2 * constAfa / (1 - constAfa) - PhaseRangeB * (1 + constAfa) / (1 - constAfa);
                double m21 = ( FrequenceB.Value * FrequenceB.Value+ FrequenceA.Value * FrequenceA.Value) / (FrequenceB.Value * FrequenceB.Value - FrequenceA.Value * FrequenceA.Value);
                return RangeB.Value - m21 * PhaseRangeB + (m21 - 1) * PhaseRangeA;
            }
        }

        /// <summary>
        /// MultiPath FrequenceC (m)
        /// </summary>
        public double Mp3Value
        {
            get
            {
                double m31 = (FrequenceC.Value * FrequenceC.Value + FrequenceA.Value * FrequenceA.Value) / (FrequenceC.Value * FrequenceC.Value - FrequenceA.Value * FrequenceA.Value);
                return RangeC.Value - m31 * PhaseRangeC + (m31 - 1) * PhaseRangeA;

                //double m32 = (FrequenceC.Value * FrequenceC.Value + FrequenceB.Value * FrequenceB.Value) / (FrequenceC.Value * FrequenceC.Value - FrequenceB.Value * FrequenceB.Value);
                //return RangeC.Value - m32 * PhaseRangeC + (m32 - 1) * PhaseRangeB;
            }
        }


        public Frequence FrequenceA => Frequence.GetFrequenceA(this.Prn, this.ReciverTime);
        public Frequence FrequenceB => Frequence.GetFrequenceB(this.Prn, this.ReciverTime);

        public Frequence FrequenceC => Frequence.GetFrequenceC(this.Prn, this.ReciverTime);


        public double PhaseRangeA => FrequenceA.WaveLength * ((this.PhaseA==null)? 0:  this.PhaseA.Value);
        public double PhaseRangeB => FrequenceB.WaveLength * ((this.PhaseB == null) ? 0 : this.PhaseB.Value);
        public double PhaseRangeC => FrequenceC.WaveLength * ((this.PhaseC == null) ? 0 : this.PhaseC.Value);

        /// <summary>
        /// MW 周单位
        /// </summary>
        public double MwCycle => MwValue* Math.Abs(FrequenceA.Value - FrequenceB.Value) * 1E6 / GeoConst.LIGHT_SPEED;
        /// <summary>
        /// whether has gross error in range.
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <returns></returns>
        public bool IsRangeGrossError(double k1 = 30, double k2= 60)
        { 
            if(RangeA_CA != null && RangeA_P != null)
            {
                return Math.Abs(RangeA_CA.Value - RangeA_P.Value) > k1;
            }
            if(RangeA != null && RangeB != null)
            {
                return Math.Abs(RangeA.Value - RangeB.Value) > k2;
            }
            return false;
        }


        /// <summary>
        /// MW值
        /// </summary>
        public double MwValue
        {
            get
            {
                //EpochSatellite epochSat = EpochSat;
                var f1 = FrequenceA.Value;
                var f2 = FrequenceB.Value;
                if (this.RangeA == null || this.PhaseRangeA == 0 || this.RangeB == null || this.PhaseRangeB == 0)
                {
                    return Double.NaN;
                }
                double L1 = this.PhaseRangeA;
                double L2 = this.PhaseRangeB;
                //用原始的P1观测值
                double P1 = this.RangeA.Value; 
                //用原始的P2观测值
                double P2 = this.RangeB.Value;

                double value = GetMwValue(f1, f2, L1, L2, P1, P2);
                return value;

                //double freqVal = f1 - f2; //此处采用宽项的频率，以周为单位推导波长和频率

                //Frequence freqence = new Frequence("MW_" + epochSat.Prn.SatelliteType, freqVal);
                //return new PhaseCombination(value, freqence);
            }

        }
        public static double GetMwValue(double f1, double f2, double L1, double L2, double P1, double P2)
        {
            double e = f1 / (f1 - f2);
            double f = f2 / (f1 - f2);
            double c = f1 / (f1 + f2);
            double d = f2 / (f1 + f2);

            double value =
                e * L1
              - f * L2

              - c * P1
              - d * P2;
            return value;
        }
        #endregion
    }

}
