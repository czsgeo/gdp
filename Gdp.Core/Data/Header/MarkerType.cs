using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// 类型 RINEX 3.0
    /// </summary>
    public enum MarkerTypes
    {
        /// <summary>
        /// Earth-fixed, high-precision monumentation
        /// </summary>        
        GEODETIC,
        /// <summary>
        /// Earth-fixed, low-precision monumentation
        /// </summary>
        NON_GEODETIC,
        /// <summary>
        /// Generated from network processing
        /// </summary>
        NON_PHYSICAL,
        /// <summary>
        /// Orbiting space vehicle
        /// </summary>
        SPACEBORNE,
        /// <summary>
        /// Aircraft, balloon, etc.
        /// </summary>
        AIRBORNE,
        /// <summary>
        ///  Mobile water craft
        /// </summary>
        WATER_CRAFT,
        /// <summary>
        /// Mobile terrestrial vehicle
        /// </summary>
        GROUND_CRAFT,
        /// <summary>
        /// "Fixed" on water surface
        /// </summary>
        FIXED_BUOY,
        /// <summary>
        ///  Floating on water surface
        /// </summary>
        FLOATING_BUOY,
        /// <summary>
        ///  Floating ice sheet, etc. 
        /// </summary>
        FLOATING_ICE,
        /// <summary>
        ///  "Fixed" on a glacier
        /// </summary>
        GLACIER,
        /// <summary>
        /// Rockets, shells, etc
        /// </summary>
        BALLISTIC,
        /// <summary>
        ///  Animal carrying a receiver.
        /// </summary>
        ANIMAL,
        /// <summary>
        /// Human being
        /// </summary>
        HUMAN,
        /// <summary>
        /// 其它。自定义或者升级版本。
        /// </summary>
        OTHER
    }
}