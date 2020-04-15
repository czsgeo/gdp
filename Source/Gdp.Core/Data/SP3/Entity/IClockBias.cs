//2014.10.18, czs, edit in beijing, 名称改为 IClockBias，为钟差而设计
//2018.03.16, czs, edit in hmx, 数据源标识，如igs,igr,com等，即使同一时刻同一颗卫星，不同的数据源也是不同的。

using System; 

namespace Gdp
{
    /// <summary>
    /// 具有卫星编号的接口
    /// </summary>
    public interface IWithTime
    {

        /// <summary>
        /// 历元时间
        /// </summary>
        Time Time { get; set; }
    }
    /// <summary>
    /// 具有卫星编号的接口
    /// </summary>
    public interface IWithPrn
    {

        /// <summary>
        /// 卫星编号
        /// </summary>
        SatelliteNumber Prn { get; set; }
    }

    /// <summary>
    /// 具有卫星编号和历元的接口
    /// </summary>
    public interface ISatTime : IWithPrn, IWithTime
    {
    }
    /// <summary>
    /// 具有卫星编号和历元的接口
    /// </summary>
    public interface ISatTimeSourced : ISatTime
    {
        /// <summary>
        /// 数据源标识，如igs,igr,com等，即使同一时刻同一颗卫星，不同的数据源也是不同的。
        /// </summary>
        string Source { get; set; }
    }

    /// <summary>
    /// 卫星钟差接口
    /// </summary>
    public interface IClockBias : ISatTimeSourced, ISimpleClockBias
    {
        ///// <summary>
        ///// 历元时间
        ///// </summary>
        //Time Time { get; set; }
        ///// <summary>
        ///// 卫星编号
        ///// </summary>
        //SatelliteNumber Prn { get; set; }
        /// <summary>
        /// 偏移速度
        /// </summary>
        double ClockDrift { get; set; }
        /// <summary>
        /// 速率
        /// </summary>
        double DriftRate { get; set; }
        
    }

    //2018.05.08, czs, extract in hmx, 最轻量级得到钟差表达
    /// <summary>
    /// 最轻量级得到钟差表达
    /// </summary>
    public interface ISimpleClockBias : IWithTime
    {

        /// <summary>
        /// 钟差偏差
        /// </summary>
        double ClockBias { get; set; }
    }
}
