//2014.05.24, czs, created 

using System;
using System.Collections.Generic;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 轻量级类型标识。可以不要？
    /// </summary>
    public enum EllipsoidType
    {
        /// <summary>
        /// CGCS2000
        /// </summary>
        CGCS2000,
        /// <summary>
        /// WGS84 椭球
        /// </summary>
        WGS84,
        /// <summary>
        /// GLONASS 采用的椭球
        /// </summary>
        PZ90,
        /// <summary>
        /// 北京54坐标系参数
        /// </summary>
        BJ54,
        /// <summary>
        /// 西安 80 坐标系
        /// </summary>
        XA80
    }

}