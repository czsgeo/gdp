//2018.09.21, czs, crreate in hmx, O文件工具

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gdp.Utils;
using Gdp.IO;

namespace Gdp.Data.Rinex
{
    /// <summary>
    ///  O文件工具
    /// </summary>
    public class ObsFileUtil
    {

        /// <summary>
        /// 读取数据
        /// </summary>
        public static Data.Rinex.RinexObsFile ReadFile(string ObsPath)
        {
            Data.Rinex.RinexObsFile ObsFile = null;
            string lastChar = Gdp.Utils.StringUtil.GetLastChar(ObsPath);
            string lastChar3 = Gdp.Utils.StringUtil.GetLastChar(ObsPath, 3);
            string lastChar5 = Gdp.Utils.StringUtil.GetLastChar(ObsPath, 5);
            if (String.Equals(lastChar, "o", StringComparison.CurrentCultureIgnoreCase) || String.Equals(lastChar3, "rnx", StringComparison.CurrentCultureIgnoreCase))
            {
                var obsFileReader = new RinexObsFileReader(ObsPath);
                ObsFile = obsFileReader.ReadObsFile();
            }

            //if (String.Equals(lastChar, "z", StringComparison.CurrentCultureIgnoreCase)
            //    || String.Equals(lastChar3, "crx", StringComparison.CurrentCultureIgnoreCase)
            //    || String.Equals(lastChar5, "crx.gz", StringComparison.CurrentCultureIgnoreCase)
            //    )
            //{
            //    Gdp.IO.InputFileManager inputFileManager = new Gdp.IO.InputFileManager(Setting.TempDirectory);
            //    ObsPath = inputFileManager.GetLocalFilePath(ObsPath, "*.*o;*.rnx", "*.*");
            //    var obsFileReader = new RinexObsFileReader(ObsPath);
            //    ObsFile = obsFileReader.ReadObsFile();
            //}
            //if (String.Equals(lastChar, "s", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    ObsFile = new TableObsFileReader(ObsPath).Read();
            //}
            if (ObsFile == null)
            {
                throw new Exception("不支持输入文件格式！");
                return ObsFile;
            }
            return ObsFile;
        }
    }
}
