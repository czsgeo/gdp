//2015.10.20, czs, create in hongqing, 列表接口类

using System;
using System.Collections.Generic;

namespace Gdp
{
    //2016.04.22, czs, create in 牛草湾, 增加缓存接口
    /// <summary>
    /// 缓存接口
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IBuffer<TValue> : IListClass<TValue>
    {

    }

    /// <summary>
    /// 列表接口类。
    /// </summary> 
    /// <typeparam name="TValue">值</typeparam>
    public interface IListClass<TValue> : ICollectionClass<int, TValue>
    {
        /// <summary>
        /// 第一个
        /// </summary>
        TValue First { get; }
        /// <summary>
        /// 第2个
        /// </summary>
        TValue Second { get; }
        /// <summary>
        /// 最后个
        /// </summary>
        TValue Last { get; }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        bool Contains(TValue val);

        /// <summary>
        /// 移除一个
        /// </summary>
        /// <param name="val"></param>
        bool Remove(TValue val);

        /// <summary>
        /// 弹出最先进入的一个
        /// </summary>
        /// <returns></returns>
        TValue Pop();

        /// <summary>
        /// 添加， 
        /// </summary> 
        /// <param name="val"></param>
        void Add(TValue val);
        /// <summary>
        /// 添加， 
        /// </summary> 
        /// <param name="val"></param>
        void Add(IEnumerable<TValue> val);
        /// <summary>
        /// 移除指定编号
        /// </summary>
        /// <param name="index"></param>
        void RemoveAt(int index);


        /// <summary>
        /// 获取自列表
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IListClass<TValue> GetSubList(int indexFrom, int count);



        /// <summary>
        /// 获取最后的部分
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        List<TValue> GetLast(int count);
    }     
}
