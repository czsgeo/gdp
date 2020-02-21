//2016.08.29, czs, create in hongqing，提取窗口数据接口
//2017.09.11, czs, edit in hongiqng, 重构，为了更好的使用

using System;
using System.Collections.Generic;

namespace Gdp
{
    //2017.09.23, czs, refactor, 提取非泛型窗口数据

    /// <summary>
    /// 窗口数据
    /// </summary>
    public interface IWindowData
    {

        /// <summary>
        /// 窗口是否已满
        /// </summary>
        bool IsFull { get; }
        /// <summary>
        /// 指定的窗口大小
        /// </summary>
        int WindowSize { get; set; }
    }

    /// <summary>
    /// 窗口数据接口
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IWindowData<TValue> : IBuffer<TValue>, IWindowData
    {
    }
}
