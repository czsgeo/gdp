//2018.03.15, czs, create in hmx, SP3文件组合器，多个组成一个。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gdp.IO;

namespace Gdp.Data.Rinex
{

    /// <summary>
    /// SP3文件组合器，多个组成一个。
    /// </summary>
    public class Sp3FileCombiner : AbstractBuilder<Sp3File>
    {
        Log log = new Log(typeof(Sp3FileCombiner));
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sp3FileCombiner()
        {
            Sp3Files = new List<Sp3File>();
        }

        /// <summary>
        /// SP3 文件
        /// </summary>
        List<Sp3File> Sp3Files { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        string SouceName { get; set; }

        /// <summary>
        /// 添加一个
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Sp3FileCombiner Add(Sp3File other)
        {
            if(SouceName == null) { SouceName = other.Header.SourceName.Substring(0,2); }
            else
            {
                if(! String.Equals( SouceName, other.Header.SourceName.Substring(0, 2))){
                    log.Warn("数据源不同，拒绝合并," + SouceName + "!=" + other.Header.SourceName);
                    return this;
                }
            }
            Sp3Files.Add(other);
            return this;
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <returns></returns>
        public override Sp3File Build()
        {
            Sp3File sp3File = new Sp3File();

            Sp3Files.OrderBy(m => m.TimePeriod.Start);

            foreach (var file in Sp3Files)
            {
                foreach (var sec in file)
                {
                    sp3File.Add(sec);
                }
            }
            return sp3File;
        }
    }
}