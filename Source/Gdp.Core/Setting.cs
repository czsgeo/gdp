using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gdp
{
   /// <summary>
   /// 通用设置
   /// </summary>
    public  class Setting
    {
        #region 网站服务
        /// <summary>
        /// 网站主页
        /// </summary>
        public static string WebSiteUrl = "http://www.gnsser.com";
        /// <summary>
        /// 汇报 Bug 网址
        /// </summary>
        public static string BugReportUrl = "http://www.gnsser.com/BugReports/Create";
        /// <summary>
        /// 访问网站附带版本信息
        /// </summary>
        public static string WebSiteUrlForNewVersion = "http://www.gnsser.com?version=" + Version + "&type=public&pcuser=" + System.Environment.UserName;
        /// <summary>
        /// 当前版本地址
        /// </summary>
        public static string CurrentVersionUrl = "http://www.gnsser.com/Soft/CurrentPublicVersion";
        /// <summary>
        /// 当前版本的网络路径
        /// </summary>
        public static string CurrentVersionCharacterUrl = "http://www.gnsser.com/Soft/CurrentPublicCharacter";
        #endregion
        /// <summary>
        /// GNSSer 临时目录名称
        /// </summary>
        public static string GnsserTempSubDirectoryName = "GnsserTempOutput";
        /// <summary>
        /// GNSSer工程目录
        /// </summary>
        public static string GnsserProjectDirectoryName = "GnsserProject";
        /// <summary>
        /// 版本说明路径
        /// </summary>
        public static string ImprintPath = @"Data\Documents\Imprint.txt";
        /// <summary>
        /// 帮助文档
        /// </summary>
        public static string HelpDocument = @"Help.txt";


        /// <summary>
        ///  版本
        /// </summary>
        public const  double Version = 1.0;

        /// <summary>
        /// 发现故障的技术支持地址
        /// </summary>
        public static string SupportEmail = "gnsser@163.com"; 
        /// <summary>
        /// 是否启用多系统
        /// </summary>
        public static bool IsEnableMultiSystem = true;
        /// <summary>
        /// 是否启用北斗。
        /// </summary>
        public const bool IsBDsEnabled = false; 

        /// <summary>
        /// 显示关键致命重要的东西
        /// </summary>
        public static bool IsShowFatal = true;
        /// <summary>
        /// 启动日志
        /// </summary>
        public static bool EnableLog = false;
        /// <summary>
        /// 初始是否为调试状态
        /// </summary>
        public static bool IsShowDebug = false;
        /// <summary>
        /// 显示警告,必须设置显示信息才显示
        /// </summary>
        public static bool IsShowWarning = true;
        /// <summary>
        /// 是否显示执行过程中的错误,必须设置显示信息才显示
        /// </summary>
        public static bool IsShowError = true;
        /// <summary>
        /// 显示信息，显示必要的信息，包含处理中的警告和错误，同时控制日志记录。如果未来加快速度可以不显示。
        /// </summary>
        public static bool IsShowInfo = true;

        /// <summary>
        /// 是否允许网络访问
        /// </summary>
        public static bool EnableNet = true;

        /// <summary>
        /// 网络是否可以访问，包括局域网。
        /// </summary>
        public static bool IsNetEnabled { get { return EnableNet && Gdp.Utils.NetUtil.IsConnected(); } }
        /// <summary>
        /// 网络是否可以访问 Internet
        /// </summary>
        public static bool IsInternetEnabled { get { return EnableNet && Gdp.Utils.NetUtil.IsConnectedToInternet(); } }

        /// <summary>
        /// 初始化程序
        /// </summary>
        public static void Init()
        {
            //TryClearTempDirectory();
        }
        /// <summary>
        /// crx2rnx.exe 路径
        /// </summary>
        public static string PathOfCrx2rnx = "\"" + Path.Combine(Setting.ExeFolder, "crx2rnx.exe") + "\"";



        /// <summary>
        /// 对话框打开的表文件后缀
        /// </summary>
        public static string GnsserTextTableFileFilter = "文本表格文件(*.txt.xls)|*.txt.xls";


        /// <summary>
        /// 对话框打开的表文件后缀
        /// </summary>
        public static string TextTableFileFilter = "文本表格文件(*.txt.xls;*.csv;*.txt;*.xls)|*.txt.xls;*.csv;*.txt;*.xls|所有文件(*.*)|*.*";
        /// <summary>
        /// 表文件后缀 .txt.xls
        /// </summary>
        public static string TextTableFileExtension = ".txt.xls"; 
        /// <summary>
        ///表文件默认分割
        /// </summary>
        public static string [] DefaultTextTableSpliter = new string[] { "\t"}; 
        public static string GlonassSlotFreqFile = @"Data\GlonassSlotFreq.txt.xls";
        public static string RinexOFileFilter = "O Files|*.??O;*.rnx;*.Obs|All Files|*.*";
        public static string RinexEphFileFilter = "星历文件|*.eph;*.sp3;*.??n;*.??p;*.??C|所有文件|*.*";
        public static string SampleOFile= @"Data\hers0010.18O";

        /// <summary>
        /// 程序根目录
        /// </summary>
        static public string ApplicationDirectory
        { get { return AppDomain.CurrentDomain.BaseDirectory; } }
       // Path.GetDirectoryName( System.Reflection.Assembly.Load("Geo").Location); } }
                
        // System.Reflection.Assembly.GetExecutingAssembly().Location;
        // Environment.CurrentDirectory; } }
        // AppDomain.CurrentDomain.BaseDirectory; } }
        /// <summary>
        /// EXE 目录
        /// </summary>
        public static string ExeFolder { get { return Path.Combine(DataDirectory, "Exe"); } } 
        /// <summary>
        /// 数据目录
        /// </summary>
        static public string DataDirectory { get { return Path.Combine(ApplicationDirectory, "Data"); } }
        /// <summary>
        /// 临时目录，程序启动或退出时将清空
        /// </summary>
        static  public string TempDirectory { get { return Path.Combine(ApplicationDirectory, @"Temp"); } }

        public static Dictionary<SatelliteNumber, double> GlonassSlotFrequences
            = new Dictionary<SatelliteNumber, double>
        {
            { new SatelliteNumber(1, SatelliteType.R), 1 },
            { new SatelliteNumber(2, SatelliteType.R), -4 },
            { new SatelliteNumber(3, SatelliteType.R), 5 },
            { new SatelliteNumber(4, SatelliteType.R), 6 },
            { new SatelliteNumber(5, SatelliteType.R), 1 },
            { new SatelliteNumber(6, SatelliteType.R), -4 },
            { new SatelliteNumber(7, SatelliteType.R), 5 },
            { new SatelliteNumber(8, SatelliteType.R), 6 },
            { new SatelliteNumber(9, SatelliteType.R), -2 },
            { new SatelliteNumber(10, SatelliteType.R), -7 },
            { new SatelliteNumber(11, SatelliteType.R), 0 },
            { new SatelliteNumber(12, SatelliteType.R), -1 },
            { new SatelliteNumber(13, SatelliteType.R), -2 },
            { new SatelliteNumber(14, SatelliteType.R), -7 },
            { new SatelliteNumber(15, SatelliteType.R), 0 },
            { new SatelliteNumber(16, SatelliteType.R), -1 },
            { new SatelliteNumber(17, SatelliteType.R), 4 },
            { new SatelliteNumber(18, SatelliteType.R), -3 },
            { new SatelliteNumber(19, SatelliteType.R), 3 },
            { new SatelliteNumber(20, SatelliteType.R), 2 },
            { new SatelliteNumber(21, SatelliteType.R), 4 },
            { new SatelliteNumber(22, SatelliteType.R), -3 },
            { new SatelliteNumber(23, SatelliteType.R), 3 },
            { new SatelliteNumber(24, SatelliteType.R), 2},
            { new SatelliteNumber(26, SatelliteType.R), -5 },
        };
        public static string[] IgsProductUrlModels = new string[]
        {
            "{UrlDirectory}/{FileDirectory}/{FileName}",
        };
        public static string[] IgsProductUrlDirectories = new string[]
        {
            "ftp://igs.gnsswhu.cn/pub/gps/products",
            "ftp://cddis.gsfc.nasa.gov/pub/gps/products",
            "ftp://igs.gnsswhu.cn/pub/gps/products/mgex",
            "ftp://igs.ensg.eu/pub/igs/products",
            "ftp://igs.ign.fr/pub/igs/products"
        };
        public static string[] IgsProductLocalDirectories = new string[]
        {
            @"D:\Data\IgsProduct",
            @"C:\Data\IgsProduct",
            @"E:\Data\IgsProduct",
        };

        /// <summary>
        /// 尝试清空临时目录
        /// </summary>
        static public void TryClearTempDirectory()
        {
             Gdp.Utils.FileUtil.TryClearDirectory(TempDirectory);
        }
    }
}
