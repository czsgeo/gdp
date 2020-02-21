//2020.02.22, czs, create in hongqing, 提取测站信息

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using Gdp.IO;
using Gdp.Data.Rinex;

namespace Gdp
{

 
    /// <summary>
    /// 提取测站信息
    /// </summary>
    public class SiteInfoExtractor
    {
        Log log = new Log(typeof(SiteInfoExtractor));
        public SiteInfoExtractor()
        {
            OutputDirectory = Setting.TempDirectory;
            this.IsLonLatFirst = true;
        }

        bool IsCancel;
        DateTime startTime;
        public string[] InputRawPathes { get; set; }
        public string OutputDirectory { get; set; }

        public bool IsLonLatFirst { get; set; }

        ObjectTableStorage table = new ObjectTableStorage();
        public void Run(string[] InputRawPathes)
        {
            this.InputRawPathes = InputRawPathes;
            table = new ObjectTableStorage();

            foreach (var inputPath in InputRawPathes)
            {
                if (IsCancel)
                {
                    log.Info("Canceled at :" + inputPath);
                    break;
                }
                string subDir = Gdp.Utils.PathUtil.GetSubDirectory(InputRawPathes, inputPath);

                Process(subDir, inputPath);
            }

            Complete();
        }


        private void Process(string subDir, string inputPath)
        {
            try
            {
                log.Info("processing :" + inputPath);

                var header = RinexObsFileReader.ReadHeader(inputPath);
                var siteInfo = header.SiteInfo;
                var obsInfo = header.ObsInfo;
                table.NewRow();
                if (IsLonLatFirst)
                {
                    table.AddItem("Lon", siteInfo.ApproxGeoCoord.Lon);
                    table.AddItem("Lat", siteInfo.ApproxGeoCoord.Lat);
                    table.AddItem("Name", siteInfo.SiteName);
                }
                else
                {
                    table.AddItem("Name", siteInfo.SiteName);
                    table.AddItem("Lon", siteInfo.ApproxGeoCoord.Lon);
                    table.AddItem("Lat", siteInfo.ApproxGeoCoord.Lat);
                }
                table.AddItem("Height", siteInfo.ApproxGeoCoord.Height);
                table.AddItem("X", siteInfo.ApproxXyz.X);
                table.AddItem("Y", siteInfo.ApproxXyz.Y);
                table.AddItem("Z", siteInfo.ApproxXyz.Z);
                table.AddItem("ReceiverType", siteInfo.ReceiverType);
                table.AddItem("ReceiverNumber", siteInfo.ReceiverNumber);
                table.AddItem("AntennaType", siteInfo.AntennaType);
                table.AddItem("AntennaNumber", siteInfo.AntennaNumber);
            }
            catch (Exception ex)
            {
                log.Error("转换出错：\t" + inputPath + "\t, " + ex.Message);
            }
        }


        public void Complete()
        {
            table.Name = "SiteInfo";
            var path = Path.Combine(this.OutputDirectory, "SiteInfo.txt.xls");
            Utils.FileUtil.CheckOrCreateDirectory(this.OutputDirectory);

            var span = DateTime.Now - startTime;
            var m = span.TotalMinutes;
            var seconds = span.TotalSeconds;
            int FileCount = InputRawPathes.Length;
            ObjectTableWriter.Write(table, path);
            string info = "Completed, the total number is " + FileCount + ". The time consumption is " + m.ToString("0.000") + "m=" + seconds.ToString("0.00") + "s, with an average of " + (seconds / FileCount).ToString("0.000") + "s each file";

            log.Info(info);
        }

    }

}