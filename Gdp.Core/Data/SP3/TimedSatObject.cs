//2018.03.16, czs, create in hmx, 单颗卫星的星历列表
//2018.03.17, czs, edit in hmx, 抽象为单颗卫星具有时间的对象

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using Gdp.Data.Rinex;

namespace Gdp.Data
{

    /// <summary>
    /// 单颗卫星具有时间的对象
    /// </summary>
    
    public class TimedSatObject<TEntity> : BaseDictionary<Time, TEntity> where TEntity: IWithTime
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public TimedSatObject(SatelliteNumber prn)
        {
            this.Prn = prn;
            this.IsSameSourceRequired = true;
            this.IsOrdered = false;
        } 
        /// <summary>
        /// 数据源前缀，一般以前两个字母表示，如ig，wh
        /// </summary>
        public string SourceCode { get; set; }
        /// <summary>
        /// 卫星编号
        /// </summary>
        public SatelliteNumber Prn { get; set; }
        /// <summary>
        /// 用于指示，是否已经按照时间大小排序。
        /// </summary>
        public bool IsOrdered { get; set; }
        /// <summary>
        /// 是否需要相同的数据源。精密定位时需要。默认需要。
        /// </summary>
        public bool IsSameSourceRequired { get; set; }
        /// <summary>
        /// 排序后的时间序列,注意：每生成一次，对象切换一次。
        /// </summary>
        public List<Time> OrderedTimes { get; set; }
        /// <summary>
        /// 计算并获取采样间隔
        /// </summary>
        /// <returns></returns>
        public double GetInterval()
        {
            if (CheckTimeOrder(2))
            { 
                return OrderedTimes[1] - OrderedTimes[0];
            }
            return 900;
        }
        BufferedTimePeriod timePeriod = null;
        /// 时段
        /// </summary>
        public BufferedTimePeriod TimePeriod
        {
            get
            {
                if(timePeriod != null) { return timePeriod; }
                if (CheckTimeOrder(2))
                {
                    double buffer = 0;
                    timePeriod = new BufferedTimePeriod(OrderedTimes[0], OrderedTimes[OrderedTimes.Count - 1], buffer);
                }
                else
                {
                    timePeriod = BufferedTimePeriod.Zero;
                }
                return timePeriod;
            }
        } 
         

        /// <summary>
        /// 快速移除第一个历元的数据
        /// </summary>
        public override void RemoveFirst()
        {
            if (CheckTimeOrder(1))
            {
                Time firstTime = OrderedTimes[0];
                base.Remove(firstTime);
            }
        } 

        /// <summary>
        /// 确保已排序
        /// </summary>
        public  bool CheckTimeOrder(int minCount = 2)
        {
            if(this.Count < minCount) { return false; }
            Build();
            return true;
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="satEphemeris"></param>
        public void Add(TimedSatObject<TEntity> satEphemeris)
        {
            foreach (var item in satEphemeris)
            {
                this.Add(item);
            }
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="satEphemeris"></param>
        public void Add(IEnumerable<TEntity> satEphemeris)
        {
            foreach (var item in satEphemeris)
            {
                this.Add(item);
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="values"></param>
        internal void Add(ICollection<TEntity> values)
        {
            foreach (var item in values)
            {
                this.Add(item);
            }
        }


        /// <summary>
        /// 添加一个星历结果。
        /// </summary>
        /// <param name="ephemeris"></param>
        /// <returns></returns>
        public AddResultType Add(TEntity ephemeris)
        {
            //if (ephemeris.Prn != this.Prn)
            //{
            //    return AddResultType.Skiped;
            //}
           
            if (!this.Contains(ephemeris.Time))//如果没有包含，则判断后直接添加
            {
                ////判断数据源
                //if (ephemeris.Source != null )
                //{
                //    var newPrefix = ephemeris.Source.Substring(0, 2);
                //    if (SourceCode == null) { SourceCode = newPrefix; }

                //    if(IsSameSourceRequired && !String.Equals( newPrefix,  SourceCode, StringComparison.CurrentCultureIgnoreCase))
                //    {
                //        log.Debug("星历数据源不相同，拒绝添加！" + SourceCode  + " != " + newPrefix);
                //        return AddResultType.Skiped;
                //    }
                //}

                this.Add(ephemeris.Time, ephemeris);

                IsOrdered = false;
                return AddResultType.Added;
            }
            else//如果包含了，则判断数据源，选择精度最高的保存
            {
                //var old = this[ephemeris.Time];
                //if (ephemeris.Source == null || old.Source == null) { return AddResultType.Skiped; }

                ////此处只考虑igs官方数据源
                //if (old.Source.Contains("igu") || old.Source.Contains("igv"))
                //{
                //    if (ephemeris.Source.Contains("igs") || ephemeris.Source.Contains("igr"))
                //    {
                //        this[ephemeris.Time] = ephemeris;
                //        log.Debug(ephemeris.Source + " 替换了 " + old.Source);
                //        IsOrdered = false;
                //        return AddResultType.Replaced;
                //    }
                //}
            }
            return AddResultType.Skiped;
        }

        /// <summary>
        /// 构建排序索引
        /// </summary>
        public void Build()
        {
            if (!IsOrdered)
            {
                OrderedTimes = new List<Time>(this.Keys);
                OrderedTimes.OrderBy(m => m.DateTime);
                IsOrdered = true;
            }
        }
        /// <summary>
        /// 简要信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Prn + ", " + SourceCode + ", "  + GetInterval() + ", " + this.Count + ", "+ TimePeriod;
        }
    }

    /// <summary>
    /// 添加结果类型
    /// </summary>
    public enum AddResultType
    {
        /// <summary>
        /// 已添加
        /// </summary>
        Added,
        /// <summary>
        /// 跳过
        /// </summary>
        Skiped,
        /// <summary>
        /// 替换
        /// </summary>
        Replaced
    }
    /// <summary>
    /// 添加结果类型
    /// </summary>
    //public enum AddResultType
    //{
    //    /// <summary>
    //    /// 已添加
    //    /// </summary>
    //    Added,
    //    /// <summary>
    //    /// 跳过
    //    /// </summary>
    //    Skiped,
    //    /// <summary>
    //    /// 替换
    //    /// </summary>
    //    Replaced
    //}
}