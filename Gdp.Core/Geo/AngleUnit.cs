//2015.04.16, czs, edit in namu, 增加注释

using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 角度单位。度和弧度。
    /// </summary>
    public enum AngleUnit
    {
        /// <summary>
        /// 默认未知
        /// </summary>
       // Unknown,
        /// <summary>
        /// 弧度
        /// </summary>
        Radian, 
        /// <summary>
        /// 度小数
        /// </summary>
        Degree,
        /// <summary>
        /// 360°59′59.99999″
        /// </summary>
        DMS_S,
        /// <summary>
        /// 360.595999999
        /// </summary>
        D_MS,
        /// <summary>
        /// 时角，天文常用
        /// </summary>
        HMS_S, 
    }

    /// <summary>
    /// 角度格式
    /// </summary>
   //public enum AngleUnit { 
   //    /// <summary>
   //    /// 度分秒格式
   //    /// </summary>
   //    DMS_S, 
   //    /// <summary>
   //    /// 度小数
   //    /// </summary>
   //    Degree,
   //     /// <summary>
   //     /// 时角，天文常用
   //     /// </summary>
   //    HMS_S, 
   //} 
}
