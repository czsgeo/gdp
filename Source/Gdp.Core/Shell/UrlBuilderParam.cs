//2020.02.03, czs, create in hongqing, UrlBuilderParam 

using System;
using System.Collections.Generic;
using System.Text;
using Gdp.IO;
using System.IO;
using Gdp;

namespace Gdp
{
    public class UrlBuilderParam
    {
        static ILog log = new Log(typeof(UrlBuilderParam));

        public UrlBuilderParam()
        {
            UrlModels = new List<string>()
            {
                "{UrlDirectory}/{Week}/{SourceName}{Week}{DayOfWeek}.{ProductType}.Z"
            };
            UrlDirectories = new List<string>()
            {
               "ftp://igs.ensg.eu/pub/igs/products", 
            };
            Source = new List<string> { "igs" };
            StartTime = Time.UtcNow - TimeSpan.FromDays(30);
            EndTime = StartTime + TimeSpan.FromDays(7);
            ProductType = IgsProductType.Sp3;
            Type = ProcessType.B;
            OutputPath = "BuildUrl.txt";
            IntervalSecond = 86400;

        }

        public List<string> UrlModels { get; set; }
        public List<string> UrlDirectories { get; set; }
        public List<string> Source { get; set; }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }
        public IgsProductType ProductType { get; set; }
        public int IntervalSecond { get; set; }
        /// <summary>
        /// 计算类型
        /// </summary>
        public ProcessType Type { get; set; }
        public string OutputPath { get; private set; }

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
            sb.Append(" " + ParamMarker + UrlBuildParamType.StartTime.ToString() + " ");
            sb.Append(" " + ParamMarker + UrlBuildParamType.EndTime.ToString() + " ");
         
            return sb.ToString();
        }



        #region 静态工具方法  
        static public UrlBuilderParam ParseArgs(string[] args)
        {
            var dic = ParseCmdLineToDic(args);

            return ParseDic(dic);
        }

        private static UrlBuilderParam ParseDic(Dictionary<string, List<string>> dic)
        {
            UrlBuilderParam UrlBuilderParam = new UrlBuilderParam();
   
            foreach (var item in dic)
            {
                var type = Gdp.Utils.EnumUtil.TryParse<UrlBuildParamType>(item.Key);
                var listVal = item.Value;
                switch (type)
                {
                    case UrlBuildParamType._:
                        break;
                    case UrlBuildParamType.B:
                        break;
                    case UrlBuildParamType.UrlModels:
                        UrlBuilderParam.UrlModels = listVal;
                        break;
                    case UrlBuildParamType.UrlDirectories:
                        UrlBuilderParam.UrlDirectories = listVal;
                        break;
                    case UrlBuildParamType.Source:
                        UrlBuilderParam.Source = listVal;
                        break;
                    case UrlBuildParamType.IntervalSecond:
                        UrlBuilderParam.IntervalSecond = (int)int.Parse(listVal[0]);
                        break;
                    case UrlBuildParamType.StartTime:
                        UrlBuilderParam.StartTime = Time.Parse(listVal.ToArray());
                        break;
                    case UrlBuildParamType.EndTime:
                        UrlBuilderParam.EndTime = Time.Parse(listVal.ToArray());
                        break;
                    case UrlBuildParamType.ProductType:
                        UrlBuilderParam.ProductType = (IgsProductType)Enum.Parse(typeof(IgsProductType), listVal[0]);
                        break;
                    case UrlBuildParamType.OutputPath:
                        UrlBuilderParam.OutputPath = (listVal[0]);
                        break;
                    default:
                        log.Warn(type + " Not implemented ");
                        break;
                }
            }
              
            return UrlBuilderParam;
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
    public enum UrlBuildParamType
    { 
        _,
        /// <summary>
        /// ProcessType
        /// </summary>
        B,

        UrlModels,
        UrlDirectories,
        // 
        Source,
        //
        IntervalSecond,
        // 
        StartTime,
        // 
        EndTime,
        // 
        ProductType,
        /// <summary>
        /// 
        /// </summary>
        OutputPath,
    }
}
