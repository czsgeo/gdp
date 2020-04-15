 

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gdp.Utils;
using Gdp.IO;

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// Rinex 观测文件读取器
    /// </summary>
    public class RinexObsFileReplacer
    {
        ILog log = Gdp.IO.Log.GetLog(typeof(RinexObsFileReplacer));
        /// <summary>
        /// 初始化一个读取器。
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="IsReadContent">是否读取内容，有的只是概略统计，则不需要读取内容</param>
        public RinexObsFileReplacer(string fileName)
        {
            this.FilePath = fileName;
        }

        public string FilePath { get; set; }


        public void ReplaceHeader(RinexObsFileHeader header, string outPath)
        {
        using (StreamWriter writer = new StreamWriter(outPath))
            {  
                var headerStr = RinexObsFileWriter.ObsHeaderToRinexString(header, header.Version);
                 
                writer.WriteLine(headerStr);
            using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line =null;
                    bool isContent = false;
                    while( ((line = reader.ReadLine()) != null)){
                        if (line.Contains(RinexHeaderLabel.END_OF_HEADER))
                        {
                            isContent = true;
                            continue;
                        }

                        if (isContent)
                        {
                            writer.WriteLine(line);
                        }
                    } 
                }
            }

        }

    }
}
