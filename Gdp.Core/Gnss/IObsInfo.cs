//2014.12.03, czs,edit, jinxingliangmao shuangliao, 实现 IToTabRow
//2015.05.28, czs , add in namu, 增加估值坐标属性
//2015.10.15, czs , create in hongqing, 增加测站某次观测信息 IObservationInfo 类


using System;
using Gdp.Data;
using System.Collections.Generic; 

namespace Gdp
{ 
    /// <summary>
    /// 某次观测信息，是一个较小时段内的活动，如一次或一天，此信息不稳定，与某次观测相关。通常与SiteInfo（较固定）一起使用。
    /// </summary>
    public interface IObsInfo : ICloneable
    { 
        /// <summary>
        /// GNSS系统及其对应的观测值类型。核心存储
        /// </summary>
        Dictionary<SatelliteType, List<string>> ObsCodes { get; set; }
        /// <summary>
        /// 卫星观测代码字典，如 G1：C1，L1。
        /// 本属性决定观测系统、观测码类型等，非常重要。
        /// 类型化的观测码集合
        /// </summary>
        Dictionary<SatelliteType, List<GnssCodeType>> ObsCodeTypes { get; set; }
        /// <summary>
        /// 获取系统频率数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetFrequenceCount(SatelliteType type = SatelliteType.G);
       
        /// <summary>
        /// 卫星系统类型
        /// </summary>
        List<SatelliteType> SatelliteTypes { get;  }

        /// <summary>
        /// 卫星列表
        /// </summary>
        List<SatelliteNumber> TotalPrns { get; set; }
        /// <summary>
        /// 采样间隔
        /// </summary>
        double Interval { get; set; }

        /// <summary>
        /// 第一次观测时间
        /// </summary>
        Time StartTime { get; set; }
        /// <summary>
        /// 最后一次观测时间
        /// </summary>
        Time EndTime { get; set; }
        /// <summary>
        /// 结束时间是否可以探知？
        /// </summary>
        bool IsEndTimeAvailable { get; set; }
        /// <summary>
        /// 观测时段，如果可用的话。
        /// </summary>
        TimePeriod TimePeriod { get; }
        /// <summary>
        /// 历元数量，通常是大概值，估算而出。
        /// </summary>
        int Count { get; }
        #region 观测信息
        /// <summary>
        /// 观察者
        /// </summary>
        string Observer { get; set; }
        /// <summary>
        /// 观测机构
        /// </summary>
        string ObserverAgence { get; set; }
        #endregion
    }

    /// <summary>
    /// 某次观测信息，是一个较小时段内的活动，如一次或一天，此信息不稳定，与某次观测相关。通常与SiteInfo（较固定）一起使用。
    /// </summary>
    public class ObsInfo : IObsInfo , ICloneable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ObsInfo() { 
            ObsCodes = new Dictionary<SatelliteType, List<string>>();
            this.ObsCodeTypes = new Dictionary<SatelliteType, List<GnssCodeType>>();
            this.IsEndTimeAvailable = true;
        }

        /// <summary>
        /// GNSS系统及其对应的观测值类型。核心存储
        /// </summary>
        public Dictionary<SatelliteType, List<string>> ObsCodes { get; set; }
        /// <summary>
        /// 卫星观测代码字典，如 G1：C1，L1。
        /// 本属性决定观测系统、观测码类型等，非常重要。
        /// 类型化的观测码集合
        /// </summary>
       public Dictionary<SatelliteType, List<GnssCodeType>> ObsCodeTypes { get; set; } 
        /// <summary>
        /// 卫星系统，来源于观测码字典，实际的记录系统，只可以获取不可以修改。
        /// </summary>
        public List<SatelliteType> SatelliteTypes { get { return new List<SatelliteType>(ObsCodes.Keys); } }

        /// <summary>
        /// 卫星列表，为空 或 有值
        /// </summary>
        public List<SatelliteNumber> TotalPrns { get; set; }
        /// <summary>
        /// 采样间隔，单位秒
        /// </summary>
        public double Interval { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get { return (int)(TimePeriod.Span / Interval); } }

        /// <summary>
        /// 第一次观测时间
        /// </summary>
        public Time StartTime { get; set; }
        Time endTime = Time.Default;
        /// <summary>
        /// 最后一次观测时间。默认时间为1天。
        /// </summary>
        public Time EndTime { get { if ( endTime == null || endTime < StartTime) endTime = StartTime + TimeSpan.FromDays(0.9999999999); return endTime; } set { endTime = value; } }
        /// <summary>
        /// 结束时间是否可以探知？
        /// </summary>
        public bool IsEndTimeAvailable { get; set; }

        /// <summary>
        /// 观测时段，如果可用的话。
        /// </summary>
        public  TimePeriod TimePeriod { get { return new TimePeriod(StartTime, EndTime); } }

        #region 观测信息

        /// <summary>
        /// 观察者
        /// </summary>
        public string Observer { get; set; }
        /// <summary>
        /// 观测机构
        /// </summary>
        public string ObserverAgence { get; set; }
        #endregion
        /// <summary>
        /// 获取系统频率数量,通过观测码判断，算法有待检验。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetFrequenceCount(SatelliteType type = SatelliteType.G)
        {
            if (!this.SatelliteTypes.Contains(type)) { return 0; }


            var codes = this.ObsCodes[type];
            List<int> sames = new List<int>();
            foreach (var str in codes)
            {
                int num = Gdp.Utils.StringUtil.GetNumber(str);
                if (!sames.Contains(num))
                {
                    sames.Add(num);
                }
            }

            return sames.Count;// .Count / 2;
        }
        public Object Clone()
        {
            ObsInfo info = new ObsInfo
            {
                Interval = Interval,
                StartTime = StartTime,
                EndTime = EndTime,
                IsEndTimeAvailable = IsEndTimeAvailable,
                Observer = Observer,
                ObserverAgence = ObserverAgence
            };
         
            foreach (var item in ObsCodes)
            {
                var list = new List<string>();
                info.ObsCodes[item.Key] = list;
                foreach (var code in item.Value)
                {
                    list.Add(code.Clone().ToString());
                }
            }

            return info;
        }
    }
}
