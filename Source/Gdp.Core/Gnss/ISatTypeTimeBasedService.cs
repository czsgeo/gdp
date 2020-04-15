//2015.12.22, czs, create in hongqing, 建立 ISatTypeTimeBasedService，基于卫星类型和指定时刻的服务

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using Gdp.Data; 

namespace Gdp
{
    /// <summary>
    /// 获取服务。基于卫星和指定时刻的服务，如钟差、星历等。
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    public interface ISatTypeTimeBasedService<TProduct>
    {
        /// <summary>
        /// 获取服务。
        /// </summary>
        /// <param name="satelliteType">钟或其载体的名称，如卫星编号</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        TProduct Get(SatelliteType satelliteType, Time time);
    }
}
