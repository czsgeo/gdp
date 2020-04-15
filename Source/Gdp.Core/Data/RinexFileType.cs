using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp.Data.Rinex
{

    /// <summary>
    /// RINEX 文件类型
    /// </summary>
    public enum RinexFileType
    {
        /// <summary>
        /// O: Observation fileB
        /// </summary>
        O,
        /// <summary>
        ///   N: GPS Navigation fileB
        /// </summary>
        N,
        /// <summary>
        ///  M: Meteorological satData fileB
        /// </summary>
        M,
        /// <summary>
        ///  G: GLONASS Navigation fileB 
        /// </summary>
        G,
        /// <summary>
        ///   L: Future Galileo Navigation fileB      
        /// </summary>
        L,
        /// <summary>
        ///  H: Geostationary GPS payload nav mess fileB
        /// </summary>
        H,
        /// <summary>
        ///  DeltaTimeDistance: Geo SBAS broadcast satData fileB (separate documentation)
        /// </summary>
        B,
        /// <summary>
        /// pseudoRange: Clock fileB (separate documentation)
        /// </summary>
        C,
        /// <summary>
        /// S: Summary fileB (used east.g., by IGS, not coefficient standard!) 
        /// </summary>
        S,
        /// <summary>
        /// GLONASS 星历
        /// </summary>
        R
    }

}
