//2016.09.27, czs create in hongqing, 星历管理器

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 


namespace Gdp.Data.Rinex
{


    /// <summary>
    /// 星历管理器,用于存储各区段的原始SP3星历，该星历用于差值。
    /// </summary>
    public class EphemerisManager : BaseDictionary<SatelliteNumber, PeriodEphemerisStorage>
    {
        /// <summary>
        /// 时段卫星星历
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="maxBreakingCount">允许星历断裂的最大次数（采样率为间隔）</param>
        public EphemerisManager(double interval, int maxBreakingCount)
        {
            this.Interval = interval;
            this.MaxBreakingCount = maxBreakingCount;
        }
        /// <summary>
        /// 间隔
        /// </summary>
        public double Interval { get; set; }
        /// <summary>
        /// 允许星历断裂的最大次数（采样率为间隔）
        /// </summary>
        public int MaxBreakingCount { get; set; }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override PeriodEphemerisStorage Create(SatelliteNumber key)
        {
            return new PeriodEphemerisStorage(Interval, MaxBreakingCount);
        }
    }

}