//2015.01.18, czs, create in namu, 为了满足控制台程序需求，特建立此项目
//2019.12.12, czs, edit in hongqing, 迁移到 .net standard, Net Core App
//2020.02.21, czs, edit in hongqing, 增加信息提取功能

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.IO; 
using Gdp;
using Gdp.Data;
using Gdp.Data.Rinex; 
using Gdp.Utils;
using Gdp.IO; 

namespace Gdp
{
    /// <summary>
    /// 为了满足控制台程序需求，特建立此项目
    /// </summary>
    public class LocalGnssExecutor
    { 
        #region 变量、属性
        static ILog log = Log.GetLog(typeof(LocalGnssExecutor));  
        static int ProcessCount = 0; 
        //若非在安装目录，需要初始化
        public static string AppFolder = "./GnssData";  
         
        #endregion

        /// <summary>
        /// 命令行参数是以空格符分开的。
        /// </summary>
        /// <param name="args"></param>
        public static void Execute(string[] args, IProgressViewer ProgressViewer = null)
        {
            //首先欢迎
            //ShowInfo("              GDP ——GNSS数据处理软件   ");
            //ShowInfo("-------------   Version 1.0,  www.gnsser.com   --------------------");


            CmdParam cmdParam = CmdParam.ParseParams(args);
            if (cmdParam == null) {
                log.Error("Empty params！");
                //此处应该显示版本helper等
                ShowHelp();
                return;
            }
 
            try
            { 
                //参数初始化
                var startTime = DateTime.Now;
                ProcessCount++;
                ShowInfo("Processing....");
                switch (cmdParam.SolveType)
                {
                    case ProcessType.H:
                        ShowHelp();
                        break;
                    case ProcessType.V:
                        System.Console.WriteLine( "Version: " + Setting.Version.ToString("0.0"));
                        break;
                    case ProcessType.B:
                        BuildUrls(args);
                        break;
                    case ProcessType.D:
                        Download(args);
                        break;
                    case ProcessType.F:
                        FormatConvert(args);
                        break;
                    case ProcessType.S:
                        Select(args);
                        break;
                    case ProcessType.C:                       
                        ConvertToOneTable(args);
                        break;
                    case ProcessType.E:
                        ExtractSiteInfo(args);
                        break;
                    default:
                        log.Warn(cmdParam.SolveType + "Not implemented " );
                        break;
                } 

                ShowInfo("Done！");
                var timeSpan = DateTime.Now - startTime;
                ShowInfo("TimeSpan:" + timeSpan); 
            }
            catch (Exception ex)
            {
                 ShowInfo(ex.Message);
            }

            //暂停屏幕
            //    System.Console.ReadLine();
        }

        private static void ShowHelp()
        {
            if (File.Exists(Setting.HelpDocument))
            {
                var help = File.ReadAllText(Setting.HelpDocument);
                System.Console.WriteLine(help);
            }
        }

        private static void ExtractSiteInfo(string[] args)
        {
            var param = SiteInfoExtractorParam.ParseArgs(args);
            var outDir = param.OutputDirectory;
            List<string> inputPathes = new List<string>();
            foreach (var inputPath in param.InputPath)
            {
                if (FileUtil.IsDirectory(inputPath))
                {
                    if (!Directory.Exists(inputPath)) continue;

                    var list = new List<string>();
                    var files = Directory.GetFiles(inputPath, "*.rnx|*.RNX|*.??o|*.??O");
                    var subDir = PathUtil.GetSubDirectory(inputPath);
                    var dir = Path.Combine(outDir, subDir);
                    FileUtil.CheckOrCreateDirectory(dir);

                    inputPathes.AddRange(files);
                }
                else
                {
                    inputPathes.Add(inputPath);
                }
            }

            SiteInfoExtractor extractor = new SiteInfoExtractor()
            {
                OutputDirectory = param.OutputDirectory
            };
            extractor.Run(inputPathes.ToArray());
        }

        private static void ConvertToOneTable(string[] args)
        {
            ConvertToOneTableParam param;
            string outDir;
            param = ConvertToOneTableParam.ParseArgs(args);
            outDir = param.OutputDirectory;
            foreach (var inputPath in param.InputPath)
            {
                if (FileUtil.IsDirectory(inputPath))
                {
                    if (!Directory.Exists(inputPath)) continue;

                    var list = new List<string>();
                    var files = Directory.GetFiles(inputPath, "*.rnx|*.RNX|*.??o|*.??O");
                    var subDir = PathUtil.GetSubDirectory(inputPath);
                    var dir = Path.Combine(outDir, subDir);
                    FileUtil.CheckOrCreateDirectory(dir);
                    foreach (var file in files)
                    {
                        ConvertToOneTable(dir, file);
                    }
                }
                else
                {
                    ConvertToOneTable(outDir, inputPath);
                }
            }
        }

        private static void ConvertToOneTable(string outDir, string inputPath)
        {
            var fileName = Path.GetFileName(inputPath);
            var outputPath = Path.Combine(outDir, fileName + ".txt.xls");
            var ObsFile = ObsFileUtil.ReadFile(inputPath);
            var table = ObsFile.BuildObjectTable(true);
            ObjectTableWriter.Write(table, outputPath);
        }

        private static void Select(string[] args)
        {
            var param = SelectObsParam.ParseArgs(args);
            var opt = param.Option;
            var outDir = param.OutputDirectory;
            foreach (var inputPath in param.InputPath)
            {
                if (FileUtil.IsDirectory(inputPath))
                {
                    if (!Directory.Exists(inputPath)) continue;

                    var list = new List<string>();
                    var files = Directory.GetFiles(inputPath, "*.rnx|*.RNX|*.??o|*.??O");
                    var subDir = PathUtil.GetSubDirectory(inputPath);
                    var dir = Path.Combine(outDir, subDir);
                    FileUtil.CheckOrCreateDirectory(dir);
                    foreach (var file in files)
                    {
                        Select(opt, dir, file);
                    }
                }
                else
                {
                    Select(opt, outDir, inputPath);
                }
            }
        }

        private static void Select(ObsFileSelectOption opt, string outDir, string inputPath)
        {
            var ObsFileFormater = new ObsFileSelector(opt, outDir);
            // ObsFileFormater.SubDirectory = subDir;
            ObsFileFormater.Init();
            ObsFileFormater.Select(inputPath);
        }

        private static void FormatConvert(string[] args)
        {
            var param = FormatObsParam.ParseArgs(args);
            var opt = param.Option;
            foreach (var inputPath in param.InputPath)
            {
                if (FileUtil.IsDirectory(inputPath))
                {
                    if (!Directory.Exists(inputPath)) continue;

                    var list = new List<string>();
                    var files = Directory.GetFiles(inputPath, "*.rnx|*.RNX|*.??o|*.??O");
                   // var subDir = PathUtil.GetSubDirectory(inputPath);
                    // var dir = Path.Combine(outDir, subDir);
                  //  FileUtil.CheckOrCreateDirectory(dir);
                    foreach (var file in files)
                    {
                        Format(opt, file);
                    }
                }
                else
                {
                    Format(opt, inputPath); 
                }
            }
        }

        private static void Format(ObsFileConvertOption opt, string inputPath)
        {
            ObsFileFormater ObsFileFormater = new ObsFileFormater(opt, inputPath);
            // ObsFileFormater.SubDirectory = subDir;
            ObsFileFormater.Init();
            ObsFileFormater.Run();
        }

        private static void Download(string[] args)
        {
            var param = UrlDownloaderParam.ParseArgs(args);

            if (!File.Exists(param.UrlTextPath))
            {
                log.Error("File not exists " + param.UrlTextPath);
            }
            else
            {
                try
                {
                    bool isOverwrite = param.IsOverWrite;
                    var SaveDir = param.DownloadDirectory;
                    Gdp.Utils.FileUtil.CheckOrCreateDirectory(SaveDir);
                    var FileUrls = File.ReadAllLines(param.UrlTextPath);
                    List<string> failed = new List<string>();
                    int okCount = 0;
                    foreach (string url in FileUrls)
                    {
                        string info = "Downloading successfully!";
                        if (!Gdp.Utils.NetUtil.FtpDownload(url, Path.Combine(SaveDir, Path.GetFileName(url)), isOverwrite))
                        {
                            failed.Add(url);
                            info = "Downloading Filed!";
                        }
                        else
                        {
                            okCount++;
                        }
                        ShowInfo(info + url);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Download Error：" + ex.Message);
                }
            }
        }

        private static void BuildUrls(string[] args)
        {
            var urlBuilderParam = UrlBuilderParam.ParseArgs(args);

            var pathBuilder = new IgsProductUrlPathBuilder(
                urlBuilderParam.UrlDirectories.ToArray(),
                urlBuilderParam.UrlModels.ToArray(),
                urlBuilderParam.Source.ToArray(),
                urlBuilderParam.StartTime.DateTime,
                urlBuilderParam.EndTime.DateTime,
                urlBuilderParam.ProductType,
                urlBuilderParam.IntervalSecond);
            var pathes = pathBuilder.Build();

            File.WriteAllLines(urlBuilderParam.OutputPath, pathes);
        }


        /// <summary>
        /// 输出信息到控制台。
        /// </summary>
        /// <param name="msg"></param>
        static void ShowInfo(string msg)
        {
            log.Info(msg);
          //  System.Console.WriteLine(msg);
        } 

    }
}
