//2015.04.16, czs, edit in namu, 增加注释

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 坐标相关常量全在这里。
    /// </summary>
     public  class CoordConsts
    {
        /// <summary>
        /// 墨卡托正方形半个边长，单位：米。
        /// </summary>
        public const double HalfMercatrorLength = 20037508.342789244;//256;// 
         /// <summary>
         /// 地球半径
         /// </summary>
        public const double EarthRadius = 6378137;
        /// <summary>
        /// 度到弧度乘法因子
        /// </summary>
        public const double DegToRadMultiplier = 0.017453292519943295769236907684886;
         /// <summary>
        /// 分到弧度乘法因子
         /// </summary>
        public const double MinToRadMultiplier = 0.0002908882086657215961539484614;
         /// <summary>
         /// 秒到弧度乘法因子
         /// </summary>
        public const double SecToRadMultiplier = 4.8481368110953599358991410233333e-6;
         /// <summary>
         /// 弧度到度乘法因子
         /// </summary>
        public const double RadToDegMultiplier = 57.295779513082320876798154814105;
         /// <summary>
         /// 弧度到分乘法因子
         /// </summary>
        public const double RadToMinMultiplier = 3437.746770784939252607889288846;
         /// <summary>
         /// 弧度到秒乘法因子
         /// </summary>
        public const double RadToSecMultiplier = 206264.80624709635515647335733076; 
         /// <summary>
         /// 1/4 PI
         /// </summary>
        public const double OneQuaterPI = 0.78539816339744830961566084581988;
        /// <summary>
        /// 墨卡托投影的维度死角
        /// </summary>
        public const double MaxLat = 85.083986522047383;//(26.840153,85.083984)

        /// <summary>
        /// 高精度PI，请不要用系统自带的 PI，只有 5 位数的精度。
        /// 3.1415926535897932384626433832795
        /// </summary>
        public const double PI = 3.1415926535897932384626433832795;
        public const double TwoPI = 3.1415926535897932384626433832795 * 2.0; 
    }
}
