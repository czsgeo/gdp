//2016.04.18, czs, edit in hongqing, SiteObsInfo提取创建

using System;
using System.Collections.Generic;
using System.Text;
using System.IO; 
using Gdp.Utils;

using Gdp.Data.Rinex;

namespace Gdp
{

    /// <summary>
    /// 测站信息和观测信息
    /// </summary>
    public class SiteObsInfo : ISiteObsInfo
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SiteObsInfo()
        {
            this.ObsInfo = new ObsInfo();
            this.SiteInfo = new SiteInfo();
            this.Comments = new List<string>();
            this.Comments.Add("This file is created by Gdp.  www.gnsser.com"); 
            
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="SiteInfo"></param>
        /// <param name="ObsInfo"></param>
        public SiteObsInfo(ISiteInfo SiteInfo, IObsInfo ObsInfo)
        {
            this.ObsInfo = ObsInfo;
            this.SiteInfo = SiteInfo;
            this.Comments = new List<string>();
            this.Comments.Add("This file is created by Gdp.  www.gnsser.com"); 
        }

        #region 属性
        /// <summary>
        /// 测站名称 实际就是MarkerName
        /// </summary>
        public string SiteName { get { return SiteInfo.SiteName; } }
         
        /// <summary>
        /// 频率数量
        /// </summary>
        public int FrequenceCount { get; set; }

        /// <summary>
        /// 注释，可以用于Gdp 参数输出
        /// </summary>
        public List<string> Comments { get; set; } 
        /// <summary>
        /// 观测信息
        /// </summary>
        public IObsInfo ObsInfo { get; set; }
        /// <summary>
        /// 测站信息
        /// </summary>
        public ISiteInfo SiteInfo { get; set; }

        /// <summary>
        /// 卫星系统,实际的记录系统，只可以获取不可以修改。
        /// </summary>
        public List<SatelliteType> SatelliteTypes { get { return ObsInfo.SatelliteTypes; } }
        /// <summary>
        /// 卫星观测代码字典，如 G1：C1，L1。
        /// 本属性决定观测系统、观测码类型等，非常重要。
        /// </summary>
        public Dictionary<SatelliteType, List<string>> ObsCodes { get { return ObsInfo.ObsCodes; } set { ObsInfo.ObsCodes = value; } }

        private Dictionary<SatelliteType, List<ObservationCode>> obsTypeCodes;
      
        public Dictionary<SatelliteType, List<ObservationCode>> ObsTypeCodes
        {
            get
            {
                if (obsTypeCodes == null)
                {
                    obsTypeCodes = new Dictionary<SatelliteType, List<ObservationCode>>();
                    foreach (var item in ObsCodes)
                    {
                        var list = new List<ObservationCode>();
                        foreach (var val in item.Value)
                        {
                            list.Add(new ObservationCode(val));
                        }
                        obsTypeCodes[item.Key] = list;
                    }
                }

                return obsTypeCodes;
            }
        }
        #endregion



        #region 方法
        /// <summary>
        /// 返回可读的观测码
        /// </summary>
        /// <returns></returns>
        public string GetReadableObsCodes()
        {

            StringBuilder sb = new StringBuilder();
            int k = 0;
            foreach (var item in ObsCodes)
            {
                if (k != 0)
                {
                    sb.Append(";");
                }
                sb.Append(item.Key);
                sb.Append(": ");
                int i = 0;
                foreach (var val in item.Value)
                {
                    if(i != 0)
                    {
                        sb.Append(","); 
                    }
                    sb.Append(val);
                    i++;
                }
                k++;
            }
            return sb.ToString();
        }





        /// <summary>
        /// 获取卫星系统类型。就有一定的判断功能。
        /// </summary>
        /// <returns></returns>
        public SatelliteType GetSatTypeMarker()
        {
            if (ObsCodes.Count >= 2)
            {
                return SatelliteType.M;
            }

            // Count == 1
            foreach (var item in ObsCodes)
            {
                return item.Key;
            }
            //throw new Exception("没有卫星系统类型，不可能出现或者没有数据！");
            
            return SatelliteType.U;
        }

        /// <summary>
        /// 移除卫星系统及其所有的观测码。
        /// </summary>
        /// <param name="type"></param>
        public void Remove(SatelliteType type)
        {
            ObsCodes.Remove(type);
        }
        /// <summary>
        /// 移除卫星系统及其所有的观测码。
        /// </summary>
        /// <param name="types"></param>
        public void Remove(List<SatelliteType> types)
        {
            foreach (var type in types)
            {
                ObsCodes.Remove(type);
            }
        }
        /// <summary>
        /// 移除卫星系统及其所有的观测码。
        /// </summary>
        /// <param name="types"></param>
        public void RemoveOther(List<SatelliteType> types)
        {
            List<SatelliteType> other = new List<SatelliteType>();
            foreach (var type in this.SatelliteTypes)
            {
                if (!types.Contains(type))
                {
                    other.Add(type);
                }
            }
            Remove(other);
        }
        /// <summary>
        /// 移除其它观测类型，通过第一个字符判断。
        /// </summary>
        /// <param name="list"></param>
        internal void RemoveOther(List<ObsTypes> list)
        {
            List<string> codes = new List<string>();
            foreach (var item in list)
            {
                codes.Add(item.ToString());
            }
            foreach (var item in ObsCodes)
            {
                List<string> toRemove = new List<string>();
                foreach (var code in item.Value)
                {
                    var fisrtChar = code.Substring(0, 1);
                    if (!codes.Contains(fisrtChar))
                    {
                        toRemove.Add(code);
                    }
                }
                foreach (var code in toRemove)
                {
                    item.Value.Remove(code);
                }
            }
        }
        /// <summary>
        /// 设置特定卫星的观测类型
        /// </summary>
        /// <param name="SatelliteType"></param>
        /// <param name="typeOfObs"></param>
        public void SetTypeOfObserves(SatelliteType SatelliteType, List<string> typeOfObs) { ObsCodes[SatelliteType] = typeOfObs; }

        /// <summary>
        /// 移除对于双频无电离层组合多余的观测值
        /// </summary>
        public void IsRemoveRedundantObsForIonoFree()
        {
            Dictionary<SatelliteType, List<string>> tobeRemoeved = new Dictionary<SatelliteType, List<string>>();
            foreach (var item in ObsCodes)
            {
                var SatelliteType = item.Key;
                List<int> freqA = ObsCodeConvert.GetRinexFrequenceNumber(SatelliteType, FrequenceType.A);
                List<int> freqB = ObsCodeConvert.GetRinexFrequenceNumber(SatelliteType, FrequenceType.B);


                List<int> total = new List<int>(freqA);
                total.AddRange(freqB);

                var tobeRemoeved2  = new List<string>();
                foreach (var kv2 in item.Value)
                {
                    ObservationCode code = ObservationCode.Parse(kv2);
                    if (!total.Contains(code.BandOrFrequency) 
                        || code.ObservationType == ObservationType.D 
                        || code.ObservationType == ObservationType.S )
                    {
                        tobeRemoeved2.Add(kv2);
                    }  
                }
                tobeRemoeved[SatelliteType] = tobeRemoeved2;
            }

            foreach (var toremove in tobeRemoeved)
            {
                var SatelliteType = toremove.Key;
                var old = ObsCodes[SatelliteType];
                old.RemoveAll(item => toremove.Value.Contains(item));// = Geo.Utils.ListUtil.GetExcept(old, item.Value); 
            }
        }
        /// <summary>
        /// RINEX 2.x 的所有观测码相同。
        /// </summary>
        /// <returns></returns>
        public List<string> GetObsCodesV2()
        {        //如果没有Mix，则可能是rinex 2.0版本，直接赋予其它观测值给它。
            var list = new List<string>();
            if (ObsCodes.Count != 0)
            {
                //直接获取
                foreach (var item in ObsCodes)
                {
                    if (item.Value[0].Length == 2)
                    {
                        return new List<string>(item.Value);
                    }
                }


                //其它情况
                foreach (var item in ObsCodes)
                {
                    foreach (var type in item.Value)
                    {
                        var ty = new ObservationCode(type).GetRinexCode(2);
                        if (!list.Contains(ty))
                        {
                            list.Add(ty);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取指定系统的观测类型列表。返回引用，可以直接在上面添加数据。
        /// 如果没有，则查看M类型，将 Mixed 的类型赋予给它。
        /// </summary>
        /// <param name="satType"></param>
        /// <returns></returns>
        public List<string> GetOrInitObsCodes(SatelliteType satType)
        {
            if (!ObsCodes.ContainsKey(satType))
            {
                var list = new List<string>();
                if (ObsCodes.ContainsKey(SatelliteType.M))
                {
                    list = ObsCodes[SatelliteType.M];
                }


                ObsCodes[satType] = list;
            }

            return ObsCodes[satType];
        }
        public List<ObservationCode> GetOrInitObsTypeCodes(SatelliteType satType)
        {
            List<string> codes = GetOrInitObsCodes(satType);
            return this.ObsTypeCodes[satType];

        }

        /// <summary>
        /// 简单显示。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.SiteInfo.SiteName);
            sb.Append(" ");
            sb.Append(this.SiteInfo.ApproxXyz);
            sb.Append(" ");
            sb.Append(this.ObsInfo.StartTime + "->" + this.ObsInfo.EndTime);

            return sb.ToString();
        }

         
        #endregion

        #region 静态工具方法
        /// <summary>
        /// 直接返回头文件原纪录。
        /// </summary>
        /// <param name="rinexFileName">RINEX 文件路径</param>
        /// <returns></returns>
        public static string ReadText(string rinexFileName)
        {
        using (TextReader r = new StreamReader(rinexFileName, Encoding.UTF8))
            {
                StringBuilder sb = new StringBuilder();
                string line = null;
                while ((line = r.ReadLine()) != null)
                {
                    //中文字符支持
                    int nonAscCount = StringUtil.GetNonAscCount(line.Substring(0, 60 > line.Length ? line.Length : 60));
                    string headerLabel = line.Substring(60 - nonAscCount).TrimEnd();//header label 61-80
                    if (headerLabel.Contains(RinexHeaderLabel.END_OF_HEADER)) break;

                    sb.AppendLine(line);
                }
                return sb.ToString();
            }
        }
        #endregion
    }

}
