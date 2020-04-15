//2014.08.20, czs, 修改，修改为计算卫星发射时刻的位置。
//2014.09.21, cy, 增加，具有伪距的星历接口
//2014.10.12，czs, edit in hailutu, 卫星的修正，放到专门的卫星改正 EphemerisCorrector 里面执行,否则会出现精密星历与导航星历相差大的现象， 
//2014.10.24, czs, edit in namu shuangliao, 实现 IClockBiasService 接口
//2016.03.05, czs, edit in hongqing, 重构为字典

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 

namespace Gdp
{
    /// <summary>
    /// 星历服务数据源。一个数据源一般是一个文件。 
    /// 从接口上来看，是支持多系统的。而数据文件中，一般一个文件对应一种卫星系统。
    /// 
    /// 星历服务的类型。有，
    /// 1）导航导航文件
    /// 2）数据库类型
    /// 3）SOA模型的服务，等等。
    /// </summary>
    public interface IEphemerisService //: IComparable<IEphemerisService> 
    {  
        /// <summary>
        /// 卫星数量
        /// </summary>
        int SatCount { get; }
         
        /// <summary>
        /// 该星历采用的坐标系统,如 IGS08， ITR97
        /// </summary>
        string CoordinateSystem { get; }
        /// <summary>
        /// 卫星编号列表。不代表每个历元都有。
        /// </summary>
        List<SatelliteNumber> Prns { get; }
        /// <summary>
        /// 指定类型的卫星编号
        /// </summary>
        /// <param name="type">卫星类型</param>
        /// <returns></returns>
        List<SatelliteNumber> GetPrns(SatelliteType type);
        /// <summary>
        /// 包含的卫星系统类型
        /// </summary>
        List<SatelliteType> SatelliteTypes { get; }
        /// <summary>
        /// 获取星历列表，这些信息是可能是拟合的结果。
        /// </summary>
        /// <param name="prn">卫星</param>
        /// <param name="from">起始时间</param>
        /// <param name="to">终止时间</param>
        /// <param name="interval">间隔（秒）</param>
        List<Ephemeris> Gets(SatelliteNumber prn, Time from, Time to, double interval); 
        /// <summary>
        ///  获取数据源中存储的离散的星历信息，这些信息是原始存储的。
        /// </summary>
        /// <param name="prn"></param>
        /// <returns></returns>
        List<Ephemeris> Gets(SatelliteNumber prn);
        /// <summary>
        /// 获取所有数据源中存储的信息。这个比较占用内存，慎用。
        /// </summary>
        /// <returns></returns>
        List<Ephemeris> Gets(); 
         
        /// <summary>
        /// 指定卫星是否健康可用
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="satTime"></param>
        /// <returns></returns>
        bool IsAvailable(SatelliteNumber prn, Time satTime);
        /// <summary>
        /// 指定系统是否支持，注意：不确保每颗卫星都可用。
        /// </summary>
        /// <param name="satType"></param>
        /// <param name="satTime"></param>
        /// <returns></returns>
        bool IsAvailable(SatelliteType satType, Time satTime);
        /// <summary>
        /// 指定系统是否支持，注意：不确保每颗卫星都可用。
        /// </summary>
        /// <param name="satType"></param>
        /// <param name="satTime"></param>
        /// <returns></returns>
        bool IsAvailable(List<SatelliteType> satType, Time satTime);
    }    
}
