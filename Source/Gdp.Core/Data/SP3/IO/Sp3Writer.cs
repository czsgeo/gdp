//2016.03.14, double, create in Zhengzhou
//2017.04.24, czs, edit in xi'an, 增加两个方法Flush 
//2017.05.06, double, edit in hongqing, 修改头文件输出
//2017.07.13, czs, 修复 头部注释输出无换行符
//2018.10.28, czs, edit in hmx, 定轨输出修改

using System; 
using System.Collections.Generic; 
using System.Text;
using System.IO;
using Gdp.Utils;

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// Rinex 观测文件读取器
    /// </summary>
    public class Sp3Writer : IDisposable
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="Sp3Header">头部信息</param>
        public Sp3Writer(string filePath, Sp3Header Sp3Header =null)
        {
            this.FilePath = filePath;
            this.Sp3Header = Sp3Header;
            if (Sp3Header == null)
            {
                this.Sp3Header = new Sp3Header
                {
                    //StartTime = DateTime.Now,
                    //VersionId = 3.02.ToString(),
                    AgencyName = "Gnsser Group"
                };
            }
            this.StringBuilder = new StringBuilder();
            HeaderText = BuildHeaderString(this.Sp3Header);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="Sp3File"></param>
        /// <param name="P_V_ModeFlag"></param>
        public Sp3Writer(string filePath, Sp3File Sp3File = null, string P_V_ModeFlag = "P")
        {
            this.FilePath = filePath;
            this.StringBuilder = new StringBuilder();
            if (Sp3File != null)
            {
                this.Sp3Header = Sp3File.Header;
                this.StringBuilder.Append(BuidSp3V3String(Sp3File, P_V_ModeFlag));
            }
        }

        #region 属性
        /// <summary>
        /// Sp3头文件。
        /// </summary>
        public Sp3Header Sp3Header { get; set; }
        /// <summary>
        /// 头部文字
        /// </summary>
        public string HeaderText { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 数据流
        /// </summary>
        protected StringBuilder StringBuilder { get; set; }
        #endregion

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="section"></param>
        public void Write(Sp3Section section)
        {
            //如果没有内容，且文件不存在，则先写入头部
            if(StringBuilder.Length == 0 && !File.Exists(FilePath))
            {
                this.StringBuilder.Append(HeaderText);
            }

            StringBuilder.AppendLine(BuildFirstLineSp3(section));
            foreach (var record in section)
            {
                StringBuilder.Append(BuildSp3Record(record,"V")); 
            }
        }
        private void Write(Ephemeris record)
        {
            StringBuilder.Append(BuildSp3Record(record)); 
        }
        /// <summary>
        /// 写入，并清空。
        /// </summary>
        public void Flush()
        {
            if (StringBuilder.Length > 0)
            { 
                File.AppendAllText(FilePath, StringBuilder.ToString());

                StringBuilder.Clear();
            }
        }

        /// <summary>
        /// 保存到文件，并清空缓存。此处采用追加的方式保存，可以多次调用此方法。
        /// </summary>
        public void SaveToFile()
        {
            Flush();
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            Flush();
        }

        #region Sp3文件的写
        #region 头文件
        /// <summary>
        /// 构建头部字符串。
        /// </summary>
        /// <param name="header"></param>
        /// <param name="P_V_ModeFlag"></param>
        /// <returns></returns>
        public static string BuildHeaderString(Sp3Header header,string P_V_ModeFlag = "P")
        {
            StringBuilder sb = new StringBuilder();

            //prevObj line
            if (P_V_ModeFlag == "P")
                sb.Append(StringUtil.FillSpace("#cP", 3));
            else sb.Append(StringUtil.FillSpace("#cV", 3));
            sb.Append(header.StartTime.Year  //四位数的年
                 + " " + header.StartTime.Month.ToString("00")
                + " " + header.StartTime.Day.ToString("00")
                + " " + header.StartTime.Hour.ToString("00")
                + " " + header.StartTime.Minute.ToString("00")
                + " " + header.StartTime.Second.ToString("00.00000000"));
            //sb.Append(StringUtil.FillSpace(DateTime.UtcNow.ToString("yyyyMMdd HHmmss") + " UTC", 20));


            //header.StartTime = Time.Parse(line.Substring(3, 28));
            //header.NumberOfEpochs = int.Parse(line.Substring(32, 7));
            //header.DataUsed = line.Substring(40, 5);
            //header.CoordinateSystem = line.Substring(46, 5);
            //header.OrbitType = line.Substring(52, 3);
            //header.AgencyName = line.Substring(56, 4);


            sb.Append(StringUtil.FillSpaceLeft(header.NumberOfEpochs.ToString(), 8));
            sb.Append(" ");
            sb.Append(StringUtil.FillSpaceLeft(header.DataUsed, 5));
            sb.Append(" ");
            sb.Append(StringUtil.FillSpaceLeft(header.CoordinateSystem, 5));
            sb.Append(" ");
            sb.Append(StringUtil.FillSpaceLeft(header.OrbitType, 3));
            sb.Append(" ");
            sb.Append(StringUtil.FillSpaceLeft(header.AgencyName, 4));   

            sb.AppendLine();

            //second line
            sb.Append(StringUtil.FillSpace("##", 2)+" ");
            sb.Append(StringUtil.FillSpace(header.GPSWeek.ToString(), 4)+"  ");
            sb.Append(StringUtil.FillSpace(header.SecondsOfWeek.ToString(".00000000"), 15));
            sb.Append(StringUtil.FillSpace(header.EpochInterval.ToString(".00000000"), 15));
            sb.Append(StringUtil.FillSpace(header.ModJulianDayStart.ToString(), 5)+" ");
            sb.Append(StringUtil.FillSpace(header.FractionalDay.ToString("0.0000000000000"), 15));
            sb.AppendLine();

            //存储卫星号
            sb.Append(StringUtil.FillSpace("+", 4));
            sb.Append(StringUtil.FillSpace(header.NumberOfSatellites.ToString(), 5));

            var prns = header.PRNs;
            int satIndex = 0;
            int line = 0;
            if (prns != null)
            {
                foreach (var item in prns)
                {
                    sb.Append(item.ToString());
                    satIndex++;
                    if (satIndex % 17 == 0)
                    {
                        sb.AppendLine();
                        line++;
                        if (line < 10)
                            sb.Append(StringUtil.FillSpace("+", 9));
                    }
                }
            }
            for (int i = 0; i < 170 - header.NumberOfSatellites; i++)
            {
                sb.Append(StringUtil.FillSpace("  0", 3));
                satIndex++;
                if (satIndex % 17 == 0)
                {
                    sb.AppendLine();
                    line++;
                    if (line < 10)
                        sb.Append(StringUtil.FillSpace("+", 9));
                }
            }
            int satIndex1 = 0;
            for (int i = 0; i < 10; i++)
            {
                sb.Append(StringUtil.FillSpace("++", 9));

                for (int j = 0; j < 17; j++)
                {
                    if (satIndex1 < header.NumberOfSatellites)
                        sb.Append(StringUtil.FillSpace("  2", 3));
                    else
                        sb.Append(StringUtil.FillSpace("  0", 3));
                    satIndex1++;
                }
                sb.AppendLine();
            }

            sb.Append(StringUtil.FillSpace("%c", 2));
            sb.Append("    cc GPS ccc cccc cccc cccc cccc ccccc ccccc ccccc ccccc");sb.AppendLine();
            sb.Append(StringUtil.FillSpace("%c", 2));
            sb.Append(" cc cc ccc ccc cccc cccc cccc cccc ccccc ccccc ccccc ccccc"); sb.AppendLine();
            sb.Append(StringUtil.FillSpace("%f", 2));
            sb.Append("  0.0000000  0.000000000  0.00000000000  0.000000000000000"); sb.AppendLine();
            sb.Append(StringUtil.FillSpace("%f", 2));
            sb.Append("  0.0000000  0.000000000  0.00000000000  0.000000000000000"); sb.AppendLine();
            sb.Append(StringUtil.FillSpace("%i", 2)); 
            sb.Append("    0    0    0    0      0      0      0      0         0");sb.AppendLine();
            sb.Append(StringUtil.FillSpace("%i", 2)); 
            sb.Append("    0    0    0    0      0      0      0      0         0"); sb.AppendLine();
            sb.Append(StringUtil.FillSpace("/*", 3));
            sb.AppendLine("Gnsser Group");
            sb.Append(StringUtil.FillSpace("/*", 3)); sb.AppendLine();
            if (header.Comments != null)
            {
                foreach (var item in header.Comments)
                {
                    if (item == "Gnsser Group") continue;
                    sb.Append(StringUtil.FillSpace("/*", 3));
                    sb.AppendLine(StringUtil.FillSpaceRight(item, 60)); 
                }
            }
            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// 将指定的Sp3转换成 RINEX V3.0字符串。
        /// </summary>
        /// <param name="file"></param>
        /// <param name="P_V_ModeFlag"></param>
        /// <returns></returns>
        public static string BuidSp3V3String(Sp3File file, string P_V_ModeFlag = "P")
        {
            StringBuilder sb = new StringBuilder();
            file.Header.SetStartTime( file.First.Time);
            file.Header.NumberOfEpochs = file.Count;
            sb.Append(BuildHeaderString(file.Header, P_V_ModeFlag));

            //Sp3Section sec = new Sp3Section();
            var prns = file.Header.PRNs;
            if(prns == null)
            {
                prns = file.First.Keys;
                prns.Sort();
            }

            foreach (var item in file.Data.Values)
            {
                sb.AppendLine(BuildFirstLineSp3(item));
                foreach (var prn in prns)
                { 
                    Ephemeris sat=new Ephemeris ();
                    if (item.Contains(prn))
                        sat = item[prn];
                    else 
                    {
                        sat.Prn = prn;
                        sat.XYZ = new XYZ();

                        sat.ClockBias = 0.999999999999; 
                    }
                    sb.Append(BuildSp3Record(sat, P_V_ModeFlag));

                }
                //foreach (Sp3Record rec in key.Data.Values)
                //{
                //    sb.Append(BuildSp3Record(rec, P_V_ModeFlag));
                //}
                
            }
            sb.Append("EOF");
            return sb.ToString();
        }

        /// <summary>
        /// 构建第一行
        /// </summary>
        /// <param name="Sp3Section"></param>
        /// <returns></returns>
        public static string BuildFirstLineSp3(Sp3Section Sp3Section)
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append("*  ");
            sb.Append(Sp3Section.Time.Year  //四位数的年
                 + " " + Sp3Section.Time.Month.ToString("00")
                + " " + Sp3Section.Time.Day.ToString("00")
                + " " + Sp3Section.Time.Hour.ToString("00")
                + " " + Sp3Section.Time.Minute.ToString("00")
                + " " + Sp3Section.Time.Second.ToString("00.00000000"));
            return sb.ToString();
        }

        /// <summary>
        /// 构建历元数据
        /// </summary>
        /// <param name="Sp3Record"></param>
        /// <param name="P_V_ModeFlag"></param>
        /// <returns></returns>
        public static string BuildSp3Record(Ephemeris Sp3Record, string P_V_ModeFlag="P")
        {            
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(BuildFirstLineSp3(Sp3Record));
            
            sb.Append("P"+ Sp3Record.Prn.ToString());
            sb.Append(StringUtil.FillSpaceLeft((Sp3Record.XYZ.X/1000).ToString("0.000000"), 14));
            sb.Append(StringUtil.FillSpaceLeft((Sp3Record.XYZ.Y/1000).ToString("0.000000"), 14));
            sb.Append(StringUtil.FillSpaceLeft((Sp3Record.XYZ.Z/1000).ToString("0.000000"), 14));
            sb.Append(StringUtil.FillSpaceLeft((Sp3Record.ClockBias*1000000).ToString("0.000000"), 14));
            if (Sp3Record.Rms != null)
            {
                //sb.Append(StringUtil.FillSpaceLeft(Sp3Record.XyzSdev.X.ToString(), 2));
                //sb.Append(StringUtil.FillSpaceLeft(Sp3Record.XyzSdev.Y.ToString(), 2));
                //sb.Append(StringUtil.FillSpaceLeft(Sp3Record.XyzSdev.Z.ToString(), 2));
            }
            //if (Sp3Record.ClockSdev!=null)
                //sb.Append(StringUtil.FillSpaceLeft(Sp3Record.ClockSdev.ToString(), 2));
            sb.AppendLine();
            if (P_V_ModeFlag == "V")
            {
                sb.Append("EP");
                sb.AppendLine();
                sb.Append("V" + Sp3Record.Prn.ToString());
                sb.Append(StringUtil.FillSpaceLeft((Sp3Record.XyzDot.X * 1000).ToString("0.000000"), 14));
                sb.Append(StringUtil.FillSpaceLeft((Sp3Record.XyzDot.Y * 1000).ToString("0.000000"), 14));
                sb.Append(StringUtil.FillSpaceLeft((Sp3Record.XyzDot.Z * 1000).ToString("0.000000"), 14));
                sb.Append(StringUtil.FillSpaceLeft((Sp3Record.ClockDrift * 1e12).ToString("0.000000"), 14));
                sb.AppendLine();
                sb.Append("EV");
                sb.AppendLine();
 
            }
            return sb.ToString();
        }
        #endregion    
    }
}