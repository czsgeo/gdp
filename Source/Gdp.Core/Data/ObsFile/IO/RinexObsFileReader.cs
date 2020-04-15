//2014.11.29, cy, edit in zhengzhou, IGS 2013.11.1 orid Rinex 2 等 被 teqc 处理过的出现了 “ 4 9 ”等字符,且出现在头部
//2015.04.16, czs, edit in namu, 字符串不足指定长度时，就不再截取，lly读取IGS abmf1260.14o 数据时发现该问题。
//2015.04.26, czs, edit in namu, 改进 RINEX 3.0 内容读取，每一个记录可以从任何位置开始，通过“ >”判断。问题发现于黄令勇给的数据 801002212K.14O
//2015.05.25, lly, edit in zz, 2.0读取卫星编号，改后的适用于卫星小于36的情况
//2015.05.25, czs, edit in namu, 提取合并 ParseObsValueV2V3，改进Rinex V2 卫星列表读取算法，支持任意卫星数量的读取。
//2016.02.03, czs, edit in hongqing, 历元头部增强容错功能，时间解析成功后再继续解析
//2016.02.04, czs, edit in hongqing, 增加适应能力，可以读取更多变化的观测文件
//2016.11.28, czs & cy  & double, edit in hongqing, 改进出错自适应，修正递归溢出错误
//2018.08.15, czs, edit in hmx, StateFlag 扩展为2位，支持Gdp自定义
//2018.09.26, czs, edit in hmx, 增加RINEX3.03的支持，GLONASS 频率偏移等
//2018.11.28, czs, edit in hmx, 测站名称指定字符串


using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Gdp.Utils;
using Gdp.IO;

namespace Gdp.Data.Rinex
{

    /// <summary>
    /// Rinex 观测文件读取器
    /// </summary>
    public class RinexObsFileReader : IEnumer<RinexEpochObservation>, IDisposable
    {
        ILog log = Log.GetLog(typeof(RinexObsFileReader));
        /// <summary>
        /// 初始化一个读取器。
        /// </summary>
        /// <param name="MaxLenOfMarkerName">最长的字符串</param>
        /// <param name="fileName">文件路径</param>
        /// <param name="isReplaceSpaceAnddMinusMaker">是否替换“-”和“ ”用“_”</param>
        /// <param name="IsReadContent">是否读取内容，有的只是概略统计，则不需要读取内容</param>
        public RinexObsFileReader(string fileName, bool IsReadContent = true, int MaxLenOfMarkerName = 8, bool isReplaceSpaceAnddMinusMaker = true)
        {
            this.IsReplaceSpaceAnddMinusMaker = isReplaceSpaceAnddMinusMaker;

            CurrentIndex = -1;
            EnumCount = int.MaxValue / 2;
            this.MaxLenOfMarkerName = MaxLenOfMarkerName;
            this.Name = fileName;
            this.IsReadContent = IsReadContent;
            if (FileUtil.IsValid(fileName))
            {
                this.FilePath = fileName;
                _header = GetHeader();//头文件包含很多有用信息。 
            }
            else
            {
                EnumCount = -1;
                log.Error("文件不存在或大小为 0 ！" + fileName);
            }
        }
        /// <summary>
        /// 是否替换“-”和“ ”用“_”
        /// </summary>
        public bool IsReplaceSpaceAnddMinusMaker { get; set; }
        /// <summary>
        /// 读取区间
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Time ReadFirstEpoch(string path)
        {
            //读取内容
            RinexObsFile file = Read(path, false, 1);
            if (file.Count > 0)
            {
                return file[0].ReceiverTime;
            }
            return Time.MinValue;
        }

        /// <summary>
        /// 读取区间
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TimePeriod ReadPeriod(string path)
        {
            var header = ReadHeader(path);
            if (header.TimePeriod == null || header.TimePeriod.Span == 0)
            {
                //读取内容
                RinexObsFile file = Read(path, false);

                return file.Header.TimePeriod;

            }
            return header.TimePeriod;
        }

        #region 属性、字段
        /// <summary>
        /// 测站名称最长字符串
        /// </summary>
        public int MaxLenOfMarkerName { get; set; }
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
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否读取数据内容。否则只读取卫星编号。默认为True。
        /// </summary>
        public bool IsReadContent { get; set; }
        /// <summary>
        /// 观测头文件
        /// </summary>
        private RinexObsFileHeader _header { get; set; }
        /// <summary>
        /// 文件版本
        /// </summary>
        private double Version { get { return _header.Version; } }
        /// <summary>
        /// 当前文件路径。
        /// </summary>
        private string FilePath { get; set; }
        /// <summary>
        /// 数据流阅读器
        /// </summary>
        private StreamReader StreamReader { get; set; }
        /// <summary>
        /// 当前行
        /// </summary>
        private string CurrentLine { get; set; }
        /// <summary>
        /// 当前的观测信息
        /// </summary>
        public RinexEpochObservation Current { get; set; }
        #endregion

        #region 方法

        #region 常用接口方法
        #region 读取观测文件，流式读取

        private Time startTime = Time.Default;
        /// <summary>
        /// 起始时间
        /// </summary>
        public Time StartTime
        {
            get
            {
                if (startTime != Time.Default) return startTime;
                if (_header.StartTime != Time.Default)
                {
                    startTime = _header.StartTime;
                    return startTime;
                }
                //从文件中获取
                string line = GetFirstContentLine(FilePath);
                startTime = Time.Parse(line.Substring(0, 26));
                return startTime;
            }
        }


        #endregion

        #region 读取观测文件，块式读取
        /// <summary>
        /// 读取观测文件，将所有数据读入内存。目前支持 Rinex 4.0 以下的格式。
        /// </summary> 
        /// <returns></returns>
        public RinexObsFile ReadObsFile(int count = int.MaxValue)
        {
            RinexObsFile f = new RinexObsFile();
            f.Header = _header;
            string line = null;
            //try
            //{
            if (this._header == null && FilePath != null)
            {
                f.Header = GetHeader();
            }

            using (StreamReader r = new StreamReader(FilePath, true))
            {
                RinexUtil.SkipHeader(r); //StreamReader 略去头部
                int i = 0;
                if (Version < 3)
                {
                    this.CurrentObsCodesV2 = f.Header.GetObsCodesV2();

                    while ((line = ReadContentLine(r, f.Header.Comments)) != null)//读取观测数据的第一句。
                    {
                        RinexEpochObservation rec = ReadEpochObservationV2(f.Header, r, line, IsReadContent, true);
                        if (rec != null)
                        {
                            f.Add(rec);
                            i++;
                            if (i >= count) { break; }
                        }
                    }
                }
                else if (Version >= 3.0 && Version < 4.0)
                {
                    string nextHeaderLine = null;
                    while (nextHeaderLine != null || (line = ReadContentLine(r, f.Header.Comments)) != null)//读取观测数据的第一句。
                    {
                        if (nextHeaderLine != null)
                            line = nextHeaderLine;

                        RinexEpochObservation rec = ReadEpochObservationV3(f.Header, r, line, out nextHeaderLine, IsReadContent, true);
                        if (rec != null)
                        {
                            f.Add(rec);
                            i++;
                            if (i >= count) { break; }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("还不支持 RINEX 4.0 以上产品。请联系管理员。");
                }
            }
            f.UpdateAndGetHeaderTimePeriodWithContentTime();
            //}
            //catch (Exception ex)
            //{
            //    var msg = "读取 " + FilePath + " 的行 “" + line + "”（或该行下某行）时出错！\r\n该文件格式可能有错误，或版本未被支持。\r\n具体：" + ex.Message;
            //    log.Error(msg);
            //    throw new ArgumentException(msg);
            //}
            return f;
        }

        #endregion

        /// <summary>
        /// 获取或读取RINEX文件的头文件。在直接读取头文件的基础上对头文件内容进行部分修正。
        /// </summary>
        /// <returns></returns>
        public RinexObsFileHeader GetHeader(bool isNeedEndTime = true)
        {
            if (_header != null) { return _header; }
            else
            {
                lock (locker)
                {
                    _header = ReadObsFileHeader(FilePath, MaxLenOfMarkerName);
                    if (_header == null) { return null; }

                    //采样间隔需要初始化,
                    if (_header.Interval == 0 || _header.Interval > 30)//增加30s以上的判断，防止随意设置
                    {
                        this.Reset();

                        this.MoveNext();
                        var pre = this.Current;
                        this.MoveNext();
                        var now = this.Current;
                        if (now != null && pre != null)
                        {
                            _header.Interval = now.ReceiverTime - pre.ReceiverTime;
                        }
                        else
                        {
                            _header.Interval = 30;//默认
                        }
                        this.Reset();
                    }

                    if (_header.SatelliteTypes.Count == 0)
                    {
                        throw new Exception("头文件没有卫星系统或观测码，请仔细检查头文件，再试！");
                    }
                    if (_header.LeapSeconds == 0)
                    {
                        _header.LeapSeconds = LeapSecond.Instance.GetLeapSecondFromGpsT(_header.StartTime.DateTime);
                    }

                    //对于RINEX2.x，需要更新各个系统的变量，取代M
                    if (_header.SatelliteTypes.Count <= 1 && _header.SatelliteTypes.Contains(SatelliteType.M))
                    {
                        //M表示复合类型,应该继续往下毒
                        this.Reset();

                        this.MoveNext();
                        //让内容的PRN来更新卫星类型。
                        int index = 0;
                        List<SatelliteType> satTypes = new List<SatelliteType>();
                        while (this.MoveNext())
                        {
                            if (Current == null || Current.Count == 0) { continue; }
                            foreach (var prn in Current.Prns)
                            {
                                if (!satTypes.Contains(prn.SatelliteType)) { satTypes.Add(prn.SatelliteType); }
                            }

                            if (index > 20) { break; }

                            index++;
                        }
                        var first = _header.ObsInfo.ObsCodes.FirstOrDefault();
                        foreach (var item in satTypes)
                        {
                            if (first.Key == item) { continue; }
                            _header.ObsInfo.ObsCodes.Add(item, first.Value);
                        }
                        _header.ObsInfo.ObsCodes.Remove(SatelliteType.M);
                        this.Reset();
                    }
                }
                //读取结束
                if (isNeedEndTime && !_header.IsEndTimeRead)
                {
                    log.Debug("遍历读取结束 " + this._header.FileName);
                    //M表示复合类型,应该继续往下毒
                    this.Reset();
                    //让内容的PRN来更新卫星类型。 
                    var isReadContend = this.IsReadContent;
                    this.IsReadContent = false;
                    Time endTime = Time.MinValue;
                    while (this.MoveNext())
                    {
                        if (Current == null) continue;

                        if (this.Current.RawTime != null && this.Current.RawTime != default(Time))
                        {
                            endTime = this.Current.RawTime;
                        }
                    }
                    this._header.EndTime = endTime;
                    log.Debug(_header.FileName + " 结束时间设置为 " + endTime);
                    this.IsReadContent = isReadContend;
                    this.Reset();
                    _header.IsEndTimeRead = true;

                }

            }
            return _header;
        }

        static object locker = new object();

        #endregion

        #region 检索
        /// <summary>
        /// 检索，查找，若没有找到，则返回null。
        /// </summary>
        /// <param name="gpsTime"></param>
        /// <param name="toleranceSeccond"></param>
        /// <returns></returns>
        public RinexEpochObservation Get(Time gpsTime, double toleranceSeccond = 1e-15)
        {
            try
            {
                return LoopAndFind(gpsTime, 0, toleranceSeccond);
            }
            catch (Exception ex)
            {
                log.Error("数据读取错误 " + gpsTime + ex.Message);
                GC.Collect();

                return null;
            }

        }

        /// <summary>
        /// 循环迭代查找
        /// </summary>
        /// <param name="time"></param>
        /// <param name="totalLoopCount">大循环次数，最多1次</param>
        /// <param name="toleranceSeccond"></param>
        /// <returns></returns>
        private RinexEpochObservation LoopAndFind(Time time, int totalLoopCount, double toleranceSeccond)
        {
            //不在有效时段内。
            if (!_header.ObsInfo.TimePeriod.Contains(time))
            { return null; }

            //首先看第一个是否为空，如果是则读取一个
            bool hasNext = true;
            if (this.Current == null) { hasNext = this.MoveNext(); }

            var currentTime = this.Current.ReceiverTime;
            //比较是否满足要求
            var differ = time - currentTime;
            //满足要求，则直接返回
            if (Math.Abs(differ) < toleranceSeccond) { LastTime = this.Current.ReceiverTime; return this.Current; }

            //不满足要求，则需要分析
            //如果在当前时间之后
            if (differ > 0)
            {
                //避免深度递归导致的堆栈溢出
                if (_header.Interval != 0 && differ != _header.Interval)
                {
                    var epochCount = differ / _header.Interval - 1;
                    for (int i = 0; i < epochCount; i++)
                    {
                        this.MoveNext();
                    }
                }

                hasNext = (this.MoveNext());
                if (hasNext)
                {
                    var newTime = this.Current.ReceiverTime;
                    differ = time - newTime;
                    //满足要求，则直接返回
                    if (Math.Abs(differ) < toleranceSeccond) { LastTime = this.Current.ReceiverTime; return this.Current; }
                    //如果在两则个历元之间，则无法获得数据，返回null
                    if (differ < 0)
                    {
                        return null;
                    }
                    //如果还在后面，则需要继续遍历
                    return LoopAndFind(time, totalLoopCount, toleranceSeccond);
                }
                else //如果没有，则返回 null
                {
                    return null;
                }
            }
            else//在当前时间之前，则可能需要回溯
            {
                //如果已经回溯一次了，则直接返回null，不再流转
                if (LastTime < time) //最后一个成功的后面，说明这段时间没有数据。
                {
                    return null;
                }
                if (totalLoopCount > 0) { return null; }
                this.Reset();
                totalLoopCount++;
                return LoopAndFind(time, totalLoopCount, toleranceSeccond);
            }
        }
        /// <summary>
        /// 上一个时间
        /// </summary>
        public Time LastTime { get; set; }
        #endregion


        #region Enumerable

        public void Dispose()
        {
            if (this.StreamReader != null)
            {
                this.StreamReader.Close();
                this.StreamReader.Dispose();
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }
        /// <summary>
        /// 如果下一个记录的头部已经读取，则不用读取第一行了。采用本参数进行存储。
        /// </summary>
        private string _nextHeaderLine = null;

        /// <summary>
        /// 读取下一个观测历元数据。
        /// 如果没有了则返回false。
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            #region 流程控制
            CurrentIndex++;
            if (CurrentIndex == StartIndex) { log.Debug("数据流 " + this.Name + " 开始读取数据。"); }
            if (this.IsCancel) { log.Info("数据流 " + this.Name + " 已被手动取消。"); return false; }
            if (CurrentIndex > this.MaxEnumIndex) { log.Info("数据流 " + this.Name + " 已达指定的最大编号 " + this.MaxEnumIndex); return false; }
            while (CurrentIndex < this.StartIndex)
            {
                this.Current = null;
                this.MoveNext();
            }
            #endregion

            if (StreamReader == null)
            {
                StreamReader = new StreamReader(FilePath);
                RinexUtil.SkipHeader(StreamReader);
            }

            if (StreamReader.BaseStream == null || StreamReader.EndOfStream)
                return false;

            //获取下一行。
            if (_nextHeaderLine != null) { CurrentLine = _nextHeaderLine; }
            else
            {
                if ((CurrentLine = ReadContentLine(StreamReader)) == null)
                    return false;
            }

            if (Version < 3)
            {
                this.Current = null;
                this.Current = ReadEpochObservationV2(_header, StreamReader, CurrentLine, IsReadContent, true);
            }
            if (Version >= 3.0 && Version < 4.0)
            {
                this.Current = null;
                this.Current = ReadEpochObservationV3(_header, StreamReader, CurrentLine, out _nextHeaderLine, IsReadContent, true);
            }

            _nextHeaderLine = null;
            return true;
        }
        /// <summary>
        /// 重置数据流
        /// </summary>
        public void Reset()
        {
            CurrentIndex = -1;
            if (this.StreamReader != null)
            {
                this.StreamReader.Close();
                this.StreamReader.Dispose();
                this.Current = null;
                StreamReader = null;
            }
        }

        #region IEnumerator
        public IEnumerator<RinexEpochObservation> GetEnumerator()
        {
            return this;
            //return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
        #endregion

        #region 静态读取方法

        /// <summary>
        ///  读取RINEX文件的头文件。
        /// </summary>
        /// <param name="rinexFilePath">文件路径</param>
        /// <param name="MaxLenOfMarkerName">文件路径</param>
        /// <returns></returns>
        public RinexObsFileHeader ReadObsFileHeader(string rinexFilePath, int MaxLenOfMarkerName = 8)
        {
            if (!FileUtil.IsValid(rinexFilePath)) { return null; }

            RinexObsFileHeader header = new RinexObsFileHeader();
            CheckAndSetNavPath(rinexFilePath, header);

            header.FilePath = rinexFilePath;
            header.TrySetAndGetContryCodeWithFileName();
            using (StreamReader r = new StreamReader(rinexFilePath, true))
            {
                string line = null;
                while ((line = r.ReadLine()) != null)
                {
                    //空行为END Header V1.0
                    if (String.IsNullOrWhiteSpace(line) || line.Length < 60)
                    {
                        return header;
                    }
                    //中文字符支持
                    int nonAscCount = StringUtil.GetNonAscCount(line.Substring(0, 60 > line.Length ? line.Length : 60));
                    string headerLabel = line.Substring(60 - nonAscCount).Trim();//header label 61-80

                    if (headerLabel.Contains(RinexHeaderLabel.END_OF_HEADER)) { break; }

                    //空行为END Header V1.0
                    if (String.IsNullOrWhiteSpace(line))
                    {

                        return header;
                    }//没有内容

                    string content = line.Substring(0, 60);
                    if (String.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }//没有内容
                    ParseHeaderLine(header, r, line, headerLabel, nonAscCount);
                }
            }


            return header;
        }
        /// <summary>
        /// 检查设置导航文件。支持当前目录和上一目录的同名导航文件
        /// </summary>
        /// <param name="rinexFilePath"></param>
        /// <param name="header"></param>
        private static void CheckAndSetNavPath(string rinexFilePath, RinexObsFileHeader header)
        {
            var postFix = "N";
            var path = CheckAndSetNavPath(rinexFilePath, header, postFix);
            if (!File.Exists(path))//尝试其它P
            {
                CheckAndSetNavPath(rinexFilePath, header, "P");
            }
        }

        public static string CheckAndSetNavPath(string rinexFilePath, RinexObsFileHeader header, string postFix)
        {
            //如果有导航文件，则记录其位置
            //支持当前目录和上一目录的同名导航文件
            var navPath = rinexFilePath.TrimEnd('O', 'o') + postFix;
            if (FileUtil.IsValid(navPath)) { header.NavFilePath = navPath; }
            else
            {
                var upDir = Path.GetDirectoryName(Path.GetDirectoryName(navPath));
                var navName = Path.GetFileName(navPath);
                navPath = Path.Combine(upDir, navName);

                if (FileUtil.IsValid(navPath))
                {
                    header.NavFilePath = navPath;
                }
            }
            return navPath;
        }

        /// <summary>
        /// 解析一条
        /// </summary>
        /// <param name="header"></param>
        /// <param name="line"></param>
        /// <param name="reader"></param>
        /// <param name="headerLabel"></param>
        /// <param name="nonAscCount"></param>
        public int ParseHeaderLine(RinexObsFileHeader header, StreamReader reader, string line, string headerLabel, int nonAscCount = 0)
        {
            int readedLineCount = 0;
            if (String.IsNullOrWhiteSpace(line)) { return readedLineCount; }

            string content = line.Substring(0, 60);
            if (String.IsNullOrWhiteSpace(content)) { return readedLineCount; }//没有内容

            switch (headerLabel)
            {
                /**
                 *  +--------------------+------------------------------------------+------------+
                    | HEADER LABEL | DESCRIPTION | FORMAT |
                    | (Columns 61-80) | | |
                    +--------------------+------------------------------------------+------------+
                    |RINEX VERSION / TYPE| - Format version : 3.00 | F9.2,11X, |
                    | | - File type: O for Observation Data | A1,19X, |
                    | | - Satellite System: G: GPS | A1,19X |
                    | | R: GLONASS | |
                    | | E: Galileo | |
                    | | S: SBAS payload | |
                    | | M: Mixed | |
                    +--------------------+------------------------------------------+------------+
                 * 
                 */
                case RinexHeaderLabel.RINEX_VERSION_TYPE:
                    header.Version = double.Parse(line.Substring(0, 9));
                    header.FileType = (RinexFileType)Enum.Parse(typeof(RinexFileType), line.Substring(20, 1));
                    if (line.Substring(40, 1) == " ") header.SatTypeMarker = RinexUtil.GetSatelliteType(header.FileType);
                    else header.SatTypeMarker = (SatelliteType)Enum.Parse(typeof(SatelliteType), line.Substring(40, 1));
                    break;
                case RinexHeaderLabel.PGM_RUN_BY_DATE:
                    header.FileInfo.CreationProgram = line.Substring(0, 20).TrimEnd();
                    header.FileInfo.CreationAgence = line.Substring(20, 20).Trim();
                    header.FileInfo.CreationDate = line.Substring(40, 20).TrimEnd();
                    break;
                case RinexHeaderLabel.COMMENT:
                    if (header.Comments == null) header.Comments = new List<string>();
                    header.Comments.Add(line.Substring(0, 60 - nonAscCount).Trim());
                    break;
                case RinexHeaderLabel.MARKER_NAME:
                    int fromIndex = 0;
                    int len = 60;
                    string makerName = SubString(line, nonAscCount, fromIndex, len);
                    //if (nonAscCount != 0)//名称为英文，否则会出现乱码，或别的软件不支持。
                    //{ makerName = PinYinConverter.GetFirst(makerName); }
                    makerName = BuildSiteName(makerName, MaxLenOfMarkerName);
                    header.MarkerName = makerName;//保持大写
                    break;
                case RinexHeaderLabel.MARKER_NUMBER:
                    header.SiteInfo.MarkerNumber = line.Substring(0, 60).Trim();
                    break;
                case RinexHeaderLabel.OBSERVER_AGENCY:
                    header.ObsInfo.Observer = line.Substring(0, 20).Trim();
                    header.ObsInfo.ObserverAgence = line.Substring(20, 40).Trim();
                    break;
                case RinexHeaderLabel.REC_NUM_TYPE_VERS:
                    header.SiteInfo.ReceiverNumber = line.Substring(0, 20).Trim();
                    header.SiteInfo.ReceiverType = line.Substring(20, 20).Trim();
                    header.SiteInfo.ReceiverVersion = line.Substring(40, 20).Trim();
                    break;
                case RinexHeaderLabel.ANT_NUM_TYPE:
                    header.SiteInfo.AntennaNumber = line.Substring(0, 20).Trim();
                    header.SiteInfo.AntennaType = line.Substring(20, 20).Trim();
                    break;
                case RinexHeaderLabel.APPROX_POSITION_XYZ:
                    XYZ xyz = new XYZ();
                    try
                    {
                        xyz.X = double.Parse(line.Substring(0, 14));
                        xyz.Y = double.Parse(line.Substring(14, 14));
                        xyz.Z = double.Parse(line.Substring(28, 14));
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            log.Warn("近似坐标解析错误，可能并不标准，将尝试自动分割法继续解析：" + ex.Message);
                            xyz = XYZ.Parse(line);
                        }
                        catch (Exception ex2)
                        {
                            log.Error("近似坐标最终解析错误：" + ex2.Message);
                            //   if (Setting.GdpConfig.IsDebug)
                            {
                                throw ex;
                            }
                        }
                    }
                    header.ApproxXyz = xyz;
                    break;
                case RinexHeaderLabel.ANTENNA_DELTA_H_E_N:
                    //+--------------------+------------------------------------------+------------+
                    //|ANTENNA: DELTA H/E/N| - Antenna height: Height of bottom       |   3F14.4   |
                    //|                    |   surface of antenna above marker        |            |
                    //|                    | - Eccentricities of antenna center       |            |
                    //|                    |   relative to marker to the east         |            |
                    //|                    |   and north (all units in meters)        |            |
                    //+--------------------+------------------------------------------+------------+

                    HEN hen = new HEN();
                    try
                    {
                        hen.H = double.Parse(line.Substring(0, 14));
                        hen.E = double.Parse(line.Substring(14, 14));
                        hen.N = double.Parse(line.Substring(28, 14));
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            log.Warn("天线坐标解析错误，可能并不标准，将尝试自动分割法继续解析：" + ex.Message);
                            var xyzTemp = XYZ.Parse(line);

                            hen = new HEN(xyzTemp.X, xyzTemp.Z, xyzTemp.Y);
                        }
                        catch (Exception ex2)
                        {
                            log.Error("天线坐标最终解析错误：" + ex2.Message);
                            {
                                throw ex;
                            }
                        }
                    }
                    header.Hen = hen;
                    break;
                case RinexHeaderLabel.WAVELENGTH_FACT_L1_2:


                    break;

                case RinexHeaderLabel.MARKER_TYPE:
                    header.MarkerType = line.Substring(0, 20).Trim();
                    break;
                case RinexHeaderLabel.TYPES_OF_OBSERV://有三行的可能， Rinex 2 适用
                    string[] obTypes = ParseObsTypesV2(reader, line, ref readedLineCount);
                    //如果是多系统，该怎样处理？
                    header.SetTypeOfObservesV2(obTypes);
                    CurrentObsCodesV2 = obTypes.ToList();
                    //foreach (string type in obTypes)
                    //{
                    //    header.ObserTypes.Add((GnssDataType)Enum.Parse(typeof(GnssDataType), type));
                    //}
                    break;
                case RinexHeaderLabel.INTERVAL:
                    header.Interval = double.Parse(line.Substring(0, 10));
                    break;
                case RinexHeaderLabel.TIME_OF_FIRST_OBS:
                    var str = line.Substring(0, 60);
                    if (String.IsNullOrWhiteSpace(str)) break;
                    header.StartTime = Time.Parse(str);

                    TimeSystem timeSystem = TimeSystem.GPS;
                    var timesSysStr = str.Substring(45).Trim();
                    if (!String.IsNullOrWhiteSpace(timesSysStr))
                    {
                        timeSystem = (TimeSystem)Enum.Parse(typeof(TimeSystem), timesSysStr);
                    }
                    header.TimeSystem = timeSystem;
                    break;
                case RinexHeaderLabel.TIME_OF_LAST_OBS:
                    var str1 = line.Substring(0, 60);
                    if (String.IsNullOrWhiteSpace(str1)) break;
                    header.EndTime = Time.Parse(str1);
                    break;
                case RinexHeaderLabel.RCV_CLOCK_OFFS_APPL:

                    break;
                case RinexHeaderLabel.LEAP_SECONDS:
                    header.LeapSeconds = int.Parse(line.Substring(0, 6));
                    break;
                case RinexHeaderLabel.NUM_OF_SATELLITES:
                    break;
                case RinexHeaderLabel.PRN_NUM_OF_OBS:
                    break;

                case RinexHeaderLabel.PRN_LIST:
                    string[] prnStrs = line.Substring(0, 60).Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string prn in prnStrs) header.ObsInfo.TotalPrns.Add(SatelliteNumber.Parse(prn));
                    break;

                case RinexHeaderLabel.SYS_PHASE_SHIFT://3.03
                    {
                        if (String.IsNullOrWhiteSpace(line)) { return readedLineCount; }
                        readedLineCount += ParseSystemPhaseShift(header, reader, line);
                    }
                    break;


                case RinexHeaderLabel.GLONASS_SLOT_FRQ://3.03
                    {
                        var str0 = line.Substring(4, 60 - 4);
                        var strs = StringUtil.Split(str0, 7);
                        foreach (var s in strs)
                        {
                            if (String.IsNullOrWhiteSpace(s)) { continue; }
                            var subStrs = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var prn = SatelliteNumber.Parse(subStrs[0]);
                            var val = int.Parse(subStrs[1]);
                            header.GlonassSlotFreqNums[prn] = val;
                        }
                    }
                    break;

                case RinexHeaderLabel.GLONASS_COD_PHS_BIS://3.03
                    {
                        var str0 = line.Substring(1, 60 - 1);
                        var strs = StringUtil.Split(str0, 13);
                        foreach (var s in strs)
                        {
                            if (String.IsNullOrWhiteSpace(s)) { continue; }
                            var subStrs = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var code = ObservationCode.Parse(subStrs[0]);
                            var val = double.Parse(subStrs[1]);
                            header.GlonassCodePhaseBias[code] = val;
                        }
                    }
                    break;

                //rinex 3.0 nav
                case RinexHeaderLabel.TIME_SYSTEM_CORR:

                    break;
                //rinex 3.0 nav
                case RinexHeaderLabel.IONOSPHERIC_CORR:

                    break;
                //rinex 3.0 obs
                case RinexHeaderLabel.SYS_OBS_TYPES:
                    //一次读取一个卫星类型。
                    SatelliteType SatelliteType = (SatelliteType)Enum.Parse(typeof(SatelliteType), line.Substring(0, 1));
                    int count = int.Parse(line.Substring(3, 3));
                    string obsTypeStr = line.Substring(7, 51);
                    double lineCount = Math.Ceiling(count / 13.0d);

                    for (int i = 0; i < lineCount - 1; i++)
                    {
                        line = ReadContentLine(reader, header.Comments);// r.ReadLine();
                        readedLineCount++;

                        obsTypeStr += " " + line.Substring(7, 51);
                    }
                    string[] types = StringUtil.SplitByBlank(obsTypeStr);
                    //List<string> codes = new List<string>();
                    //foreach (var key in types)
                    //{
                    //    codes.Add((GnssDataType)Enum.Parse(typeof(GnssDataType), key, true));
                    //}
                    header.GetOrInitObsCodes(SatelliteType).AddRange(types);
                    break;
                //case RinexHeaderLabel.END_OF_HEADER:
                //    return header;
                default: break;
            }
            return readedLineCount;
        }
        /// <summary>
        /// 提取测站名称，只处理字符串将空格以0替换
        /// </summary>
        /// <param name="makerName"></param>
        /// <param name="MaxLenOfMarkerName"></param>
        /// <param name="replacerChar">替换“ ”和“-”的字符</param>
        /// <returns></returns>
        public static string BuildSiteName(string makerName, int MaxLenOfMarkerName = 8, string replacerChar = "0")
        {
            makerName = makerName.Trim().Replace(" ", replacerChar).Replace("-", replacerChar);
            makerName = StringUtil.SubString(makerName, 0, MaxLenOfMarkerName).ToUpper();
            return makerName;
        }

        private static string[] ParseObsTypesV2(StreamReader reader, string line, ref int readedLineCount)
        {
            var ObserTypesNumberV2 = int.Parse(line.Substring(3, 6));

            string obsLine = line.Substring(6, 60 - 6);
            int residualCount = (ObserTypesNumberV2 % 9) > 0 ? 1 : 0;//剩余的
            int obslineCount = ObserTypesNumberV2 / 9 + residualCount - 1;// 已经读取一行，所以减去一行
            for (int i = 0; i < obslineCount; i++)
            {
                obsLine += "  " + reader.ReadLine().Substring(6, 60 - 6);
                readedLineCount++;
            }
            // if (header.ObserTypesNumber > 9) obsLine += "  " + r.ReadLine().Substring(6, 60 - 6);

            string[] obTypes = obsLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return obTypes;
        }

        /// <summary>
        /// 解析系统频率偏移
        /// </summary>
        /// <param name="header"></param>
        /// <param name="reader"></param>
        /// <param name="line"></param>
        /// <param name="isStart">是否为起始行</param>
        /// <param name="toReadSatCount">未读卫星数量</param>
        /// <param name="code"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static int ParseSystemPhaseShift(RinexObsFileHeader header,
            StreamReader reader,
            string line,
            bool isStart = true,
            int toReadSatCount = 0, ObservationCode code = null, double val = 0)
        {
            int readedCount = 0;
            if (isStart)
            {
                var firstChar = line.Substring(0, 1);
                if (String.IsNullOrWhiteSpace(firstChar)) { return readedCount; }

                var satType = SatelliteTypeHelper.PareSatType(firstChar);
                var str = StringUtil.SubString(line, 2, 3);
                if (String.IsNullOrWhiteSpace(str)) { return readedCount; }
                if (str[0] == ' ' || str.Trim().Length == 2) { str = "L" + str.Trim(); }
                code = ObservationCode.Parse(str);

                str = StringUtil.SubString(line, 6, 8);
                if (String.IsNullOrWhiteSpace(str)) { return readedCount; }
                val = double.Parse(str);
                str = StringUtil.SubString(line, 15, 3);
                if (String.IsNullOrWhiteSpace(str)) { return readedCount; }

                toReadSatCount = int.Parse(str);
            }

            string[] prns = line.Substring(19, 60 - 19).Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var prnStr in prns)
            {
                SatelliteNumber prn = SatelliteNumber.Parse(prnStr);
                if (!header.SystemPhaseShift.ContainsKey(prn))
                {
                    header.SystemPhaseShift[prn] = new Dictionary<ObservationCode, double>();
                }
                header.SystemPhaseShift[prn][code] = val;
            }
            var remains = toReadSatCount - prns.Length;
            if (remains > 0)
            {
                line = ReadNextNoNullLine(reader); readedCount++;
                readedCount += ParseSystemPhaseShift(header, reader, line, false, remains, code, val);
            }
            return readedCount;
        }

        #region Rinex 2.0 数据内容
        /// <summary>
        /// 读取观测时段的头部（第一行或前两行）。 Rinex V1 - V2
        /// </summary>
        /// <param name="reader">数据流读取器</param>
        /// <param name="line">当前行</param>
        /// <param name="rec">历元观测信息</param>
        /// <returns></returns>
        private List<SatelliteNumber> ReadEpochHeaderV2(StreamReader reader, string line, ref RinexEpochObservation rec)
        {
            while (String.IsNullOrWhiteSpace(line))
            {
                if (reader.EndOfStream || line == null) { return new List<SatelliteNumber>(); }
                line = ReadContentLine(reader);
                //第一行必然有时间，否则持续读取
                Time time = TryParseTime(reader, ref line, rec);
            }

            //简单的判断是否是头部
            var epochFlagChar = line.Substring(28, 1)[0];
            while (!Char.IsNumber(epochFlagChar))
            {
                line = ReadContentLine(reader);
                if (line.Length > 28)
                {
                    epochFlagChar = line.Substring(28, 1)[0];
                    log.Debug("忽略读取 " + line);
                }
            }

            //遍历直到时间解析正确，如果非
            int EpochFlag = int.Parse(epochFlagChar.ToString());
            int SatCount = int.Parse(line.Substring(29, 3));
            int maxLoop = 500;
            int loopCount = 0;
            //  while ((EpochFlag == 4 || EpochFlag == 3) && String.IsNullOrWhiteSpace(line.Substring(0, 26))) //8
            //8 结尾是钟跳，6是周跳
            /// <summary>
            /// 历元标记，对应于 RINEX EpochFlag
            /// 历元标志：0表示正常，1表示在前一历元与当前历元之间发生了电源故障，>1为事件标志
            /// If EVENT FLAG record (epoch flag > 1):         
            /// - Event flag:                                
            /// 2: start moving antenna                     
            /// 3: new site occupation (end of kinem. satData) (at least MARKER NAME record follows)   
            /// 4: header information follows             
            /// 5: external event (epoch is significant,same time frame as observation time tags)
            /// 6: cycle slip records follow to optionally  report detected and repaired cycle slips (same format as OBSERVATIONS records;  
            ///slip instead of observation; LLI and signal strength blank)  
            /// </summary>
            if (EpochFlag == 4)//头部信息单独处理
            {
                for (int i = 0; i < SatCount; i++)
                {
                    line = reader.ReadLine();
                    if (line.Length <= 60) { continue; }

                    string headerLabel = line.Substring(60).TrimEnd();//header label 61-80
                    int readedLineCount = 0;
                    if (headerLabel == RinexHeaderLabel.TYPES_OF_OBSERV)
                    {
                        this.CurrentObsCodesV2 = ParseObsTypesV2(reader, line, ref readedLineCount).ToList();
                    }

                    //var readedLineCount = ParseHeaderLine(this._header, reader, line, headerLabel);
                    i = readedLineCount + i;
                    log.Debug("RINEX 中断， " + line);
                }
                //读完后再读一行为下一历元头部内容
                line = reader.ReadLine();
                EpochFlag = int.Parse(line.Substring(27, 2));//扩展为2位，支持Gdp自定义
                SatCount = int.Parse(line.Substring(30, 2));
            }
            else
            {
                //大于 1，则按行读取
                while (EpochFlag > 1 && EpochFlag % 10 != 8 && EpochFlag % 10 != 6)//&& String.IsNullOrWhiteSpace(line.Substring(0, 26))) //8
                {
                    log.Warn("历元标签出现 " + EpochFlag + ", " + FilePath);
                    //4后面是注释
                    for (int i = 0; i < SatCount; i++)
                    {
                        line = reader.ReadLine();
                        log.Debug("RINEX 中断， " + line);
                    }

                    line = ReadContentLine(reader, rec.Comments);

                    if (String.IsNullOrWhiteSpace(line)) { return new List<SatelliteNumber>(); }

                    EpochFlag = int.Parse(line.Substring(27, 2));//扩展为2位，支持Gdp自定义
                    SatCount = int.Parse(line.Substring(30, 2));
                    loopCount++;
                    if (loopCount > maxLoop)
                    {
                        log.Fatal("读取 " + rec.Name + " 观测文件时，循环尝试了 " + loopCount + " 次，强制退出！");
                        break;
                    }
                }
            }

            Time GpsTime = TryParseTime(reader, ref line, rec);
            List<SatelliteNumber> prns = null;

            double receiverClockOffset = StringUtil.ParseDouble(line, 68, 12);

            rec.RawTime = GpsTime;
            rec.EpochFlag = EpochFlag;
            //rec.SatCount = int.Parse(line.Substring(30, 2));
            rec.ReceiverClockOffset = receiverClockOffset;

            //读取卫星编号，超过12个则换行读取。
            string satLine = line.Substring(32, Math.Min(line.Length - 32, 12 * 3));//第一次读取
            int leftSatCount = SatCount - 12;//余下卫星数量

            while (leftSatCount > 0)//循环读取余下
            {
                if (leftSatCount > 12)
                {
                    satLine += ReadContentLine(reader, rec.Comments).Substring(32, 12 * 3);
                }
                else
                {
                    satLine += ReadContentLine(reader, rec.Comments).Substring(32, leftSatCount * 3);
                }
                //update
                leftSatCount = leftSatCount - 12;
            }
            if (SatCount == 0)
            {
                return new List<SatelliteNumber>();
            }
            if (!String.IsNullOrWhiteSpace(satLine)) { prns = SatelliteNumber.ParsePRNs(satLine, SatCount); }
            else { prns = new List<SatelliteNumber>(); }
            return prns;
        }

        /// <summary>
        /// 当前观测码,V2是相同的，只对V2有用。
        /// </summary>
        List<string> CurrentObsCodesV2 { get; set; }
        private List<ObservationCode> currentObsCodes { get; set; }
        public List<ObservationCode> CurrentObsCodes
        {
            get
            {
                if (currentObsCodes == null)
                    currentObsCodes = ObservationCode.GetObsCodes(CurrentObsCodesV2);
                return currentObsCodes;
            }
        }

        /// <summary>
        /// RINEX 2s。 读取一组观测记录。一个测站，同一时刻观测了多个卫星。
        /// 如果出错则返回null。
        /// </summary>
        /// <param name="header">头文件</param>
        /// <param name="reader">数据流读取</param>
        /// <param name="line">内容的第一行</param>
        /// <param name="IsReadContent">是否读取内容</param>
        /// <param name="skipError">是否忽略错误</param>
        /// <returns>如果出错则返回null。</returns>
        public RinexEpochObservation ReadEpochObservationV2(RinexObsFileHeader header, StreamReader reader, string line, bool IsReadContent = true, bool skipError = true)
        {
            RinexEpochObservation rec = null;
            List<string> conments = header.Comments;

            rec = new RinexEpochObservation() { Header = header };
            List<SatelliteNumber> prns = ReadEpochHeaderV2(reader, line, ref rec);
            if (prns.Count == 0)
            {
                return null;
            }

            //是否读取内容
            if (!IsReadContent)//略过内容行
            {
                int lineCountPerSat = (int)Math.Ceiling(CurrentObsCodesV2.Count / 5.0);
                //读取卫星观测值          
                foreach (SatelliteNumber satNum in prns)//挨个卫星读取
                {
                    for (int i = 0; i < lineCountPerSat; i++)
                    {
                        line = ReadContentLine(reader, conments);
                    }
                    rec.Set(satNum, null);
                }
            }
            else
            {
                //读取卫星观测值          
                foreach (SatelliteNumber satNum in prns)//挨个卫星读取
                {
                    RinexSatObsData s = ReadObservationDataV2(reader, satNum, conments);
                    s.ReciverTime = rec.ReceiverTime;
                    if (s != null) rec.Set(satNum, s);
                }
            }
            return rec;
        }

        /// <summary>
        /// 读取测站观测一个卫星的观测值记录。Rinex 3.0 以下（不含）的读取方法
        /// 如果有问题，直接抛出异常。
        /// </summary>
        /// <param name="obsCodes"></param>
        /// <param name="reader"></param>
        /// <param name="satNum"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        public RinexSatObsData ReadObservationDataV2(StreamReader reader, SatelliteNumber satNum, List<String> comments = null)
        {
            string line = null;
            RinexSatObsData s = new RinexSatObsData();
            s.RinexVersion = 2.1;// header.Version;
                                 //try
                                 //{
            s.Prn = satNum;
            //读取原始记录观测值为一行。 
            line = ReadContentLine(reader, comments);

            line = StringUtil.FillSpace(line, 80);


            var obsTypeCount = this.CurrentObsCodesV2.Count;// header.GetObsTypesCount(satNum.SatelliteType);
            int restLineLen = (obsTypeCount - 5) * 16;
            int nextCharCount = restLineLen > 80 ? restLineLen : 80;
            for (int i = 1; i * 5 < obsTypeCount; i++)
            {
                line += StringUtil.FillSpace(ReadContentLine(reader, comments), nextCharCount);
                restLineLen = (obsTypeCount - 5 * (i + 1)) * 16;
                nextCharCount = restLineLen > 80 ? restLineLen : 80;
            }

            List<string> obsStrList = StringUtil.Split(line, 16, obsTypeCount);
            int index = 0;
            foreach (string obsStr in obsStrList)
            {
                var code = CurrentObsCodesV2[index];
                var codeObj = CurrentObsCodes[index];
                RinexObsValue obsVal = ParseObsValueV2V3(obsStr, codeObj);

                s.Add(code, obsVal);
                index++;
            }
            //}
            //catch (Exception ex)
            //{
            //    throw new ArgumentException("解析行“" + line + "”时出错！\r\n" + ex.Message);
            //}
            return s;
        }

        /// <summary>
        /// 读取一行内容，并且略过头部标记。判断是否包含头部标签，如是则略过
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="line"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        private static string ReadContentLineAndSkipHeaderLabel(StreamReader reader, string line, RinexEpochObservation rec)
        {
            if (line == null) { return line; }

            while (RinexHeaderLabel.ContainLabel(line) || line == "")
            {
                line = ReadContentLine(reader, rec.Comments);
            }
            return line;
        }
        /// <summary>
        /// 尝试解析时间，有时候头部会有其它数据，容易出错，循环继续往下解析，直到成功。
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="line"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        private Time TryParseTime(StreamReader reader, ref string line, RinexEpochObservation rec)
        {
            Time GpsTime = Time.MinValue;
            try
            {
                GpsTime = Time.Parse(line.Substring(0, 26));
            }
            catch (Exception ex)
            {
                log.Error("解析时间出错,将继续尝试：" + ex.Message + line);
                line = ReadContentLine(reader, rec.Comments);
                //while (line == "")
                //{
                //    line = ReadContentLineAndSkipHeaderLabel(reader, line, rec);
                //    if (line == null) { log.Error("时间解析字符串为 NULL，返回默认值 " + Time.Default); return Time.Default; }
                //}
                if (line == null) { log.Error("时间解析字符串为 NULL，返回默认值 " + Time.Default); return Time.Default; }

                return TryParseTime(reader, ref line, rec);
            }
            return GpsTime;
        }
        #endregion

        #region Rinex 3.0 数据内容
        /// <summary>
        /// 读取RINEX 3.0版本的观测段。 
        /// </summary>
        /// <param name="header">RINEX header</param>
        /// <param name="reader">数据流</param>
        /// <param name="line">行</param>
        /// <param name="nextHeaderLine">下一记录的起始行，有的突然中断，并且改行已经读取，为头一行，则返回这一行</param>
        /// <param name="skipError">是否忽略错误。忽略则返回null，否则抛出异常</param>
        /// <returns></returns>
        public RinexEpochObservation ReadEpochObservationV3(RinexObsFileHeader header, StreamReader reader, string line, out string nextHeaderLine, bool isParseContent = true, bool skipError = true)
        {
            nextHeaderLine = null;

            if (line == null) { return null; }


            RinexEpochObservation rec = new RinexEpochObservation() { Header = header, Name = header.FileName };
#if !DEBUG
            try//解析观测时段头有可能错误。有时是正常的 COMMENT
            {
#endif
            //判断是否是观测段的开始
            var indexOfStart = line.IndexOf('>');
            while (indexOfStart == -1)
            {
                log.Warn(header.FileName + "，略过非观测段的开始，包含标识符：“>”，内容是：" + line);
                line = ReadContentLine(reader);
                indexOfStart = line.IndexOf('>');
            }


            //不排除有这种数据 //R20R09R19> 2014  7 31  5 26 50.0000000  0 29
            var headerLine = line.Substring(indexOfStart);
            int SatCount = ParseEpochHeaderLineV3(headerLine, reader, ref rec);

            //内容行，一行一颗卫星
            for (int i = 0; i < SatCount; i++)
            {
                line = ReadContentLine(reader, rec.Comments);
                //2019.05.20，czs， in hongqing， 大批量计算时，出现了null，导致了中断。
                if (line == null) { return rec; }

                //在过程中，可能突然中止，如下：
                //R20R09R19> 2014  7 31  5 26 50.0000000  0 29
                //此时应该立即转向记录头部，这几颗卫星记录为空。
                if (line.Contains(">"))
                {
                    nextHeaderLine = line;
                    return rec;
                }

                RinexSatObsData record = ParseObservationLineV3(header, line, isParseContent);
                record.ReciverTime = rec.ReceiverTime;
                rec[record.Prn] = record;
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                var info = header.FileName + "，正在读取行“" + line + "” " + ex.Message;
                Geo.IO.Log.GetLog(typeof(RinexObsFileReader)).Error(info);
                if (skipError) return rec;
                throw new ApplicationException(info);
            }
#endif
            return rec;
        }

        /// <summary>
        /// 解析 Rinex 3.0 历元观测数据的第一行,包含历元、卫星数量接收机钟差（通常为0，不可用）等参数。
        /// </summary>
        /// <param name="line">带解析行</param>
        /// <param name="rec">数据对象</param>
        /// <returns></returns>
        private int ParseEpochHeaderLineV3(string line, StreamReader reader, ref RinexEpochObservation rec)
        {//扩展为2位，支持Gdp自定义
            var EpochFlag = int.Parse(line.Substring(30, 2));//标记，如果为0，则正常，为1表示power failure between previous and current epoch，大于1表示其它状况。
                                                             //if (epochFlag != 0 || epochFlag !=8)
                                                             //{
                                                             //    line = ReadContentLine(reader);
                                                             //}
            int SatCount = int.Parse(line.Substring(32, 3));

            int maxLoop = 500;
            int loopCount = 0;
            /// <summary>
            /// 历元标记，对应于 RINEX EpochFlag
            /// 历元标志：0表示正常，1表示在前一历元与当前历元之间发生了电源故障，>1为事件标志
            /// If EVENT FLAG record (epoch flag > 1):         
            /// - Event flag:                                
            /// 2: start moving antenna                     
            /// 3: new site occupation (end of kinem. satData) (at least MARKER NAME record follows)   
            /// 4: header information follows             
            /// 5: external event (epoch is significant,same time frame as observation time tags)
            /// 6: cycle slip records follow to optionally  report detected and repaired cycle slips (same format as OBSERVATIONS records;  
            ///slip instead of observation; LLI and signal strength blank)  
            /// </summary>
            while (EpochFlag != 0 && EpochFlag % 10 != 8 && EpochFlag % 10 != 6) //8 结尾是钟跳， 6为周跳
            {
                log.Warn("历元标签出现 " + EpochFlag + ", " + FilePath);
                //4后面是注释
                for (int i = 0; i < SatCount; i++)
                {
                    line = reader.ReadLine();
                    log.Debug("RINEX 中断， " + line);
                }

                line = ReadContentLine(reader, rec.Comments);

                if (String.IsNullOrWhiteSpace(line)) { return 0; }

                EpochFlag = int.Parse(line.Substring(30, 2)); //扩展为2位，支持Gdp自定义
                SatCount = int.Parse(line.Substring(32, 3));
                loopCount++;
                if (loopCount > maxLoop)
                {
                    log.Fatal("读取 " + rec.Name + " 观测文件时，循环尝试了 " + loopCount + " 次，强制退出！");
                    break;
                }
            }

            //第一行 
            rec.RawTime = Time.Parse(line.Substring(2, 27));//时间项目有可能是不存在的，这里要判断,解析函数具有判断功能。
            rec.EpochFlag = EpochFlag;
            SatCount = int.Parse(line.Substring(32, 3));
            rec.ReceiverClockOffset = StringUtil.ParseDouble(line, 41, 15);
            return SatCount;
        }
        /// <summary>
        /// 解析一行卫星观测数据，并不读取。
        /// Rinex3.0 观测文件。
        /// </summary>
        /// <param name="header">头文件</param>
        /// <param name="line">记录卫星数据的文本行</param>
        /// <param name="isReadContentLine">标记是否读取内容行，如不，则快速略过</param>
        /// <param name="ignoreEmpty">是否忽略空值</param>
        /// <returns></returns>
        public RinexSatObsData ParseObservationLineV3(RinexObsFileHeader header, string line, bool isReadContentLine = true, bool ignoreEmpty = false)
        {
            SatelliteNumber satNum = SatelliteNumber.Parse(line.Substring(0, 3));
            var types = header.GetOrInitObsTypeCodes(satNum.SatelliteType);
            var typestr = header.ObsCodes[satNum.SatelliteType];
            int obsCount = types.Count;
            RinexSatObsData record = new RinexSatObsData();
            record.Prn = satNum;

            if (isReadContentLine)
            {
                //一颗卫星的所有观测值
                for (int j = 0; j < obsCount; j++)
                {
                    int startIndex = 3 + 16 * j;
                    int len = 16;
                    string obsStr = StringUtil.SubString(line, startIndex, len);

                    //解析  
                    bool isEmpty = String.IsNullOrEmpty(obsStr.Trim());
                    if (isEmpty && ignoreEmpty)
                    { continue; }//空值继续,如果跳过了，就对应不上头部的参数类型了，如果不跳过，则会浪费更多内存空间。2015.05.09 

                    var code = types[j];
                    var codest = typestr[j];
                    RinexObsValue obsVal = ParseObsValueV2V3(obsStr, code);

                    record.Add(codest, obsVal);
                }
            }
            return record;
        }
        /// <summary>
        /// 解析 RINEX V2 和 V3 的单数据存储部分
        /// </summary>
        /// <param name="obsStr"></param>
        /// <returns></returns>
        private RinexObsValue ParseObsValueV2V3(string obsStr, ObservationCode obsCode)
        {
            if (obsStr.Trim().Length == 0) return new RinexObsValue(0, obsCode);

            double val = StringUtil.ParseDouble(StringUtil.SubString(obsStr, 0, 14), 0);
            RinexObsValue obsVal = new RinexObsValue(val, obsCode);

            if (obsStr.Length >= 15)
            { obsVal.LossLockIndicator = (LossLockIndicator)StringUtil.ParseInt(obsStr.Substring(14, 1)); }
            if (obsStr.Length >= 16)
            { obsVal.SignalStrength = StringUtil.ParseInt(obsStr.Substring(15, 1)); }

            return obsVal;
        }

        #endregion

        #endregion

        #region 静态工具方法
        /// <summary>
        /// 测站名称
        /// </summary>
        /// <param name="path"></param>
        /// <param name="charCount"></param>
        /// <param name="isFormateName"></param>
        /// <returns></returns>
        public static string ReadSiteName(string path, int charCount = 8, bool isFormateName = true)
        {
            var header = ReadHeader(path, charCount, isFormateName);
            var name = header.SiteName;
            if (String.IsNullOrWhiteSpace(name))
            {
                name = BuildSiteName(Path.GetFileName(path), charCount);
            }
            return name;
        }

        /// <summary>
        /// 读取头文件的快捷方法。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="markerCount"></param>
        /// <param name="isFormateName"></param>
        /// <returns></returns>
        public static RinexObsFileHeader ReadHeader(string path, int markerCount = 8, bool isFormateName = true, bool isNeedEndTime = true)
        {
            using (RinexObsFileReader reader = new RinexObsFileReader(path, false, markerCount, isFormateName))
            {
                return reader.GetHeader(isNeedEndTime);
            }
        }
        /// <summary>
        /// 读取头文件的快捷方法。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="count"></param>
        /// <param name="isReadConent"></param>
        /// <returns></returns>
        public static RinexObsFile Read(string path, bool isReadConent = true, int count = int.MaxValue)
        {
            using (RinexObsFileReader reader = new RinexObsFileReader(path, isReadConent))
            {
                return reader.ReadObsFile(count);
            }
        }

        /// <summary>
        /// 历元数量。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int ReadGetEpochCount(string path)
        {
            int count = 0;
            using (RinexObsFileReader reader = new RinexObsFileReader(path, false))
            {
                while (reader.MoveNext())
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 读取下一行有内容的行，非空行，空白行。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadNextNoNullLine(StreamReader reader)
        {
            string line = reader.ReadLine();
            if (!String.IsNullOrWhiteSpace(line))
            {
                return line;
            }

            if (line == null)
            {
                return "";
            }

            return ReadNextNoNullLine(reader);

        }

        /// <summary>
        /// 读取第一个内容行。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetFirstContentLine(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                RinexUtil.SkipHeader(reader);
                return ReadContentLine(reader);
            }
        }

        /// <summary>
        /// 读取并返回 RINEX 内容行。如果有注释行，则读取注释，继续读取，返回新的内容行，空行也直接返回。
        /// </summary>
        /// <param name="r">StreamReader</param>
        /// <param name="comments">注释</param>
        /// <returns></returns>
        private static string ReadContentLine(StreamReader r, List<String> comments = null)
        {
            string line = r.ReadLine();
            if (line == null) return line;
            //判断是否是注释行

            while (IsCommentLine(line) && r.Peek() != -1)
            {
                if (comments != null) comments.Add(GetCommenValue(line));

                line = r.ReadLine();
            }

            return line;
        }
        private static string ReadContentLine(StreamReader r, out int readedCount, List<String> comments = null)
        {
            string line = r.ReadLine();
            readedCount = 1;
            if (line == null) return line;
            //判断是否是注释行

            while (IsCommentLine(line) && r.Peek() != -1)
            {
                if (comments != null) comments.Add(GetCommenValue(line));

                line = r.ReadLine();
                readedCount++;
            }

            return line;
        }
        /// <summary>
        /// 判断本行是否是注释行。即，在60列时，具有COMMENT标识。
        /// </summary>
        /// <param name="line">输入行</param>
        /// <returns></returns>
        private static bool IsCommentLine(string line)
        {
            if (line.Length >= 67)
            {
                if (line.Substring(60, 7) == RinexHeaderLabel.COMMENT)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取注释行的内容。
        /// </summary>
        /// <param name="line">输入行</param>
        /// <returns></returns>
        private static string GetCommenValue(string line)
        {
            return StringUtil.SubString(line, 0, 60).Trim();
        }

        /// <summary>
        /// 截取包含非ascii码的字符串，如汉字
        /// </summary>
        /// <param name="line"></param>
        /// <param name="nonAscCount"></param>
        /// <param name="fromIndex"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static string SubString(string line, int nonAscCount, int fromIndex, int len)
        {
            return StringUtil.SubString(line, fromIndex, len - nonAscCount).Trim();
            // return line.Substring(fromIndex, len - nonAscCount).Trim();
        }
        #endregion

        #endregion

        /// <summary>
        /// 最小比例
        /// </summary>
        /// <param name="inpath"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double GetRatioOfSatCount(string inpath, double satCount)
        {
            int count = 0;
            int all = 0;
            using (RinexObsFileReader reader = new RinexObsFileReader(inpath, false))
            {
                while (reader.MoveNext())
                {
                    if (reader.Current.Prns.Count >= satCount)
                    {
                        count++;
                    }
                    all++;
                }
            }
            return count * 1.0 / all;
        }
    }

}
