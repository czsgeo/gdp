//2013.11.04, czs, edit, 采用二级缓存加快计算速度
//2014.06.24, 崔阳, edit, 修改了IsHealth(),首先判断是否满足插值的条件。判断是否满足插值的限差   Cui Yang 2014.06.24
//2014.12.29,czs, edit in namu, 缓冲不易设置太大，如果设置2倍的话，会造成在星历拼接的时段结果不稳定。
//2015.05.10, czs, edit in namu, 分离数据与服务
//2016.03.06, czs, edit in hongqing, 优化
//2016.09.27, czs, edit in hongqqing, 优化，可用性判断
//2018.04.27, czs, edit in hmx, 增加设置星历的最大断裂次数的参数

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 


namespace Gdp.Data.Rinex
{
    /// <summary>
    /// 多时段卫星星历管理器。防止时段空缺造成的星历差值错误。
    /// </summary>
    public class PeriodEphemerisStorage : BaseDictionary<BufferedTimePeriod, EphemerisStorage>
    {
        /// <summary>
        /// 时段卫星星历
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="MaxBreakingCount"></param>
        public PeriodEphemerisStorage(double interval, int MaxBreakingCount)
        {
            this.Interval = interval;
            this.MaxBreakingCount = MaxBreakingCount;
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
        /// 创建一个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override EphemerisStorage Create(BufferedTimePeriod key)
        {
            return new EphemerisStorage();
        }

        /// <summary>
        /// 批量添加，并进行分类。
        /// </summary>
        /// <param name="entities"></param>
        public void Add(List<Gdp.Ephemeris> entities)
        {
            double maxBreakingSeconds = Interval * MaxBreakingCount;
            var first = entities[0];
            Time lastTime = first.Time;
            BufferedTimePeriod perid = new BufferedTimePeriod(lastTime, lastTime, Interval / 2) { IsOnlyStartHashCode = true };
            this.GetOrCreate(perid).Add(first.Time.SecondsOfWeek, first);

            int i = -1;
            foreach (var item in entities)
            {
                i++;
                if (i == 0) { continue; }
                var span = (item.Time - lastTime);
                if (span  < maxBreakingSeconds)//允许断裂，自动拟合拼接
                {
                    var list = this.GetOrCreate(perid);
                    perid.End = item.Time;

                    list.Add(item.Time.SecondsOfWeek, item);
                }
                else //超出限差，重新建立时段
                {
                    perid = new BufferedTimePeriod(item.Time, item.Time, Interval / 2) { IsOnlyStartHashCode = true };
                    this.GetOrCreate(perid).Add(item.Time.SecondsOfWeek, item);
                }
                lastTime = item.Time;
            }
        }

        /// <summary>
        /// 返回指定时段的星历列表。如果没有则返回null
        /// </summary>
        /// <returns></returns>
        public EphemerisStorage GetSatEphemerises(Time Time)
        {
            foreach (var item in this.Keys)
            {
                if (item.BufferedContains(Time)) { return this[item]; }
            }
            return null;
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }

}