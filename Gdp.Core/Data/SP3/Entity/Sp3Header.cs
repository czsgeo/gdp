//2014.12.27, lly, edit, time < satAccLen 修改为 j < satAccLen ？？
//2016.01.20, double,edit,修改头文件读取流程
//2018.03.16, czs, edit in hmx, 增加数据源名称，通常为文件名称，如igs19921.sp3

using System;
using System.Collections.Generic;
using System.Text;
using System.IO; 


namespace Gdp.Data.Rinex
{
    /// <summary>
    /// Sp3 头文件。
    /// </summary>
    public class Sp3Header
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sp3Header()
        {
            DataUsed = "Nav";
            CoordinateSystem = "Nav";
            OrbitType = "HLM";
            AgencyName = "GNSr";

            VersionId = "c";

        }
         
        #region 属性
        /// <summary>
        /// 下标对应行号 － 1.
        /// </summary>
        public static string[] LineSymbols = {
            //1-22是头文件部分
             "#", "##", "+ ", "+ ", "+ ", 
             "+ ", "+ ","++","++","++",
             "++","++","%c","%c","%f",
             "%f", "%i", "%i","/*","/*",
             "/*","/*",

             "* ",//Sp3 LINE 23 (epoch header clk)
             "P",//Sp3 LINE 24 (position and clock clk)
             "V", //Sp3 LINE 25 (velocity and clock clk)

             "EOF"//Sp3 LINE 22+numeps*(numsats+1)+1 (last line)
    };
        /// <summary>
        /// 下标对应行号 － 1.
        /// </summary>
        public static string[] LineSymbolsVersionC = {
            //1-32是头文件部分
             "#", "##", "+ ", "+ ", "+ ", 
             "+ ", "+ ", "+ ", "+ ", "+ ", 
             "+ ", "+ ","++","++","++",
             "++","++","++","++","++",
             "++","++","%c","%c","%f",
             "%f", "%i", "%i","/*","/*",
             "/*","/*",

             "* ",//Sp3 LINE 33 (epoch header clk)
             "P",//Sp3 LINE 34 (position and clock clk)
             "V", //Sp3 LINE 35 (velocity and clock clk)

             "EOF"//Sp3 LINE 32+numeps*(numsats+1)+1 (last line)
    };
        /// <summary>
        /// 数据源名称，通常为文件名称，如igs19921.sp3
        /// </summary>
        public string SourceName { get; set; }
        // Columns 1-2         Version Symbol     #c                  A2
        /// <summary>
        ///版本号
        /// </summary>
        public string VersionId { get; set; }
        //Column  3           Pos or Vel Flag    P or V              A1
        /// <summary>
        /// 指示是位置坐标还是速度
        /// </summary>
        public string P_V_ModeFlag { get; set; }
        /*
         *
         Columns 4-7         Year Start         2001                I4          
         Column  8           Unused             _                   blank
         Columns 9-10        Month Start        _8                  I2
         Column  11          Unused             _                   blank 
         Columns 12-13       Day of Month St    _8                  I2
         Column  14          Unused             _                   blank
         Columns 15-16       Hour Start         _0                  I2
         Column  17          Unused             _                   blank
         Columns 18-19       Minute Start       _0                  I2
         Column  20          Unused             _                   blank 
         Columns 21-31       Second Start       _0.00000000         F11.8
         */
        /// <summary>
        /// 起始时间
        /// </summary>
        public Time StartTime { get; set; }


        /// <summary>
        /// 设置历元时间
        /// </summary>
        /// <param name="startTime"></param>
        public void SetStartTime(Time startTime)
        {
            StartTime = startTime;
            GPSWeek = startTime.GpsWeek;
            SecondsOfWeek = startTime.SecondsOfWeek;
            //ModJulianDayStart = (int)startTime.MJulianDays_Double;
            //FractionalDay = startTime.MJulianDays_Double - ModJulianDayStart;
        }


        /// <summary>
        /// 历元数量
        /// </summary>
        public int NumberOfEpochs { get; set; }
        //  Columns 41-45       Data Used          ____d               A5
        /// <summary>
        /// 使用的数据
        /// </summary>
        public string DataUsed { get; set; }
        //  Columns 47-51       Coordinate Sys     ITR97               A5
        /// <summary>
        /// 坐标系统
        /// </summary>
        public string CoordinateSystem { get; set; }
        // Columns 53-55       Orbit Type         FIT                 A3
        /// <summary>
        /// 轨道类型
        /// </summary>
        public string OrbitType { get; set; }
        //Columns 57-60       Agency             _NGS                A4
        /// <summary>
        /// 机构名称
        /// </summary>
        public string AgencyName { get; set; }

        /// <summary>
        /// 计算出的结束时间
        /// </summary>
        public Time EndTime
        {
            get
            {
                return StartTime + TimeSpan.FromSeconds(EpochInterval * (NumberOfEpochs -1));
            }
        }

        //Line 2
        /// <summary>
        /// GPS 周
        /// </summary>
        public int GPSWeek { get; set; }
        /// <summary>
        /// GPS 周秒
        /// </summary>
        public double SecondsOfWeek { get; set; }
        /// <summary>
        /// 单位 S
        /// </summary>
        public double EpochInterval { get; set; }
        /// <summary>
        /// 轨道数据首历元儒略日的整数部分。
        /// </summary>
        public int ModJulianDayStart { get; set; }
        /// <summary>
        /// 轨道数据首历元儒略日的小数部分。
        /// </summary>
        public double FractionalDay { get; set; }
        int _NumberOfSatellites;
        //Line 3-4-7
        /// <summary>
        /// 卫星数量
        /// </summary>
        public int NumberOfSatellites {
            get { if (_NumberOfSatellites == 0 && PRNs != null) return PRNs.Count; return _NumberOfSatellites; }
            set { _NumberOfSatellites = value; } }
        /// <summary>
        /// 卫星编号
        /// </summary>
        public List<SatelliteNumber> PRNs { get; set; }

        //Line 8-12
        /// <summary>
        /// 用于表示卫星的精度，1表示极佳， 99表示不要用，0表示未知。
        /// </summary>
        public Dictionary<SatelliteNumber, int> SatAccuraces { get; set; }

        //Columns 1-2         Symbols            %c                  A2
        /// <summary>
        /// 文件类型，M表示多系统，版本c新增
        /// </summary>
        public string FileType { get { return Characters[0]; } set { Characters[0] = value; } }
        //Columns 1-2         Symbols            %c                  A2
        /// <summary>
        /// 时间系统，版本c新增
        /// </summary>
        public string TimeSystem { get { return Characters[2];  } set { Characters[2] = value; } }
        /// <summary>
        /// Base for Pos/Vel，版本c新增
        /// </summary>
        public double BaseForPosOrVel { get { return Floats[0];  } set { Floats[0] = value; } }
        /// <summary>
        /// Base for Clk/Rate，版本c新增
        /// </summary>
        public double BaseForClkOrRate { get { return Floats[1];  } set { Floats[1] = value; } }

        //Sp3 LINES 13-14
        /// <summary>
        /// 存储的字符，13-14行存储
        /// </summary>
        public List<string> Characters { get; set; }

        //Sp3 LINES 15-16
        /// <summary>
        /// 存储的浮点数
        /// </summary>
        public List<double> Floats { get; set; }

        //Sp3 LINES 17-18
        /// <summary>
        /// 存储的整数区
        /// </summary>
        public List<int> Ints { get; set; }

        //Sp3 LINES 19-22
        /// <summary>
        /// 注释
        /// </summary>
        public List<string> Comments { get; set; }
        /// <summary>
        /// 头文件行数      2016.01.20 double添加
        /// </summary>
        public int LineNumber { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 获取卫星精度信息,如果没有，则返回0，表示未知。
        /// 用于表示卫星的精度，1表示极佳， 99表示不要用，0表示未知。
        /// </summary>
        /// <param name="prn"></param>
        /// <returns></returns>
        public int GetSatSatAccurace(SatelliteNumber prn)
        {
            if (SatAccuraces.ContainsKey(prn))
            {
                return SatAccuraces[prn];
            }
            return 0;
        }

        /// <summary>
        /// 读取头部信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Sp3Header Read(string path)
        {
            Sp3Header header = new Sp3Header();
            header.SourceName = Path.GetFileName(path);
            using (TextReader r = new StreamReader(path))
            {
                string line = null;

                string symbol = null;
                line = r.ReadLine();
                symbol = line.Substring(0, 1);
                if (symbol != LineSymbols[0]) { throw new Exception("SP3文件不合法"); }

                header.VersionId = line.Substring(1, 1);
                header.P_V_ModeFlag = line.Substring(2, 1);
                header.StartTime = Time.Parse(line.Substring(3, 28));
                header.NumberOfEpochs = int.Parse(line.Substring(32, 7));
                header.DataUsed = line.Substring(40, 5);
                header.CoordinateSystem = line.Substring(46, 5);
                header.OrbitType = line.Substring(52, 3);
                header.AgencyName = line.Substring(56, 4);


                int lineIndex = 0;
                int sartLineIndexOfSat = 0;
                int sartLineIndexOfSatAccuracy = 0;
                do
                {
                    line = r.ReadLine();
                    lineIndex++;

                    switch (FirstTwoCharToMarker(line))
                    {
                        case  Sp3HeaderLineMarker.Time:
                            header.GPSWeek = int.Parse(line.Substring(3, 4));
                            header.SecondsOfWeek = double.Parse(line.Substring(9, 15));
                            header.EpochInterval = double.Parse(line.Substring(24, 14));
                            header.ModJulianDayStart = int.Parse(line.Substring(39, 5));
                            header.FractionalDay = double.Parse(line.Substring(45, 15));                     
                            break;
                        case Sp3HeaderLineMarker.SatelliteCount:
                            sartLineIndexOfSat = lineIndex;
                            header.NumberOfSatellites = int.Parse(line.Substring(4, 2));

                            int len = header.NumberOfSatellites >= 17 ? 17 : header.NumberOfSatellites;

                            header.PRNs = SatelliteNumber.ParsePRNs(line.Substring(9, len * 3));
                            break;
                        //都用于存储卫星号。
                        case  Sp3HeaderLineMarker.SatelliteNumber:
                            int prevSatCount = 17 * (lineIndex - sartLineIndexOfSat);
                            int satLen = header.NumberOfSatellites >= prevSatCount + 17 ? 17 : header.NumberOfSatellites - prevSatCount;

                            if (satLen > 0) header.PRNs.AddRange(SatelliteNumber.ParsePRNs(line.Substring(9, satLen * 3)));
                            break;
                        //用于表示卫星的精度，1表示极佳， 99表示不要用，0表示未知。
                        case  Sp3HeaderLineMarker.Accuracy:
                            if (sartLineIndexOfSatAccuracy == 0) { sartLineIndexOfSatAccuracy = lineIndex; }
                            if (header.SatAccuraces == null) header.SatAccuraces = new Dictionary<SatelliteNumber, int>();

                            int prevSatAccuCount = 17 * (lineIndex - sartLineIndexOfSatAccuracy);
                            int satAccLen = header.NumberOfSatellites >= prevSatAccuCount + 17 ? 17 : header.NumberOfSatellites - prevSatAccuCount;
                            if (satAccLen > 0)
                            {
                                List<string> sts = Utils.StringUtil.Split(line.Substring(9, satAccLen * 3), 3);
                                for (int i = prevSatAccuCount, j = 0; j < satAccLen; i++, j++)
                                {
                                    header.SatAccuraces.Add(header.PRNs[i], int.Parse(sts[j]));
                                }
                            }
                            break;
                        case  Sp3HeaderLineMarker.Char:
                            if (header.Characters == null) header.Characters = new List<string>();
                            header.Characters.AddRange(new List<String>(line.Substring(3, 57).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
                            break;

                        case  Sp3HeaderLineMarker.Float:
                            if (header.Floats == null) header.Floats = new List<double>();
                            List<String> list = new List<String>(line.Substring(3, 57).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                            foreach (string Str in list)
                            {
                                header.Floats.Add(double.Parse(Str));
                            }
                            break;

                        case  Sp3HeaderLineMarker.Int:
                            if (header.Ints == null) header.Ints = new List<int>();
                            List<String> listINts = new List<String>(line.Substring(3, 57).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                            foreach (string Str in listINts)
                            {
                                header.Ints.Add(int.Parse(Str));
                            }
                            break;
                        case  Sp3HeaderLineMarker.Comment:
                            if (header.Comments == null) header.Comments = new List<string>();
                            if (line.Length > 3) header.Comments.Add(line.Substring(3));
                            break;
                        case  Sp3HeaderLineMarker.End:
                            header.LineNumber = lineIndex;
                            return header;
                           // break;
                        default:
                            throw new ApplicationException("天啊！你是不可能看到我的！除非出错了，难道是版本变了？");
                    }
                } while (String.Compare(line.Substring(0, 2), "* ", true) != 0);
                header.LineNumber = lineIndex+1;
            }
            return header;
        }

        /// <summary>
        /// 显示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "StartTime:" + StartTime + "\r\n"
                + "历元数量：" + NumberOfEpochs + "\r\n"
                + "CoordinateSystem：" + CoordinateSystem + "\r\n"
                + "FileType：" + FileType + "\r\n"
                + "TimeSystem：" + TimeSystem + "\r\n"
                + "DataUsed：" + DataUsed + "\r\n"
                + "OrbitType：" + OrbitType + "\r\n"
                + "AgencyName：" + AgencyName + "\r\n"
                + "GPSWeek：" + GPSWeek + "\r\n"
                ;
        }
     
        /// <summary>
        /// 前两个字符标识
        /// </summary>
        /// <param name="str1"></param>
        /// <returns></returns>
        private static Sp3HeaderLineMarker FirstTwoCharToMarker(string str1)
        {
            var firstTwoChar = str1.Substring(0, 2);

            switch (firstTwoChar)
            {
                case "##":
                    return Sp3HeaderLineMarker.Time;
                case "+ ":
                    if (String.Compare(str1.Substring(4, 2), "  ", true) != 0)
                        return Sp3HeaderLineMarker.SatelliteCount;
                    return Sp3HeaderLineMarker.SatelliteNumber;
                case "++":
                    return Sp3HeaderLineMarker.Accuracy;
                case "%c":
                    return Sp3HeaderLineMarker.Char;
                case "%f":
                    return Sp3HeaderLineMarker.Float;
                case "%i":
                    return Sp3HeaderLineMarker.Int;
                case "/*":
                    return Sp3HeaderLineMarker.Comment;
                case "* ":
                    return Sp3HeaderLineMarker.End;
                default: throw new Exception("SP3文件不合法"); ;
            }
        }
        #endregion
    }

    /// <summary>
    /// 头文件行标记
    /// </summary>
    public enum Sp3HeaderLineMarker
    {
        /// <summary>
        /// #a/c
        /// </summary>
        Version = 0,
        /// <summary>
        /// ##
        /// </summary>
        Time = 1,
        /// <summary>
        /// +
        /// </summary>
        SatelliteCount = 2,
        /// <summary>
        /// +
        /// </summary>
        SatelliteNumber = 4,
        /// <summary>
        /// ++
        /// </summary>
        Accuracy = 5,
        /// <summary>
        /// %c
        /// </summary>
        Char = 6,
        /// <summary>
        /// %f
        /// </summary>
        Float = 7,
        /// <summary>
        /// %i
        /// </summary>
        Int = 8,
        /// <summary>
        /// /*
        /// </summary>
        Comment = 9,
        /// <summary>
        /// * 内容的开始
        /// </summary>
        End = 10,
    }
}
