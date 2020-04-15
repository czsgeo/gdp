//2015.05.12, czs, create in namu, 字典接口
//2015.10.20, czs, refactor in hongqing, 提取集合接口

using System;
using System.Collections.Generic;

namespace Gdp
{
    /// <summary>
    /// 具有关键字的数据存储结构。核心存储为字典。属于管理者模式应用。
    /// </summary>
    /// <typeparam name="TIndex">关键字</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public interface ICollectionClass<TIndex, TValue> : Namable,  IEnumerable<TValue>, IDisposable//, IManager<TValue>
     //   where TIndex : IComparable<TIndex>
    {
        /// <summary>
        /// 统计数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 检索器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue this[TIndex key] { get; set; }
      
        /// <summary>
        /// 设置，直接替换
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void Set(TIndex key, TValue val);
       
        /// <summary>
        /// 获取，若无则返回默认实例 ，如null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue Get(TIndex key);

        /// <summary>
        ///  获取，若无则返回新建实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue GetOrCreate(TIndex key);

        /// <summary>
        /// 清空
        /// </summary>
        void Clear();
    }

}
