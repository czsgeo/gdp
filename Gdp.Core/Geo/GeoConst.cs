//2016.06.04, czs,create in hongqing, 常用常量

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 常用常量
    /// </summary>
    public class GeoConst
    {
        public const double PI = 3.14159265358979323846;
        public const double GM = 3.9860050E14;
        /// <summary>
        /// 地球半径估值（米）6371,000
        /// </summary>
        public const double EARTH_RADIUS_APPROX = 6371000;
        /// <summary>
        /// 光速
        /// </summary>
        public const double LIGHT_SPEED = 299792458;
        /// <summary>
        /// 1米等于多少纳米
        /// </summary>
        public const int NanoPerUnit = 1000000000;
    }
}