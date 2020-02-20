//2014.10.18, czs, edit in beijing, 名称改为 ClockBias，为钟差而设计

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{

    //2018.05.08, czs, extract in hmx, 最轻量级得到钟差表达
    /// <summary>
    /// 最轻量级得到钟差表达
    /// </summary>
    public class SimpleClockBias:IWithTime
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SimpleClockBias()
        {
        }
        /// <summary>
        /// 卫星钟差,在当前参考时刻的钟差。
        /// SV clock bias       (fraction)
        /// </summary>
        public double ClockBias { get; set; }

        /// <summary>
        /// 历元???应该是卫星钟的参考时间//czs,2014.10.18
        /// </summary>
        public Time Time { get; set; }

    }


    /// <summary>
    /// 钟差基础类。
    /// </summary>
    public class SatClockBias : SimpleClockBias, IComparable<SatClockBias>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SatClockBias()
        {
        }
        /// <summary>
        /// 卫星编号
        /// </summary>
        public SatelliteNumber Prn { get; set; }

        /// <summary>
        /// 数据源标识，如igs,igr,com等，即使同一时刻同一颗卫星，不同的数据源也是不同的。
        /// </summary>
        public string Source { get; set; }
        #region time
        /// <summary>
        /// 卫星钟的漂移
        /// SV clock drift      (sec/sec)
        /// </summary>
        public double ClockDrift { get; set; }
        /// <summary>
        /// 卫星钟漂移速度，频漂速度
        /// SV clock drift rate (sec/sec2)
        /// </summary>
        public double DriftRate { get; set; }

        /// <summary>
        /// SP3文件中卫星钟差的精度估值sdev,单位是皮秒
        /// 单位皮秒。文件中文是psec。
        /// psec, picosecond, 皮秒， 1皮秒=10-12秒， 1纳秒=10-9秒， 1微秒=10-6秒， 1毫秒=10-3秒
        /// </summary>
        public double ClockBiasRms { get; set; }
        #endregion
        /// <summary>
        /// 获取偏移时刻的钟差
        /// </summary>
        /// <param name="timeDiffer">与参考时间之差</param>
        /// <returns></returns>
        public double GetOffset(double timeDiffer)
        {
            return GetClockOffset(this, timeDiffer);
        }

        /// <summary>
        /// 获取钟差
        /// </summary>
        /// <param name="bias">钟差参数</param>
        /// <param name="timeDiffer">与参考时间之差</param>
        /// <returns></returns>
        public static double GetClockOffset(SatClockBias bias, double timeDiffer)
        {
            double deltaSatTime = bias.ClockBias + bias.ClockDrift * timeDiffer + bias.DriftRate * timeDiffer * timeDiffer;
            return deltaSatTime;
        }

        #region override
        /// <summary>
        /// 比较，此处只对比时间
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SatClockBias other)
        {
            return (int)Time.CompareTo(other.Time);
        }
        /// <summary>
        /// 相对
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            SatClockBias o = obj as SatClockBias;
            if (o == null) return false;

            return o.Time.Equals(this.Time)
                && ClockBias == o.ClockBias
                && ClockDrift == o.ClockDrift
                ;
        }
        /// <summary>
        /// 哈希表
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Time.GetHashCode() * 3
               + ClockBias.GetHashCode() * 5
               + ClockDrift.GetHashCode() * 13
               ;
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Time + " " + Prn + " " + ClockBias + " " + ClockDrift;
        }
        #endregion

    }
}
