//2014.10.24, czs, create in namu shuangliao, 钟差服务类
//2016.03.05, czs, edit in hongqing, 增加 ISatProduct 星历钟差服务都定义为字典，key为卫星编号

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using Gdp.IO; 

namespace Gdp
{
    /// <summary>
    /// 基于卫星的产品或数值服务
    /// </summary>
    public interface ISatBasedService<TProduct> : IService<TProduct>, IEnumerable<TProduct>
    {
        /// <summary>
        /// 卫星编号
        /// </summary>
        SatelliteNumber Prn { get; }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        TProduct Get(Time time);
    }
    /// <summary>
    /// 单个卫星的信息服务
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    public abstract class SingleSatService<TProduct> : Named, ISatBasedService<TProduct>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SingleSatService() { }
        /// <summary>
        /// 指定了卫星编号的构造函数
        /// </summary>
        /// <param name="prn"></param>
        public SingleSatService(SatelliteNumber prn) { this.Prn = prn; }
        /// <summary>
        /// 卫星编号
        /// </summary>
        public SatelliteNumber Prn { get; protected set; }
        /// <summary>
        /// 通过时间获取服务
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public abstract TProduct Get(Time time);

        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<TProduct> GetEnumerator();
        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    /// <summary>
    /// 基于卫星钟差服务,多卫星的服务
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    public interface IMultiSatProductService<TProduct> :
        // IDictionaryClass<SatelliteNumber, ISatBasedService<TProduct>>,
        ITimedService<TProduct, BufferedTimePeriod>
    {
        /// <summary>
        /// 获取服务。
        /// </summary>
        /// <param name="prn">钟或其载体的名称，如卫星编号</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        TProduct Get(SatelliteNumber prn, Time time);

        /// <summary>
        /// 返回原始数据
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        List<TProduct> Gets(SatelliteNumber prn, Time timeStart, Time timeEnd);

        /// <summary>
        /// 获取钟差，支持测站名称
        /// </summary>
        /// <param name="nameOrPrn">钟或其载体的名称，如卫星编号</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        TProduct Get(String nameOrPrn, Time time);
    }
     

    /// <summary>
    /// 多卫星产品服务基类
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    public abstract class MultiSatProductService<TProduct> :
        Named,
        //BaseDictionary<SatelliteNumber, ISatBasedService<TProduct>>,
        IMultiSatProductService<TProduct>
    {
        protected Log log = new Log(typeof(MultiSatProductService<TProduct>));
        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="nameOrPrn"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public abstract TProduct Get(string nameOrPrn, Time time);

        /// <summary>
        /// 获取时段服务
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        public abstract List<TProduct> Gets(SatelliteNumber prn, Time timeStart, Time timeEnd);

        /// <summary>
        /// 具有数据的时段
        /// </summary>
        public virtual BufferedTimePeriod TimePeriod { get; protected set; }

        /// <summary>
        /// 获取指定卫星和时刻的产品
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public abstract TProduct Get(SatelliteNumber prn, Time time);

        public override string ToString()
        {
            return Name + ", " + this.TimePeriod.ToString();
        }
    }
}
