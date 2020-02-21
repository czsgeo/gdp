//2015.05.12, czs, create in namu, 字典接口
//2015.10.20, czs, refactor in hongqing, 提取集合接口

using System;
using System.Collections.Generic;

namespace Gdp
{
    /// <summary>
    /// 具有关键字的数据存储结构。核心存储为字典。属于管理者模式应用。
    /// </summary>
    /// <typeparam name="TKey">关键字</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public interface IDictionaryClass<TKey, TValue> : ICollectionClass<TKey, TValue>
        //where TKey : IComparable<TKey>
    {
        /// <summary>
        /// 关键字
        /// </summary>
        List<TKey> Keys { get; }
        /// <summary>
        /// 值集合
        /// </summary>
        List<TValue> Values { get; }

        /// <summary>
        /// 添加，若有保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void Add(TKey key, TValue val);
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(TKey key);
        /// <summary>
        /// 移除一个
        /// </summary>
        /// <param name="key"></param>
        void Remove(TKey key);
    }
}
