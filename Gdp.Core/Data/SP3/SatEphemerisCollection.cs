//2018.03.16, czs, create in hmx, 单颗卫星的星历列表
//2018.03.16, czs, edit in hmx, 数据源标识，如igs,igr,com等，即使同一时刻同一颗卫星，不同的数据源也是不同的。


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gdp.Data.Rinex;

namespace Gdp.Data
{

    /// <summary>
    /// 卫星星历集合
    /// </summary>
    public class SatEphemerisCollection : TimedSatObjectCollection<Ephemeris>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="isUniqueSource">是否唯一数据源，高精度的需求</param>
        /// <param name="SourceCode">数据源代码，默认ig</param>
        public SatEphemerisCollection(bool isUniqueSource = true, string SourceCode = "ig") : base(isUniqueSource, SourceCode)
        {
            this.Name = "组合多文件星历 ";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">按照类型读入的原始数据结构</param>
        /// <param name="isUniqueSource"></param>
        /// <param name="indicatedSource"></param>
        public SatEphemerisCollection(Dictionary<SatelliteType, Dictionary<string, List<Sp3File>>> data, bool isUniqueSource = true, string indicatedSource = "ig") : base(isUniqueSource, indicatedSource)
        {
            this.Name = "组合多文件星历";
            
            //把这些组合成一个 SatEphemerisCollection 
            foreach (var typedSource in data)
            {
                var currentType = typedSource.Key;
                if (typedSource.Value.Count > 0)
                {
                    this.Name += "," + typedSource.Key + ":";
                }
                foreach (var sourcedSp3File in typedSource.Value)
                {
                    var source = sourcedSp3File.Key;
                    if (isUniqueSource && !source.Contains(indicatedSource))
                    {
                        log.Warn("由于指定了数据源，且设置为唯一，忽略 " + source);
                        continue;
                    }
                    int i = 0;
                    foreach (var file in sourcedSp3File.Value)
                    {
                        if (i != 0) { this.Name += ", "; }
                        this.Name += file.Name;
                        if(this.CoordinateSystem == null)
                        {
                            this.CoordinateSystem = file.CoordinateSystem; 
                        }else if ( String.Compare( this.CoordinateSystem , file.CoordinateSystem, true) != 0){
                            log.Warn( "坐标系统不一致：" + this.CoordinateSystem  + " != " +file.CoordinateSystem +", " + file);
                        }                      

                        foreach (var sat in file.GetSatEphemerisCollection())
                        {
                            //只处理选定的类型
                            if (currentType != sat.Prn.SatelliteType) { continue; }

                            this.GetOrCreate(sat.Prn).Add(sat);
                        }
                        i++;
                    }
                }
            }
        }


        /// <summary>
        /// 该星历采用的坐标系统,如 IGS08， ITR97
        /// </summary>
        public string CoordinateSystem { get; set; }

    }
}