//2015.05.08, czs, create in namu,  RINEX O 文件写入器
//2015.10.28, czs, edit in hongqing, RINEX 2.0 文件的写入。
//2016.10.28, czs, edit in hongqing, 增加生成 Gdp 注释介绍函数
//2017.08.10, czs, edit in hongqing, 写入时间包含秒以下数字
//2017.09.26, cy, edit in chongqing, 修正换行错误，571行 9 ->8, i从0开始计算
//2018.04.18, czs & lly, edit in hmx, lly预处理发现错误，czs 修正两位数的年在处理00年时少输出一个0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Gdp.Utils;
using System.Text;


namespace Gdp.Data.Rinex
{
    /// <summary>
    /// RINEX O 文件写入器
    /// </summary>
    public class RinexObsFileWriter : IDisposable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RinexObsFileWriter()
        {

        }
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="Version">版本</param>
        public RinexObsFileWriter(string path, double Version = 3.02)
        {
            this.FilePath = path;
            this.Version = Version;
            Utils.FileUtil.CheckOrCreateDirectory(Path.GetDirectoryName(path));
            this.Writer = new StreamWriter(FilePath, false, Encoding.ASCII);
            IsUseXCodeAsPLWhenEmpty = false;
        }

        #region 基本属性
        /// <summary>
        /// 是否简洁输出，最后的变量没有值，则不输出，节约大量空间 .RINEX 3有用。
        /// </summary>
        public bool IsConcise { get; set; }
        /// <summary>
        /// RINEX3转换2，当没有P2L2时，是否用其它代替。
        /// </summary>
        public bool IsUseXCodeAsPLWhenEmpty { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public double Version { get; set; }

        /// <summary>
        /// 数据流读写器。
        /// </summary>
        public String FilePath { get; set; }
        /// <summary>
        /// 文件写
        /// </summary>
        public StreamWriter Writer { get; set; }
        /// <summary>
        /// 头部信息，所有的写入都以头部信息为基准，或者说头部信息的 ObsCodes 为基准。
        /// </summary>
        public RinexObsFileHeader Header { get; set; }
        /// <summary>
        /// 观测代码
        /// </summary>
        public Dictionary<SatelliteType, List<string>> ObsCodes { get { return this.Header.ObsCodes; } }
        #endregion

        /// <summary>
        /// 写头部文件
        /// </summary>         
        /// <param name="header"></param>
        public void WriteHeader(RinexObsFileHeader header)
        {
            if (header == null) { return; }
            this.Header = header;
            var str = ObsHeaderToRinexString(header, Version);
            this.Writer.Write(str);
        }
        /// <summary>
        /// 重写头部信息
        /// </summary>
        /// <param name="header"></param>
        public void ReWriteHeader(RinexObsFileHeader header)
        {
            this.Dispose();
            //读取
            RinexObsFile obsFile = null;
            using (RinexObsFileReader reader = new RinexObsFileReader(this.FilePath))
            {
                obsFile = reader.ReadObsFile();
                header.ObsCodes = obsFile.Header.ObsCodes;
                obsFile.Header = header;
            }
            //重写
            this.Writer = new StreamWriter(FilePath, false, Encoding.ASCII);
            this.Write(obsFile);
        }

        /// <summary>
        /// 写一个观测历元
        /// </summary>
        /// <param name="epochObs"></param>
        public void WriteEpochObservation(RinexEpochObservation epochObs)
        {
            string rinex = null;
            if (Version >= 3)
            {
                rinex = GetRecordStringV3(epochObs);
            }
            else
            {
                rinex = GetRecordStringV2(epochObs);
            }

            Writer.Write(rinex);
        }

        /// <summary>
        /// 写入到指定的文件路径
        /// </summary>
        /// <param name="obsFile"></param>
        /// <param name="minIntervalSeconds"></param>
        public void Write(RinexObsFile obsFile, int minIntervalSeconds = 0)
        {
            Write(Writer, obsFile, obsFile.Header.Version, minIntervalSeconds);
        }

        #region 静态工具方法
        #region  写文件
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="obsFile"></param>
        /// <param name="version"></param>
        /// <param name="minIntervalSeconds"></param>
        public void Write(StreamWriter Writer, RinexObsFile obsFile, double version, int minIntervalSeconds = 0)
        {
            string rinex = GetRinexString(obsFile, version, minIntervalSeconds);

            Writer.Write(rinex);
        }
        /// <summary>
        ///  to RINEX String，自动采用观测文件内容更新时间。
        /// </summary>
        /// <param name="obsFile"></param>
        /// <param name="minIntervalSeconds">最小采样率</param>
        public string GetRinexString(RinexObsFile obsFile, double version, int minIntervalSeconds = 0)
        {
            if (obsFile == null || obsFile.Count == 0) { return ""; }

            this.Header = obsFile.Header;
            obsFile.Header.StartTime = obsFile[0].ReceiverTime;
            if (obsFile.Count > 1)
            {
                obsFile.Header.EndTime = obsFile[obsFile.Count - 1].ReceiverTime;
                obsFile.Header.Interval = (obsFile[1].ReceiverTime - obsFile[0].ReceiverTime);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(ObsHeaderToRinexString(obsFile.Header, version));
            //更新采样率
            if (obsFile.Header.Interval < minIntervalSeconds) { obsFile.Header.Interval = minIntervalSeconds; }

            RinexEpochObservation prevEpochObs = null;
            int i = 0;
            foreach (var item in obsFile.ToArray())
            {
                if (item.Count == 0) { continue; }

                if (prevEpochObs == null) { prevEpochObs = item; }//第一次

                var differ = item.ReceiverTime - prevEpochObs.ReceiverTime;
                if (differ >= minIntervalSeconds || i == 0)
                {
                    if (version >= 3)
                    {
                        sb.Append(GetRecordStringV3(item));
                    }
                    else
                    {
                        sb.Append(GetRecordStringV2(item));
                    }

                    prevEpochObs = item;//roll
                }
                i++;
            }
            return sb.ToString();
        }
        #endregion

        #region 写记录内容
        /// <summary>
        /// 一个历元观测量
        /// </summary>
        /// <param name="epochObs"></param>
        /// <returns></returns>
        public string GetRecordStringV2(RinexEpochObservation epochObs)
        {
            if (epochObs.Count == 0) { return ""; }

            var prns = GetOutputPrns(epochObs);
            StringBuilder sb = new StringBuilder();
            var OneBlankSpace = " ";
            sb.Append(OneBlankSpace);//第一个为空格
            var epoch = epochObs.ReceiverTime;
            var firstLine = epoch.SubYear.ToString("00") //两位数的年
                + OneBlankSpace + epoch.Month.ToString("00")
                + OneBlankSpace + epoch.Day.ToString("00")
                + OneBlankSpace + epoch.Hour.ToString("00")
                + OneBlankSpace + epoch.Minute.ToString("00")
                + OneBlankSpace + epoch.Seconds.ToString("00.0000000")//F11.7
                + StringUtil.FillSpaceLeft(epochObs.EpochFlag + "", 3)
                + StringUtil.FillSpaceLeft(prns.Count + "", 3);

            sb.Append(firstLine);

            var firstEnd = StringUtil.FillSpaceLeft(epochObs.ReceiverClockOffset.ToString("0.000000000"), 12);
            //卫星列表
            int satIndex = 0;
            //var prns = epochObs.Prns;
            int satCount = prns.Count;
            int maxSatCountInLine = 12;
            StringBuilder line = new StringBuilder();
            foreach (var item in prns)
            {
                line.Append(item.ToString());
                satIndex++;

                //卫星数量小于maxSatCountInLine个，且到最后一个时。
                if (satIndex == satCount && satCount < maxSatCountInLine)
                {
                    AppendObsFirstEnd(sb, firstEnd, line);
                }

                //卫星数量大于等于12，且到了12的倍数个时
                if (satIndex % maxSatCountInLine == 0)
                {
                    if (satIndex / maxSatCountInLine == 1)//第一行
                    {
                        AppendObsFirstEnd(sb, firstEnd, line);
                    }
                    else//第 1 行以后
                    {
                        AppendToString(sb, line);
                    }

                    //如果后面还有卫星，则应该换行继续。
                    if (satIndex < satCount)
                    {
                        sb.AppendLine();
                        sb.Append(StringUtil.Fill("", 32));
                    }
                }

                //最后一颗卫星,全部上缴
                if (satIndex == satCount)
                {
                    AppendToString(sb, line);
                }
            }
            //结束行
            sb.AppendLine();

            //内容部分 
            var obsCodes = Header.GetObsCodesV2();
            foreach (var prn in prns)//遍历每一颗卫星
            {
                var obs = epochObs[prn];
                //一行只能记录5个，超出后换行
                int index = 0;
                int leftCount = obsCodes.Count;
                foreach (var type in obsCodes)//遍历每一个类型 如 C1，L1
                {
                    var val = obs.TryGetObsValue(type);
                    if ((val == null || val.Value == 0) && IsUseXCodeAsPLWhenEmpty)
                    {
                        if (type == "L2")
                        {
                            val = obs.PhaseB;
                        }
                        else if (type == "P2")
                        {
                            val = obs.RangeB;
                        }
                    }


                    var str = ToRinexValueString(val);

                    sb.Append(str);
                    index++;
                    leftCount--;
                    if (index >= 5 && leftCount > 0)
                    {
                        sb.AppendLine();
                        index = 0;
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static void AppendToString(StringBuilder sb, StringBuilder line)
        {
            sb.Append(line.ToString());
            line.Clear();
        }

        private static void AppendObsFirstEnd(StringBuilder sb, string firstEnd, StringBuilder line)
        {
            sb.Append(Utils.StringUtil.FillSpaceRight(line.ToString(), 36) + firstEnd);
            line.Clear();
        }

        /// <summary>
        /// 获取将要输出的卫星列表
        /// </summary>
        /// <param name="epochObs"></param>
        /// <returns></returns>
        public List<SatelliteNumber> GetOutputPrns(RinexEpochObservation epochObs)
        {
            return epochObs.Prns.Where(m => ObsCodes.Keys.Contains(m.SatelliteType)).ToList();
        }

        /// <summary>
        /// 一个历元观测量
        /// </summary>
        /// <param name="epochObs"></param> 
        /// <returns></returns>
        public string GetRecordStringV3(RinexEpochObservation epochObs)
        {
            if (epochObs.Count == 0) { return ""; }

            var prns = GetOutputPrns(epochObs);

            StringBuilder sb = new StringBuilder();
            var OneBlankSpace = " ";
            sb.Append("> ");
            var epoch = epochObs.ReceiverTime;
            var firstLine = epoch.Year.ToString("0000")  //四位数的年
                 + OneBlankSpace + epoch.Month.ToString("00")
                + OneBlankSpace + epoch.Day.ToString("00")
                + OneBlankSpace + epoch.Hour.ToString("00")
                + OneBlankSpace + epoch.Minute.ToString("00")
                + OneBlankSpace + epoch.Seconds.ToString("00.0000000")//F11.7
                + StringUtil.FillSpaceLeft(epochObs.EpochFlag + "", 3)
                + StringUtil.FillSpaceLeft(prns.Count + "", 3)
                + StringUtil.FillSpaceLeft("", 6)
                + StringUtil.FillSpaceLeft(epochObs.ReceiverClockOffset.ToString("0.000000000000"), 15)
                ;
            sb.AppendLine(firstLine);

            foreach (var prn in prns)
            {
                var sat = epochObs[prn];
                sb.Append(prn.ToString());//PRN可能有三位？
                if (sat != null)
                {
                    var obstypes = ObsCodes[prn.SatelliteType];
                    int emptyCount = 0;
                    foreach (var type in obstypes)
                    {
                        var val = sat.TryGetObsValue(type);

                        #region 简明输出处理逻辑
                        if (IsConcise)//简明输出
                        {
                            if (val == null)//若为空，则跳过，记录次数
                            {
                                emptyCount++;
                                continue;
                            }
                            else//若不是空，则补足前期数据。              
                            {
                                for (int i = 0; i < emptyCount; i++)
                                {
                                    sb.Append(Utils.StringUtil.FillSpace("  ", 16));
                                }
                                emptyCount = 0;
                            }
                        }
                        #endregion

                        if (val != null && (Double.IsNaN(val.Value) || Double.IsInfinity(val.Value)))
                        {
                            val.Value = 0;
                        }
                        sb.Append(ToRinexValueString(val));
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// V2 和 V3 格式相同，都是14为记录，3为小数，1位LLI，1位信号强度。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static string ToRinexValueString(RinexObsValue val)
        {
            if (val == null) { val = RinexObsValue.Zero; }

            StringBuilder subSb = new StringBuilder();
            subSb.Append(StringUtil.FillSpaceLeft(val.Value.ToString("0.000"), 14));

            if (val.LossLockIndicator == 0) subSb.Append(" ");
            else subSb.Append((int)val.LossLockIndicator);

            if (val.SignalStrength == 0) subSb.Append(" ");
            else subSb.Append(val.SignalStrength.ToString());
            var str = subSb.ToString();
            return str;
        }
        #endregion

        #region 写 RINEX 头文件
        /// <summary>
        ///  RINEX文件的头文件。
        /// </summary>
        /// <param name="header">头部文件</param>
        /// <param name="version">版本</param>
        /// <returns></returns>
        public static string ObsHeaderToRinexString(RinexObsFileHeader header, double version)
        {
            StringBuilder sb = new StringBuilder();

            BuildHeaderLine(header, version, sb);
            // return "";

            // HeaderLabel.PGM_RUN_BY_DATE:
            BuildProgramRunByDateLine(sb);

            // HeaderLabel.COMMENT:   
            sb.AppendLine(BuildGnsserCommentLines());

            foreach (var item in header.Comments)
            {
                if (item.Contains("Gnsser")) { continue; } //不重复写Gnsser信息。
                if (String.IsNullOrWhiteSpace(item)) continue;
                sb.AppendLine(StringUtil.FillSpace(item, 60) + RinexHeaderLabel.COMMENT);
            }

            BuildReceiverAntennaInfoText(header, sb);

            BuildCoordInfoText(header, sb);

            if (version >= 3)
            {
                BuildObsCodeLineRinexV3(header, sb);
            }
            else
            {
                BuildObsCodeLineRinexV2(header, sb);
            }

            sb.AppendLine(StringUtil.FillSpace(
                StringUtil.FillSpaceLeft(header.Interval.ToString("0.000"), 10),
                60) + RinexHeaderLabel.INTERVAL);

            BuildDateTimeInfo(header, sb);

            //SYS / PHASE SHIFT
            if (header.SystemPhaseShift != null && header.SystemPhaseShift.Count > 0)
            {
                BuildSystemPhaseShiftText(header, sb);
            }

            //RINEX 3.3
            if (header.GlonassSlotFreqNums != null && header.GlonassSlotFreqNums.Count > 0)
            {
                BuildGlonassSlotFreqNumsText(header, sb);
            }

            //GLONASS_COD_PHS_BIS
            // C1C    0.000 C1P    0.000 C2C    0.000 C2P    0.000        GLONASS COD/PHS/BIS 
            if (header.GlonassCodePhaseBias != null && header.GlonassCodePhaseBias.Count > 0)
            {
                BuildGlonassCodePhaseBiasText(header, sb);
            }
            //end header
            sb.AppendLine(StringUtil.FillSpace("", 60) + RinexHeaderLabel.END_OF_HEADER);
            return sb.ToString();
        }

        #region 头文件文本构建细节
        private static void BuildDateTimeInfo(RinexObsFileHeader header, StringBuilder sb)
        {
            sb.AppendLine(
                 StringUtil.FillSpace(
                StringUtil.FillSpaceLeft(header.StartTime.Year.ToString(), 6) +
                StringUtil.FillSpaceLeft(header.StartTime.Month.ToString(), 6) +
                StringUtil.FillSpaceLeft(header.StartTime.Day.ToString(), 6) +
                StringUtil.FillSpaceLeft(header.StartTime.Hour.ToString(), 6) +
                StringUtil.FillSpaceLeft(header.StartTime.Minute.ToString(), 6) +
                StringUtil.FillSpaceLeft(header.StartTime.Seconds.ToString("0.0000000"), 13) +
                StringUtil.FillSpaceLeft(header.TimeSystem, 8),
                60) + RinexHeaderLabel.TIME_OF_FIRST_OBS);

            if (header.EndTime > header.StartTime && header.EndTime != Time.Default && header.EndTime != Time.MaxValue && header.EndTime != Time.MinValue)
            {
                sb.AppendLine(
                  StringUtil.FillSpace(
                 StringUtil.FillSpaceLeft(header.EndTime.Year.ToString(), 6) +
                 StringUtil.FillSpaceLeft(header.EndTime.Month.ToString(), 6) +
                 StringUtil.FillSpaceLeft(header.EndTime.Day.ToString(), 6) +
                 StringUtil.FillSpaceLeft(header.EndTime.Hour.ToString(), 6) +
                 StringUtil.FillSpaceLeft(header.EndTime.Minute.ToString(), 6) +
                 StringUtil.FillSpaceLeft(header.EndTime.Seconds.ToString("0.0000000"), 13) +
                 StringUtil.FillSpaceLeft(header.TimeSystem, 8),
                 60) + RinexHeaderLabel.TIME_OF_LAST_OBS);
            }
            sb.AppendLine(StringUtil.FillSpace(StringUtil.FillSpaceLeft(header.LeapSeconds, 6), 60) + RinexHeaderLabel.LEAP_SECONDS);
        }

        private static void BuildCoordInfoText(RinexObsFileHeader header, StringBuilder sb)
        {
            if (header.ApproxXyz != null)
            {
                var xyz = header.ApproxXyz;
                var rinexApproxXyzLine = BuildApproxXyzLine(xyz);

                sb.AppendLine(rinexApproxXyzLine);

            }
            if (header.Hen != null)
            {
                sb.AppendLine(StringUtil.FillSpace(
                StringUtil.FillSpaceLeft(header.Hen.H.ToString("0.0000"), 14) +
                StringUtil.FillSpaceLeft(header.Hen.E.ToString("0.0000"), 14) +
                StringUtil.FillSpaceLeft(header.Hen.N.ToString("0.0000"), 14), 60
                ) + RinexHeaderLabel.ANTENNA_DELTA_H_E_N);
            }
            if (header.AntDeltaXyz != null)
            {
                sb.AppendLine(StringUtil.FillSpace(
                  StringUtil.FillSpaceLeft(header.AntDeltaXyz.X.ToString("0.0000"), 14) +
                  StringUtil.FillSpaceLeft(header.AntDeltaXyz.Y.ToString("0.0000"), 14) +
                  StringUtil.FillSpaceLeft(header.AntDeltaXyz.Z.ToString("0.0000"), 14), 60
                  ) + RinexHeaderLabel.ANTENNA_DELTA_XYZ);
            }
        }

        private static void BuildReceiverAntennaInfoText(RinexObsFileHeader header, StringBuilder sb)
        {
            //HeaderLabel.MARKER_NAME:
            sb.AppendLine(StringUtil.FillSpace(header.SiteInfo.SiteName, 60) + RinexHeaderLabel.MARKER_NAME);
            sb.AppendLine(StringUtil.FillSpace(header.SiteInfo.MarkerNumber, 60) + RinexHeaderLabel.MARKER_NUMBER);
            if (header.MarkerType != null)
                sb.AppendLine(StringUtil.FillSpace(header.MarkerType, 60) + RinexHeaderLabel.MARKER_TYPE);
            sb.AppendLine(StringUtil.FillSpace(header.ObsInfo.ObserverAgence, 60) + RinexHeaderLabel.OBSERVER_AGENCY);
            sb.Append(StringUtil.FillSpace(header.SiteInfo.ReceiverNumber, 20));
            sb.Append(StringUtil.FillSpace(header.SiteInfo.ReceiverType, 20));
            sb.AppendLine(StringUtil.FillSpace(header.SiteInfo.ReceiverVersion, 20) + RinexHeaderLabel.REC_NUM_TYPE_VERS);

            sb.Append(StringUtil.FillSpace(header.SiteInfo.AntennaNumber, 20));
            sb.AppendLine(StringUtil.FillSpace(header.SiteInfo.AntennaType, 40) + RinexHeaderLabel.ANT_NUM_TYPE);
        }

        private static void BuildProgramRunByDateLine(StringBuilder sb)
        {
            sb.Append(StringUtil.FillSpace("Gnsser", 20));
            sb.Append(StringUtil.FillSpace("GeoSolution", 20));
            sb.Append(StringUtil.FillSpace(DateTime.UtcNow.ToString("yyyyMMdd HHmmss") + " UTC", 20));
            sb.AppendLine(RinexHeaderLabel.PGM_RUN_BY_DATE);
        }

        private static void BuildHeaderLine(RinexObsFileHeader header, double version, StringBuilder sb)
        {
            // HeaderLabel.RINEX_VERSION_TYPE:
            sb.Append(StringUtil.FillSpace(StringUtil.FillSpaceLeft(version, 9), 20));
            sb.Append(StringUtil.FillSpace("OBSERVATION DATA", 20));
            sb.Append(StringUtil.FillSpace(header.GetSatTypeMarker().ToString(), 20));
            sb.AppendLine(RinexHeaderLabel.RINEX_VERSION_TYPE);
        }

        private static void BuildSystemPhaseShiftText(RinexObsFileHeader header, StringBuilder sb)
        {
            var sameTypePrns = header.GetCodeSatTypedPrns();
            foreach (var typeKv in sameTypePrns)
            {
                foreach (var prnKv in typeKv.Value)
                {
                    var satType = typeKv.Key;
                    int satCount = prnKv.Value.Count;
                    var code = prnKv.Key;
                    var val = header.GetSystemPhaseShift(prnKv.Value[0], code);

                    StringBuilder ssb = new StringBuilder();
                    ssb.Append(satType);
                    ssb.Append(" ");
                    ssb.Append(code);
                    ssb.Append(" ");
                    var valStr = StringUtil.FillSpaceLeft(val.ToString("0.00000"), 8);
                    ssb.Append(valStr);
                    ssb.Append(" ");
                    var satCountStr = StringUtil.FillSpaceLeft(satCount, 3);
                    ssb.Append(satCountStr);
                    int i = -1;
                    foreach (var prn in prnKv.Value)
                    {
                        i++;
                        if (i % 10 == 0 && i != 0)
                        {
                            var line = StringUtil.FillSpace(ssb.ToString(), 60) + RinexHeaderLabel.SYS_PHASE_SHIFT;
                            sb.AppendLine(line);
                            ssb.Clear();

                            ssb.Append(StringUtil.FillSpaceLeft(0, 18));
                        }

                        ssb.Append(" ");
                        ssb.Append(prn);
                    }
                    if (ssb.Length > 0)
                    {
                        var line = StringUtil.FillSpace(ssb.ToString(), 60) + RinexHeaderLabel.SYS_PHASE_SHIFT;
                        sb.AppendLine(line);
                        ssb.Clear();

                        ssb.Append(StringUtil.FillSpaceLeft(0, 18));
                    }
                }
            }
        }

        private static void BuildGlonassSlotFreqNumsText(RinexObsFileHeader header, StringBuilder sb)
        {
            int length = header.GlonassSlotFreqNums.Count;
            string line = Utils.StringUtil.FillSpaceLeft(length + "", 3) + " ";
            int i = 0;
            foreach (var kv in header.GlonassSlotFreqNums)
            {
                if (i % 8 == 0 && i != 0)
                {
                    sb.AppendLine(StringUtil.FillSpace(line, 60) + RinexHeaderLabel.GLONASS_SLOT_FRQ);
                    line = "    ";
                }
                line += kv.Key + " " + Utils.StringUtil.FillSpaceLeft(kv.Value, 2) + " ";//3 + 1 + 2 + 1
                i++;
            }
            if (line.Length > 4)//最后检查一次
            {
                sb.AppendLine(StringUtil.FillSpace(line, 60) + RinexHeaderLabel.GLONASS_SLOT_FRQ);
            }
        }

        private static void BuildGlonassCodePhaseBiasText(RinexObsFileHeader header, StringBuilder sb)
        {
            int length = header.GlonassCodePhaseBias.Count;
            string line = "";
            int i = 0;
            foreach (var kv in header.GlonassCodePhaseBias)
            {
                if (i % 4 == 0 && i != 0)
                {
                    sb.AppendLine(StringUtil.FillSpace(line, 60) + RinexHeaderLabel.GLONASS_COD_PHS_BIS);
                    line = "";
                }
                line += " " + kv.Key + " " + Utils.StringUtil.FillSpaceLeft(kv.Value.ToString("0.000"), 8);
                i++;
            }
            if (line.Length > 1) //最后检查一次
            {
                sb.AppendLine(StringUtil.FillSpace(line, 60) + RinexHeaderLabel.GLONASS_COD_PHS_BIS);
            }
        }
        /// <summary>
        /// 构建Gnsser 注释信息，以字符串返回，末尾未加换行符。
        /// </summary>
        /// <returns></returns>
        public static string BuildGnsserCommentLines()
        {
            StringBuilder sb = new StringBuilder();
            var writingInfo = "Built by Gnsser. www.gnsser.com, gnsser@163.com";
            var writingCommentLine = BuildCommentLine(writingInfo);
            sb.AppendLine(writingCommentLine);
            var gnsserInfo = "Gnsser, a GNSS data processing software, created in China";
            var gnsserCommentLine = BuildCommentLine(gnsserInfo);
            sb.Append(gnsserCommentLine);
            return sb.ToString();
        }


        /// <summary>
        /// 构建RINEX注释行
        /// </summary>
        /// <param name="writingInfo"></param>
        /// <returns></returns>
        public static string BuildCommentLine(string writingInfo)
        {
            if (writingInfo.Length > 60) { writingInfo = writingInfo.Substring(0, 60); }
            var writingCommentLine = StringUtil.FillSpace(writingInfo, 60) + RinexHeaderLabel.COMMENT;
            return writingCommentLine;
        }
        /// <summary>
        /// 构建初始坐标行。含标签。
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public static string BuildApproxXyzLine(XYZ xyz)
        {
            var rinexApproxXyzLine = StringUtil.FillSpace(
            StringUtil.FillSpaceLeft(xyz.X.ToString("0.0000"), 14) +
            StringUtil.FillSpaceLeft(xyz.Y.ToString("0.0000"), 14) +
            StringUtil.FillSpaceLeft(xyz.Z.ToString("0.0000"), 14), 60
            ) + RinexHeaderLabel.APPROX_POSITION_XYZ;
            return rinexApproxXyzLine;
        }
        /// <summary>
        /// 获取 RINEX2.0 形式的观测码文本行。多系统情况下还需要解决。！！2015.10.28
        /// </summary>
        /// <param name="header"></param>
        /// <param name="sb"></param>
        private static void BuildObsCodeLineRinexV2(RinexObsFileHeader header, StringBuilder sb)
        {
            List<string> obsTypes = header.GetObsCodesV2();// new List<string>();
            int allSatCount = obsTypes.Count;
            ////每一类型卫星遍历
            //foreach (var satType in header.SatelliteTypes)
            //{
            //    var types = header.GetOrInitObsCodes(satType);
            //    obsTypes.AddRange(types);
            //    allSatCount += types.Count;
            //}
            //第一行
            StringBuilder lineSb = new StringBuilder();

            lineSb.Append(StringUtil.FillSpaceRight("", 3));//3个空格 
            lineSb.Append(StringUtil.FillSpaceLeft(allSatCount + "", 3));//3 个字符

            //一行最多13个
            int i = 0;
            foreach (var valType in obsTypes)
            {
                //此处还需要做转换
                var val = valType;
                if (val.Length == 3)
                {
                    val = val.Substring(0, 2);
                }
                lineSb.Append(StringUtil.FillSpaceLeft(val, 6));


                if (i % 8 == 0 && i != 0) //换行
                {
                    sb.AppendLine(StringUtil.FillSpaceRight(lineSb.ToString(), 60) + RinexHeaderLabel.TYPES_OF_OBSERV);
                    lineSb.Clear();
                    lineSb.Append(StringUtil.FillSpaceLeft("", 6));
                }
                i++;
            }

            if (lineSb.Length != 0)
            {
                sb.AppendLine(StringUtil.FillSpaceRight(lineSb.ToString(), 60) + RinexHeaderLabel.TYPES_OF_OBSERV);
                lineSb.Clear();
            }
        }

        private static void BuildObsCodeLineRinexV3(RinexObsFileHeader header, StringBuilder sb)
        {
            //每一类型卫星遍历
            foreach (var satType in header.SatelliteTypes)
            {
                //第一行
                var types = header.GetOrInitObsCodes(satType);
                StringBuilder lineSb = new StringBuilder();
                lineSb.Append(StringUtil.FillSpaceRight(satType.ToString(), 3));
                var count = types.Count;
                lineSb.Append(StringUtil.FillSpaceLeft(count + "", 3));
                double lineCount = Math.Ceiling(count / 13.0d);
                //一行最多13个
                int i = 0;
                foreach (var valType in types)
                {
                    //此处还需要做转换 
                    var val = new ObservationCode(valType).GetRinexCode(3);
                    lineSb.Append(StringUtil.FillSpaceLeft(val, 4));

                    i++;

                    if (i % 13 == 0 && i != 0)
                    {
                        sb.AppendLine(StringUtil.FillSpaceRight(lineSb.ToString(), 60) + RinexHeaderLabel.SYS_OBS_TYPES);
                        lineSb.Clear();
                        lineSb.Append(StringUtil.FillSpaceLeft("", 6));
                    }
                }
                if (lineSb.Length != 0)
                {
                    sb.AppendLine(StringUtil.FillSpaceRight(lineSb.ToString(), 60) + RinexHeaderLabel.SYS_OBS_TYPES);
                    lineSb.Clear();
                }
            }
        }

        #endregion
        #endregion
        #endregion

        public void Dispose()
        {
            if (Writer != null)
            {
                this.Writer.Close();
                this.Writer.Dispose();
                this.Writer = null;
            }
        }
        /// <summary>
        /// 立刻写入
        /// </summary>
        public void Flush()
        {
            this.Writer.Flush();
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="obsFile"></param>
        /// <param name="path"></param>
        /// <param name="verion"></param>
        /// <param name="minInterval"></param>
        public static void Write(RinexObsFile obsFile, string path, double verion = -1, int minInterval = 1)
        {
            if (verion < 2) { verion = obsFile.Header.Version; }
            using (RinexObsFileWriter writer = new RinexObsFileWriter(path, verion))
            {
                writer.Write(obsFile, minInterval);
            }
        }
    }




}
