//2016.01.20, czs, edit in honqqing,  增加c版本读取
//2016.03.06, czs, edit in hongqing, 数据流读取
//2016.10.16, double ,add in hongqing,增加了#cV格式的sp3文件读取

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gdp.IO;

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// Sp3 记录部分。
    /// </summary>
    public class Sp3Reader :AbstractTimedStreamReader<Sp3Section>, IDisposable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        public Sp3Reader(string filePath, bool IsAvailableOnly = true)
            : base(filePath)
        {
            Header = Sp3Header.Read(InputPath);
            FilterdPrn = SatelliteNumber.Default;
            this.IsAvailableOnly = IsAvailableOnly;
        }
        /// <summary>
        /// 是否只读取合法数据
        /// </summary>
        public bool IsAvailableOnly { get; set; }
        /// <summary>
        /// 头部信息。
        /// </summary>
        public Sp3Header Header { get; set; }
        /// <summary>
        /// 过滤PRN，，只读取这个，可以写为数组？？或增加系统类型？？2016.03.06
        /// </summary>
        public SatelliteNumber FilterdPrn { get; set; }
        /// <summary>
        /// 读取文件
        /// </summary> 
        /// <returns></returns>
        public Sp3File ReadAll()
        {
            this.Reset();

            Sp3File file = new Sp3File();
            file.Name = Path.GetFileName(InputPath);
            file.Header = Header;

            //Time start = Time.MaxValue;
            //Time end = Time.MinValue;
            while (this.MoveNext())
            {
                var section = this.Current;

                if (section != null && section.Count != 0)
                {
                    file.Add( section); 

                    //if (section.Time < start) start = section.Time;
                    //if (section.Time > end) end = section.Time;
                }
            }

            //file.TimePeriod = new BufferedTimePeriod(start, end);

            log.Info("完全加载了星历文件到内存 " +  Name);
            return file; 
        }
        Time startTime = Time.MaxValue;
        /// <summary>
        /// 从文件开始获取时间
        /// </summary>
        /// <returns></returns>
        public override Time TryGetStartTime()
        {
            if (startTime != Time.MaxValue)
            { return startTime; }
             
            this.Reset();
            while (this.MoveNext())
            { 
                startTime = Current.Time;
                return startTime;
            }
            //StreamReader.BaseStream.Position = pos;
            return startTime;
        }

        Time endTime = Time.MinValue;
        /// <summary>
        /// 从文件末尾获取时间
        /// </summary>
        /// <returns></returns>
        public override Time TryGetEndTime()
        { 
            if (endTime != Time.MinValue)
            {
                return endTime;
            }

            try
            {
                endTime = this.Header.EndTime;
                return endTime;
            }
            catch (Exception ex)
            {
                log.Error("获取Sp3结束时间失败！将尝试其他方法。" + ex.Message);
            }


            this.Reset();
             
            //30颗卫星 5000有2个多历元
            var differ = StreamReader.BaseStream.Length > 5000 ? 5000 : StreamReader.BaseStream.Length;

            StreamReader.BaseStream.Seek(-differ, SeekOrigin.End);
            string line = null;
            while ((line = StreamReader.ReadLine()) != null)
            {
                if(IsEpochSectionFirstLine(line)){
                     endTime = ParseTime(line);
                } 
            }
              
            return endTime;
        }
        /// <summary>
        /// 移动到下一个，尝试解析，如果到末尾了，则返回false
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {            
            if (StreamReader.EndOfStream) {  return false; }  
            #region 流程控制
            CurrentIndex++;
            if (CurrentIndex == StartIndex) { log.Debug("数据流 " + this.Name + " 开始读取数据。"); }
            if (this.IsCancel) { log.Info("数据流 " + this.Name + " 已被手动取消。"); return false; }
            if (CurrentIndex > this.MaxEnumIndex) { log.Info("数据流 " + this.Name + " 已达指定的最大编号 " + this.MaxEnumIndex); return false; }
            while (CurrentIndex < this.StartIndex) { this.MoveNext(); }
            #endregion

            string line = null;
            //try
            {
                line = StreamReader.ReadLine();
                if (!IsEpochSectionFirstLine(line))
                {
                    log.Warn( Name + "星历略过 " + line );
                    return MoveNext();
                }
                var val = ReadSection(Header, StreamReader, line, FilterdPrn, IsAvailableOnly);
                if (val != null)
                {
                    this.Current = val;
                    return true;
                }
            }
            //catch (Exception ex)
            //{
            //    log.Error("读取星历文件" + Name + "出错：" + ex.Message + line + ", 将继续尝试");
            //    return MoveNext();
            //}
            return false;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            StreamReader.BaseStream.Position = 0;
            StreamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            RinexUtil.SkipLines(StreamReader, Header.LineNumber);
        }

        #region 解析内容方法
        /// <summary>
        /// Sp3 LINE 23 (epoch header clk)
        /// </summary>
        public static string RecordHeadSymbol = "* ";  
        /// <summary>
        /// Sp3 LINE 24 (position and clock clk)
        /// </summary>
        public static string PositionClockSymbol = "P"; 
        /// <summary>
        /// Sp3 LINE 25 (velocity and clock clk)
        /// </summary>
        public static string VelocityClockSymbol = "V";  
        /// <summary>
        /// double add sp3为#cV格式时的单颗星星历内容第二行首两位字符
        /// </summary>
        public static string EphemerisPosition = "EP";
        /// <summary>
        /// double add sp3为#cV格式时的单颗星星历内容第四行首两位字符
        /// </summary>
        public static string EphemerisVelocity = "EV";


        /// <summary>
        /// 钟差最大值
        /// </summary>
        public static double MaxClockError = 99999999.9;
        /// <summary>
        /// 读取 SP3 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="r"></param>
        /// <param name="firstLine"></param>
        /// <param name="satNum">是否读取指定卫星的值</param>
        /// <param name="availableOnly">是否只返回可用的记录</param>
        /// <returns></returns>
        public static Sp3Section ReadSection(Sp3Header header, StreamReader r, string firstLine, SatelliteNumber satNum, bool availableOnly = true)
        {
            Sp3Section sec = new Sp3Section();
            sec.Time = ParseTime(firstLine);
            

            //if (header.OrbitType == SatelliteType.C)//如果是历元，需要转换为北斗时间来计算。
            {
                //  time = time + 14;
            }
            int satLineCount = 1;
            for (int i = 0; i < header.NumberOfSatellites * satLineCount; i++)
            {
                firstLine = r.ReadLine();
                if (firstLine == "EOF") break;
                var firstChar = firstLine.Substring(0, 1);
                var firstTwoChar = firstLine.Substring(0, 2);
                //判断是否内容
                if (firstTwoChar == RecordHeadSymbol)
                {
                    return ReadSection(header, r, firstLine, satNum, availableOnly);
                }

                //解析卫星编号
                var prn = SatelliteNumber.Parse(firstLine.Substring(1, 3));
                if (satNum != SatelliteNumber.Default && !satNum.Equals(prn)) { continue; }  //过滤指定卫星否

                if (!sec.Contains(prn)) { sec[prn] = new Ephemeris(prn, sec.Time) { Source = header.SourceName }; }
                var rec = sec[prn];

                if (firstChar == PositionClockSymbol || firstTwoChar == EphemerisPosition)
                { 
                    double clock;
                    var xyz = ParseLine(firstLine, out clock);
                    //单位转换为米
                     rec.XYZ = xyz * 1000.0;
                    if (clock == 999999.999999) //continue;
                    {                       
                        clock =  MaxClockError;
                        if (availableOnly) { sec.Remove(prn); continue; }
                    } //????????????是否该标记卫星状态糟糕   //崔阳，修改，应直接去除该卫星，不能使用。
                    rec.ClockBias = clock * 1e-6;

                    if (firstLine.Length > 70)
                    {
                        ParsePosStd(firstLine, rec);
                    }
                }
                else if (firstChar == VelocityClockSymbol || firstTwoChar == EphemerisVelocity)
                {
                    satLineCount = 2;
                    double dclock;
                    var xyz = ParseLine(firstLine, out dclock);
                    rec.XyzDot = xyz * 0.1;
                    rec.ClockDrift = dclock * 1e-10;
                }

            }
            return sec;
        }

        private static void ParsePosStd(string firstLine, Ephemeris rec)
        {
            //读取精度估值stdv
            //Columns 62-63       x-sdev (b**n mm)   18                  I2
            //Column  64          Unused             _                   blank
            //Columns 65-66       y-sdev (b**n mm)   18                  I2
            //Column  67          Unused             _                   blank
            //Columns 68-69       z-sdev (b**n mm)   18                  I2
            //Column  70          Unused             _                   blank
            //Columns 71-73       c-sdev (b**n psec) 219                 I3

            double sdevX = 0.0, sdevY = 0.0, sdevZ = 0.0;
            string tmp = firstLine.Substring(61, 2);
            if (tmp != "  ")
            { sdevX = double.Parse(tmp) * 0.001; } //保留为米
            tmp = firstLine.Substring(64, 2);
            if (tmp != "  ")
            { sdevY = double.Parse(tmp) * 0.001; }
            tmp = firstLine.Substring(67, 2);
            if (tmp != "  ")
            { sdevZ = double.Parse(tmp) * 0.001; }

            rec.Rms = new XYZ(sdevX, sdevY, sdevZ);

            double sdevClock = 0.0;
            tmp = firstLine.Substring(70, 3); //保留了皮秒单位，因为10-12次方太小
            if (tmp != "   ")
            { sdevClock = double.Parse(tmp); }

            rec.ClockBiasRms = sdevClock;
        }
        /// <summary>
        /// 解析一行。原始读取。
        /// </summary>
        /// <param name="firstLine"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="clock"></param>
        private static XYZ ParseLine(string firstLine, out double clock)
        {
            double x, y, z;
            x = double.Parse(firstLine.Substring(4, 14));
            y = double.Parse(firstLine.Substring(18, 14));
            z = double.Parse(firstLine.Substring(32, 14));
            clock = double.Parse(firstLine.Substring(46, 14));
            return new XYZ(x, y, z);
        }
        /// <summary>
        /// 解析时间
        /// </summary>
        /// <param name="firstLine"></param>
        /// <returns></returns>
        private static Time ParseTime(string firstLine)
        {
            var time = Time.Parse(firstLine.Substring(3, firstLine.Length - 3 > 28 ? 28 : firstLine.Length - 3));
            return time;
        }
        /// <summary>
        /// 判断是否是起始行
        /// </summary>
        /// <param name="firstLine"></param>
        /// <returns></returns>
        private static bool IsEpochSectionFirstLine(string firstLine)
        {
            var firstTwoChar = firstLine.Substring(0, 2);
            if (firstTwoChar == RecordHeadSymbol) { return true; }
            return false;

            if (firstLine.Trim() == "EOF")
            {
                return false;
            }
            if (firstTwoChar != RecordHeadSymbol) { return false; } // throw new Exception("SP3文件不合法,头部首2字母为 " + firstTwoChar + ", 非 " + RecordHeadSymbol);

            return true;
        }
        #endregion
       public void Dispose()
        {
            if (this.StreamReader != null)
            {
                this.StreamReader.Dispose();

            } 
        }
    }



    //Sp3 LINE 23 (epoch header clk)
    //colA  1- 2    symbols                  *_
    // colA  4- 7    year end               1993
    // colA  9-10    month end              _1
    // colA 12-13    secondOfWeek of month end       29
    // colA 15-16    hour end               _0
    // colA 18-19    second end             _0
    // colA 21-31    second end             _0.00000000

    //Sp3 LINE 24 (position and clock clk)
    // colA  1       symbol                   P
    // colA  2- 4    satellite id             __1
    // colA  5-18    XArray-coordinate (km)        __14722.638510
    // colA 19-32    YArray-coordinate (km)        ___6464.319150
    // colA 33-46    z-coordinate (km)        _-21020.844810
    // colA 47-60    clock (microsec)         _____-8.059218

    //Sp3 LINE 25 (velocity and clock clk)
    // colA  1       symbol                   V
    // colA  2- 4    satellite id             __1
    // colA  5-18    XArray-dot (decim/sec)        __-1196.628800
    // colA 19-32    YArray-dot (decim/sec)        __26950.022500
    // colA 33-46    z-dot (decim/sec)        ___7502.277100
    // colA 47-60    cl rate (10e-4 msec/sec) ______0.000000




    //   SP3 Line Twenty four (The Position and Clock Record)
    //(See example 1)

    //Column  1           Symbol             P                   A1
    //Columns 2-4         Vehicle Id.        G01                 A1,I2.2
    //Columns 5-18        x-coordinate(km)   _-11044.805800      F14.6 
    //Columns 19-32       y-coordinate(km)   _-10475.672350      F14.6
    //Columns 33-46       z-coordinate(km)   __21929.418200      F14.6
    //Columns 47-60       clock (microsec)   ____189.163300      F14.6
    //Column  61          Unused             _                   blank
    //Columns 62-63       x-sdev (b**n mm)   18                  I2
    //Column  64          Unused             _                   blank
    //Columns 65-66       y-sdev (b**n mm)   18                  I2
    //Column  67          Unused             _                   blank
    //Columns 68-69       z-sdev (b**n mm)   18                  I2
    //Column  70          Unused             _                   blank
    //Columns 71-73       c-sdev (b**n psec) 219                 I3
    //Column  74          Unused             _                   blank
    //Column  75          Clock Event Flag   E                   A1
    //Column  76          Clock Pred. Flag   P                   A1
    //Columns 77-78       Unused             __                  2 blanks
    //Column  79          Maneuver Flag      M                   A1
    //Column  80          Orbit Pred. Flag   P                   A1

    /// <summary>
    /// 时间序列数据流读取器
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    public interface ITimedStreamReader<TProduct> : IEnumer<TProduct>
    {
        /// <summary>
        /// 尝试获取起始时间，失败则返回最大值
        /// </summary>
        /// <returns></returns>
        Time TryGetStartTime();
        /// <summary>
        /// 尝试获取结束时间，失败则返回最小值
        /// </summary>
        /// <returns></returns>
         Time TryGetEndTime();
    }


    /// <summary>
    ///时间序列数据流读取
    /// </summary>
    public abstract class AbstractTimedStreamReader<TProduct> :  ITimedStreamReader<TProduct>
    {
        protected ILog log = Log.GetLog(typeof(AbstractTimedStreamReader<TProduct>));
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        public AbstractTimedStreamReader(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("输入不存在：" + filePath);
            this.InputPath = filePath;
            EnumCount = int.MaxValue / 2;
            this.CurrentIndex = -1;
            StreamReader = new StreamReader(filePath, Encoding.Default);
            this.Name = Path.GetFileName(filePath);
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否取消
        /// </summary>
        public bool IsCancel { get; set; }
        /// <summary>
        /// 当前编号，从 0 开始。
        /// </summary>
        public int CurrentIndex { get; set; }
        /// <summary>
        /// 起始编号，从0开始。
        /// </summary>
        public int StartIndex { get; set; }
        /// <summary>
        /// 遍历数量，默认为最大值的一半。
        /// </summary>
        public int EnumCount { get; set; }
        /// <summary>
        /// 最大的循环编号
        /// </summary>
        public int MaxEnumIndex { get { return StartIndex + EnumCount; } }
        /// <summary>
        /// 设置遍历数量
        /// </summary>
        /// <param name="StartIndex"></param>
        /// <param name="EnumCount"></param>
        public void SetEnumIndex(int StartIndex, int EnumCount) { this.StartIndex = StartIndex; this.EnumCount = EnumCount; }
        /// <summary>
        /// 路径
        /// </summary>
        protected string InputPath { get; set; }
        /// <summary>
        /// 数据流读取器
        /// </summary>
        protected StreamReader StreamReader { get; set; }

        #region GetEnumerator
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TProduct> GetEnumerator()
        {
            return this;
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        /// <summary>
        /// 从文件开始获取时间
        /// </summary>
        /// <returns></returns>
        public abstract Time TryGetStartTime();
        /// <summary>
        /// 从文件末尾获取时间
        /// </summary>
        /// <returns></returns>
        public abstract Time TryGetEndTime();
        /// <summary>
        /// 当前钟差
        /// </summary>
        public TProduct Current { get; protected set; }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (StreamReader != null)
            {
                StreamReader.Dispose();
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }
        /// <summary>
        /// 移动到下一个，错误则返回false
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();
        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            this.CurrentIndex = -1;
            StreamReader.BaseStream.Position = 0;
        }
    }
}
