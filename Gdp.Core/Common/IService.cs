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
    /// 服务，提供一个默认的实现。
    /// 当条件与产品一致时，可以组成服务链。
    /// </summary>
    /// <typeparam name="TProduct">产品和条件</typeparam> 
    public interface IServiceChain<TProduct> : IService<TProduct, TProduct>
    {
        /// <summary>
        /// 添加一个服务
        /// </summary>
        /// <param name="service"></param>
        void Add(IService<TProduct, TProduct> service);
        /// <summary>
        /// 添加一个服务
        /// </summary>
        /// <param name="service"></param>
        void Insert(int index, IService<TProduct, TProduct> service);
    }

        
    /// <summary>
    /// 服务，提供一个默认的实现。
    /// </summary>
    /// <typeparam name="TProduct">产品</typeparam>
    /// <typeparam name="TCondition">条件</typeparam>
    public interface IService<TProduct, TCondition> : IService<TProduct>
    {
        /// <summary>
        /// 获取一个产品。
        /// </summary>
        /// <param name="condition">查询产品的条件</param>
        /// <returns></returns>
        TProduct Get(TCondition condition);
    }

    /// <summary>
    /// 服务，一切皆服务。
    /// </summary>
    /// <typeparam name="TProduct">服务提供的产品</typeparam>
    public interface IService<TProduct> : IService
    { 
         
    }


    /// <summary>
    /// 顶层服务接口。 
    /// </summary>
    public interface IService : Namable
    {

    }
     
}
