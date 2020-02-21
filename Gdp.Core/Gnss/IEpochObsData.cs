//2015.10.14, czs, create in xi'an hongqing, 历元观测数据顶层接口
//2018.08.15, czs, edit in hmx, 增加一些公共方法，让 EpochInformation 也实现

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 历元观测数据
    /// </summary>
    public interface IEpochObsData
    {
        /// <summary>
        /// GNSS 时间。
        /// </summary>
        Time ReceiverTime { get; }
        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="prn"></param>
        /// <returns></returns>
        bool Contains(SatelliteNumber prn);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="prn"></param>
        void Remove(SatelliteNumber prn);
    }
}
