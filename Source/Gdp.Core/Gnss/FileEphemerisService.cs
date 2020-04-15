//2014.08.20, czs, refactor，去掉了一些冗余的接口
//2014.09.21, cui yang, add, 卫星发射时刻采用距离估计似乎更好
//2014.10.10，czs，edit in 海鲁吐， 确定使用伪距、接收机钟面时获取发射时刻的卫星星历
//2014.10.12，czs, edit in hailutu, 卫星的修正，放到专门的卫星改正 EphemerisCorrector 里面执行,否则会出现精密星历与导航星历相差大的现象
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 星历以文件形式存储。
    /// 一个类的实例代表一个文件。
    /// 导航导航文件，又包括GPS，GLONASS，北斗，伽利略等，文件又包含精密星历 Sp3 等。
    /// 导航文件应该分类，如GPS发布轨道根数，2小时一次，而GLONASS有的星历太短，还需要反推轨道根数？
    /// </summary>
    public abstract class FileEphemerisService : MultiSatProductService<Ephemeris>, IEphemerisService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FileEphemerisService() { } 

        #region 属性  
        /// <summary>
        /// 星历服务类型
        /// </summary>
       // public virtual EphemerisServiceType ServiceType { get { return EphemerisServiceType.Unknown; } }

        /// <summary>
        /// 卫星数量。这个只能说明整个文件数据源包含的卫星数量。不代表单个历元。
        /// </summary>
        public virtual int SatCount { get { return Prns.Count; } }
        
        /// <summary>
        /// 该星历采用的坐标系统,如 IGS08， ITR97
        /// </summary>
        public abstract string CoordinateSystem  { get; }
        #endregion

        #region 虚拟，子类必须实现

        /// <summary>
        /// 卫星编号列表。
        /// </summary>
        public abstract List<SatelliteNumber> Prns { get; }


        /// <summary>
        /// 卫星类型列表
        /// </summary>
        public virtual List<SatelliteType> SatelliteTypes
        {
            get
            {
                List<SatelliteType> types = new List<SatelliteType>();
                foreach (var item in Prns)
                {
                    if (!types.Contains(item.SatelliteType))
                        types.Add(item.SatelliteType);
                }
                return types;
            }
        }
        /// <summary>
        /// 指定类型的卫星编号
        /// </summary>
        /// <param name="type">卫星类型</param>
        /// <returns></returns>
        public List<SatelliteNumber> GetPrns(SatelliteType type)
        {
            List<SatelliteNumber> prns = new List<SatelliteNumber>();
            foreach (var item in Prns)
            {
                if (item.SatelliteType == type && !prns.Contains(item))
                    prns.Add(item);
            }
            return prns;
        }
        /// <summary>
        /// 获取所有数据源中存储的信息。这个比较占用内存，慎用。
        /// </summary>
        /// <returns></returns>
        public abstract List<Ephemeris> Gets();
        /// <summary>
        /// 获取指定时刻卫星信息
        /// </summary>
        /// <param name="prnName">卫星编号</param>
        /// <param name="gpsTime">时间</param>
        /// <returns></returns>
        public override Ephemeris Get(string prnName, Time gpsTime) { return Get(SatelliteNumber.Parse(prnName), gpsTime); }
        /// <summary>
        /// 是否健康
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="satTime"></param>
        /// <returns></returns>
        public abstract bool IsAvailable(SatelliteNumber prn, Time satTime); 
        #endregion

        /// <summary>
        /// 比较大小。用于排序。如果为负数 则表示本对象较小。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(FileEphemerisService other) { return this.TimePeriod.CompareTo(other.TimePeriod); } 

        /// <summary>
        /// 获取数据源中存储的离散的星历信息，这些信息是原始存储的。
        /// </summary>
        /// <param name="prn"></param>
        /// <returns></returns>
        public virtual List<Ephemeris> Gets(SatelliteNumber prn) { return Gets(prn, TimePeriod.Start, TimePeriod.End); }

        /// <summary>
        /// 获取星历列表，这些信息是可能是拟合的结果。
        /// </summary>
        /// <param name="prn">卫星编号</param>
        /// <param name="from">起始时间</param>
        /// <param name="to">终止时间</param>
        /// <param name="interval">间隔（秒）</param>
        public virtual List<Ephemeris> Gets(SatelliteNumber prn, Time from, Time to, double interval)
        {
            List<Ephemeris> list = new List<Ephemeris>();
            for (Time i = from; i <= to; i = i + interval)
            {
                list.Add(Get(prn, i));
            }
            return list;
        }
        /// <summary>
        /// 打印输出
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + ", "+ this.TimePeriod.ToString() + ", " + Gdp.Utils.StringUtil.GetArrayString<SatelliteType>(this.SatelliteTypes.ToArray());
        }
        /// <summary>
        /// 从时段和系统类型判断是否支持
        /// </summary>
        /// <param name="satType"></param>
        /// <param name="satTime"></param>
        /// <returns></returns>
        public virtual bool IsAvailable(SatelliteType satType, Time satTime)
        {
            return (SatelliteTypes.Contains(satType)) && TimePeriod.Contains(satTime);
        }
        /// <summary>
        /// 从时段和系统类型判断是否支持
        /// </summary>
        /// <param name="satType"></param>
        /// <param name="satTime"></param>
        /// <returns></returns>
        public virtual bool IsAvailable(List<SatelliteType> satType, Time satTime)
        {
            if (!TimePeriod.Contains(satTime)) { return false; }
            foreach (var item in satType)
            {
                if(!SatelliteTypes.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
