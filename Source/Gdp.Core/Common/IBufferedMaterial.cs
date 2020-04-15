//2016.03.27, czs, create in hongqing, 缓存数据流接口
//2016.04.22, czs, create in hongqing, 缓存数据流接口,缓存可自定义

using System;
using System.Collections.Generic;

namespace Gdp
{
    /// <summary>
    /// 缓存数据流接口
    /// </summary>
    /// <typeparam name="TMaterial"></typeparam>
    //public interface IBufferedMaterial<TMaterial> : IBufferedMaterial<TMaterial, List<TMaterial>>
    //{

    //}

    /// <summary>
    /// 缓存数据源
    /// </summary>
    /// <typeparam name="TMaterial"></typeparam>
    public interface IBufferedMaterial<TMaterial> : IBufferedMaterial<TMaterial, IWindowData<TMaterial>>
    {

    }


    /// <summary>
    /// 缓存数据流接口
    /// </summary>
    /// <typeparam name="TMaterial"></typeparam>
    /// <typeparam name="TBuffer"></typeparam>
    public interface IBufferedMaterial<TMaterial, TBuffer>
    {
        /// <summary>
        /// 缓存大小
        /// </summary>
        int BufferSize { get; set; }
        /// <summary>
        /// 数据源是否结束
        /// </summary>
        bool IsMaterialEnded { get; }
        /// <summary>
        /// 最后一个原料
        /// </summary>
        TMaterial LastBufferedMaterial { get; }
        /// <summary>
        /// 缓存
        /// </summary>
        TBuffer MaterialBuffers { get; }
        /// <summary>
        /// 原料缓存实际大小
        /// </summary>
        int MaterialBufferSize { get; }
    }
}
