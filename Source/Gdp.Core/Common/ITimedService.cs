//2014.10.24, czs, create in namu shuangliao, 通用数据源服务接口
//2014.10.18, czs, create in beijing, IService<TProduct> 服务的内容为产品
//2014.10.18, czs, create in beijing,IService<TProduct, TCondition> 服务的内容为产品
//2014.11.20, czs, edit in numu, 合并类型化的服务接口，都命名为 IService
//2015.05.10, czs, create in namu, 增加具有时间范围的服务

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Gdp
{ 
    /// <summary>
    /// 断断续续的服务，也可以是多时间段的连续的服务，如，多个文件的钟差服务。
    /// </summary>
    /// <typeparam name="TProduct">产品</typeparam>
    /// <typeparam name="TCondition">条件</typeparam>
    public interface IGeoTimedService<TProduct, TCondition> : ITimedService<TProduct, TCondition, TimePeriod>
    {
    }

    /// <summary>
    /// 断断续续的服务，也可以是多时间段的连续的服务，如，多个文件的钟差服务。
    /// </summary>
    /// <typeparam name="TProduct">产品</typeparam>
    /// <typeparam name="TCondition">条件</typeparam>
    /// <typeparam name="TTimeScope">时间</typeparam>
    public interface ITimedService<TProduct, TCondition, TTimeScope> : ITimedService<TProduct, TTimeScope> , IService<TProduct, TCondition>
    { 
    }

    /// <summary>
    /// 断断续续的服务，也可以是多时间段的连续的服务，如，多个文件的钟差服务。
    /// </summary>
    /// <typeparam name="TProduct">产品</typeparam>
    /// <typeparam name="TTimeScope">时间</typeparam>
    public interface ITimedService<TProduct, TTimeScope> : IService<TProduct>, ITimedService<TTimeScope>
    { 

        /// <summary>
        /// 间隔，单位：秒
        /// </summary>
       // int Interval { get; set; }
    }

    //2018.05.02, czs, create in hmx, 具有缓存时间的服务接口
    /// <summary>
    /// 断断续续的服务，也可以是多时间段的连续的服务，如，多个文件的钟差服务。
    /// </summary>
    /// <typeparam name="TTimeScope">时间</typeparam>
    public interface ITimedService<TTimeScope> : IService, ITimePeriod<TTimeScope>
    {

        /// <summary>
        /// 间隔，单位：秒
        /// </summary>
        // int Interval { get; set; }
    }
    //2018.05.02, czs, create in hmx, 具有缓存时间的服务接口
    /// <summary>
    /// 具有缓存时间的服务接口
    /// </summary>
    public interface ITimePeriod<TTimeScope>
    {
        /// <summary>
        /// 时间段
        /// </summary>
        TTimeScope TimePeriod { get; }
    }
}
