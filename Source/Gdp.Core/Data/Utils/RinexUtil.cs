using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gdp.Utils;

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// RINEX文件工具。
    /// </summary>
    public class RinexUtil
    {
        /// <summary>
        /// 由文件类型确定导航系统。
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static SatelliteType GetSatelliteType( RinexFileType fileType)
        {
            switch (fileType)
            {
                case RinexFileType.N: return SatelliteType.G;
                case RinexFileType.L: return SatelliteType.E;
                case RinexFileType.G: return SatelliteType.R;
                default: break;
            }
            return SatelliteType.G;
        }


        /// <summary>
        /// StreamReader 略过的行数
        /// </summary>
        /// <param name="r">数据流阅读器</param>
        /// <param name="lineCount"></param>
        public static void SkipLines(StreamReader r, int lineCount)
        {
            //略去头部
            for (int i = 0; i < lineCount; i++)
            {
                r.ReadLine();
            }
        }
        /// <summary>
        /// /略去头部
        /// </summary>
        /// <param name="r">数据流阅读器</param>
        public static void SkipHeader(StreamReader r)
        {
            string line = null;
            //略去头部
            while ((line = r.ReadLine()) != null)
            {
                if ( String.IsNullOrWhiteSpace(line) ||  line.Length < 60 || line.Substring(60).Contains(RinexHeaderLabel.END_OF_HEADER))
                    break;
            }
        }
         
    }
}
