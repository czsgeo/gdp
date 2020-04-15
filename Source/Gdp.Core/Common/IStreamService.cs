//2014.10.18, czs, create in beijing, 任何结果皆为产品
//2014.10.23, czs, edit in flight 成都->沈阳 CA4185， 重构，增加批量生产，增加原材料，产品不再标识，任何类皆是产品，皆是原材料。
//2014.11.20, czs, edit in numu, IMultiParamedService 命名为 IMultiService
//2015.10.25, czs, edit in pengzhou, 批量服务实现流服务IEnumer，重命名为流服务 IStreamService

using System;
using System.Collections.Generic; 

namespace Gdp
{
   
    /// <summary>
    /// 具有缓存的数据流服务
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    /// <typeparam name="TMaterial"></typeparam>
    public interface IBufferedStreamService<TProduct, TMaterial>
        : IStreamService<TProduct, TMaterial>,
        IBufferedMaterial<TMaterial>
    {    
    }


    /// <summary>
    /// 生产委托
    /// </summary>
    /// <typeparam name="TMaterial">原料或查询条件的类型</typeparam>
    /// <typeparam name="TProduct">产品或结果类型</typeparam>
    /// <param name="material">原料或查询条件</param>
    /// <param name="product">产品</param>
    public delegate void ProducedEventHandler<TProduct, TMaterial>(TProduct product, TMaterial material);
    /// <summary>
    /// 生产委托
    /// </summary> 
    /// <typeparam name="TProduct">产品或结果类型</typeparam> 
    /// <param name="product">产品</param>
    public delegate void ProducedEventHandler<TProduct>(TProduct product);
    /// <summary>
    /// 生产委托
    /// </summary> 
    /// <typeparam name="TMaterial">原料类型</typeparam> 
    /// <param name="material">原料</param>
    public delegate void MaterialEventHandler<TMaterial>(TMaterial material);
    /// <summary>
    /// 对原料进行检核，只有通过与不通过，返回结果。
    /// </summary>
    /// <typeparam name="TMaterial"></typeparam>
    /// <param name="material"></param>
    /// <returns></returns>
    public delegate bool MaterialCheckEventHandler<TMaterial>(TMaterial material); 
     
    /// <summary>
    /// 流式服务。
    /// </summary>
    /// <typeparam name="TProduct">产品</typeparam>
    /// <typeparam name="TMaterial">原料或者查询条件</typeparam>
    public interface IStreamService<TProduct, TMaterial> : IStreamService<TProduct>
    { 
        /// <summary>
        /// 计算完成一个，激发一次。
        /// </summary>
        event ProducedEventHandler<TProduct,TMaterial> Produced;
        /// <summary>
        /// 枚举类型的数据源
        /// </summary>
        IEnumer<TMaterial> DataSource { get; }
        /// <summary>
        /// 存储最后一次(当前)生产用的原料或输入条件。
        /// </summary>
        TMaterial CurrentMaterial { get;}
    } 
   
    /// <summary>
    /// 没有指定数据源的流式服务。
    /// </summary>
    /// <typeparam name="TProduct">产品</typeparam>
    public interface IStreamService<TProduct> : IService<TProduct>,
        IEnumer<TProduct>,  ICancelAbale
    {

        /// <summary>
        /// 计算完成一个，激发一次。
        /// </summary>
        event Action<TProduct> ProductProduced;

        /// <summary>
        /// 存储最后一次（当前）生产的产品
        /// </summary>
        TProduct CurrentProduct { get; }

        /// <summary>
        /// 批量生产
        /// </summary> 
        /// <param name="startIndex">起始的编号，从0开始</param>
        /// <param name="maxCount">最大的计算数量</param>
        /// <returns></returns>
        List<TProduct> Gets(
           int startIndex = 0,
           int maxCount = int.MaxValue);

        /// <summary> 
        /// 获取下一列表
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        List<TProduct> GetNexts(int count);

    }
}