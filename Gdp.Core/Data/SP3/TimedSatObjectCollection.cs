//2018.03.16, czs, create in hmx, 单颗卫星的星历列表
//2018.03.17, czs, edit in hmx, 多系统卫星的星历列表


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 

namespace Gdp.Data
{  /// <summary>
   ///多系统卫星的属性列表
   /// </summary>
    public class TimedSatObjectCollection<TEntity> : TimedSatObjectCollection<TEntity, TimedSatObject<TEntity>>
        where TEntity : IWithTime
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="isUniqueSource">是否唯一数据源，高精度的需求</param>
        /// <param name="SourceCode">数据源代码，默认ig</param>
        public TimedSatObjectCollection(bool isUniqueSource = true, string SourceCode = "ig")
            : base(isUniqueSource, SourceCode)
        {
        }

        /// <summary>
        /// 创建一个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TimedSatObject<TEntity> Create(SatelliteNumber key)
        {
            return new TimedSatObject<TEntity>(key) { IsSameSourceRequired = IsSameSourceRequired, SourceCode = SourceCode };
        }
    }
    /// <summary>
    ///多系统卫星的属性列表
    /// </summary>
    public abstract class TimedSatObjectCollection<TEntity, TTimedSatObject> : BaseDictionary<SatelliteNumber, TTimedSatObject>
        where TEntity : IWithTime
        where TTimedSatObject : TimedSatObject<TEntity>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="isUniqueSource">是否唯一数据源，高精度的需求</param>
        /// <param name="SourceCode">数据源代码，默认ig</param>
        public TimedSatObjectCollection(bool isUniqueSource = true, string SourceCode = "ig")
        {
            this.SourceCode = SourceCode;
            this.IsSameSourceRequired = isUniqueSource;
        }


        /// <summary>
        /// 数据源前缀，一般以前两个字母表示，如ig，wh
        /// </summary>
        public string SourceCode { get; set; }
        /// <summary>
        /// 是否需要相同的数据源。精密定位时需要。默认需要。
        /// </summary>
        public bool IsSameSourceRequired { get; set; }
        BufferedTimePeriod timePeriod = null;
        /// <summary>
        /// 最长的时间段
        /// </summary>
        public BufferedTimePeriod TimePeriod
        {
            get
            {
                if(timePeriod!= null) { return timePeriod; }
                if (this.Count == 0)
                {
                    timePeriod = new BufferedTimePeriod(Time.MinValue, Time.MinValue);
                    return timePeriod;
                } 

                BufferedTimePeriod period = new BufferedTimePeriod(First.TimePeriod);
                period.SetSameBuffer(32.0); //32秒缓冲 
                if (this.Count == 1)
                {
                    timePeriod = period;
                    return timePeriod;
                }

                foreach (var item in this)
                {
                    if (period.Start > item.TimePeriod.Start)
                    {
                        period.Start = item.TimePeriod.Start;
                    }
                    if (period.End < item.TimePeriod.End)
                    {
                        period.End = item.TimePeriod.End;
                    }
                }
                timePeriod = period;
                return period;
            }
        }

        /// <summary>
        /// 返回其中一个间隔，默认为1
        /// </summary>
        public double Interval
        {
            get
            {
                int inter = 1;
                foreach (var item in this.Values)
                {
                    if (item.Count > 1)
                    {
                        return item.GetInterval();
                    }
                }
                return inter;
            }
        }

        /// <summary>
        /// 当前所有的卫星编号集合
        /// </summary>
        public List<SatelliteNumber> Prns { get { return this.Keys; } }

        /// <summary>
        /// 卫星系统类型
        /// </summary>
        public List<SatelliteType> SatelliteTypes
        {
            get
            {
                List<SatelliteType> satTypes = new List<SatelliteType>();
                foreach (var item in Prns)
                {
                    if (!satTypes.Contains(item.SatelliteType))
                    {
                        satTypes.Add(item.SatelliteType);
                    }
                }

                return satTypes;
            }
        }
        /// <summary>
        /// 创建一个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        //public override TTimedSatObject Create(SatelliteNumber key)
        //{
        //    return new TimedSatObject<TEntity>(key) { IsSameSourceRequired = IsSameSourceRequired, SourceCode = SourceCode };
        //}
    }
}