//2020.02.03, czs, create in hongqing, FormatObsParam 

using System;
using System.Collections.Generic;
using System.Text;
using Gdp.IO;
using System.IO;
using Gdp;

namespace Gdp
{
    public class FormatObsParam
    {
        static ILog log = new Log(typeof(FormatObsParam));

        public FormatObsParam()
        {
            Option = new ObsFileConvertOption();
            Option.OutputDirectory = "./Formated"; 
            Type = ProcessType.F;   
        }
        /// <summary>
        /// Option
        /// </summary>
        public ObsFileConvertOption Option { get; set; }

        /// <summary>
        /// 计算类型
        /// </summary>
        public ProcessType Type { get; set; }
        public List<string> InputPath { get; private set; }

        #region 字符常量
        /// <summary>
        /// 数组分隔符，分号
        /// </summary>
        public const char ArraySpliter = ' ';
        /// <summary>
        /// 参数起始符
        /// </summary>
        public const char ParamMarker = '-';
        /// <summary>
        /// 参数赋值符号
        /// </summary>
        public const char AsignMarker = ':';
        #endregion


        public override string ToString()
        {
            return ToLine();
        }
        
        /// <summary>
        /// 转换为一行命令
        /// </summary>
        /// <returns></returns>
        public string ToLine()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" " +  Type.ToString());
            sb.Append(" " + ParamMarker + FormatObsParamType.OutputDirectory.ToString() + " "); 
         
            return sb.ToString();
        }
         

        #region 静态工具方法  
        static public FormatObsParam ParseArgs(string[] args)
        {
            var dic = ParseCmdLineToDic(args);

            return ParseDic(dic);
        }

        private static FormatObsParam ParseDic(Dictionary<string, List<string>> dic)
        {
            FormatObsParam FormatObsParam = new FormatObsParam();
            var Option = FormatObsParam.Option;
            foreach (var item in dic)
            {
                var type = Gdp.Utils.EnumUtil.TryParse<FormatObsParamType>(item.Key);
                var listVal = item.Value;
                switch (type)
                {
                    case FormatObsParamType._:
                        break;
                    case FormatObsParamType.F:
                        break; 
                    case FormatObsParamType.OutputDirectory:
                        Option.OutputDirectory = (listVal[0]);
                        break;
                    case FormatObsParamType.InputPath: 
                        FormatObsParam.InputPath = (listVal);
                        break;
                    case FormatObsParamType.OutVersion:
                        Option.IsEnableRinexVertion = true;
                        Option.Version = Gdp.Utils.DoubleUtil.TryParse(listVal[0]);
                        break;
                    case FormatObsParamType.SatelliteType:
                        var sats = new List<SatelliteType>();
                        foreach (var val in listVal)
                        {
                            var satType = Utils.EnumUtil.TryParse<SatelliteType>(val);
                            sats.Add(satType);
                        }
                        Option.IsEnableSatelliteTypes = true;
                        Option.SatelliteTypes = sats;
                        break; 
                    case FormatObsParamType.StartTime:
                        Option.IsEnableTimePeriod = true;
                        Option.TimePeriod.Start = Time.Parse(listVal.ToArray());
                        break;
                    case FormatObsParamType.EndTime:
                        Option.IsEnableTimePeriod = true;
                        Option.TimePeriod.End = Time.Parse(listVal.ToArray());
                        break;
                    case FormatObsParamType.Interval:
                        Option.Interval = Utils.IntUtil.ParseInt(listVal[0]);
                        break;
                    default:
                        log.Warn(type + " Not implemented ");
                        break;
                }
            }
              
            return FormatObsParam;
        }

        static public Dictionary<string, List<string>> ParseCmdLineToDic(string [] args)
        {
            var dic = new Dictionary<string, List<string>>();
            var currentType = "_";
            foreach (var item in args)
            {
                if (item.StartsWith(ParamMarker.ToString()))
                {
                    currentType = (item).TrimStart(ParamMarker);
                    dic[currentType] = new List<string>();
                    continue;
                } 

                //解析内容，主要为解析数组
                var cmd = item.TrimStart(' ');
                cmd = cmd.Trim(); //

                //var vals = cmd.Split(new char[] { ArraySpliter }, StringSplitOptions.RemoveEmptyEntries);

                if (dic.ContainsKey(currentType))
                {
                    dic[currentType].Add(cmd);
                }
                else
                {
                    dic[currentType] = new List<string>() { cmd };
                }

            }
            return dic;
        } 
        #endregion 
    } 

    /// <summary>
    /// 参数类型,一个字符起始
    /// </summary>
    public enum FormatObsParamType
    { 
        _,
        /// <summary>
        /// ProcessType
        /// </summary>
        F,

        InputPath,
        OutputDirectory, 
        OutVersion,
        Interval,
        SatelliteType,
        StartTime,
        EndTime,

    }
}
