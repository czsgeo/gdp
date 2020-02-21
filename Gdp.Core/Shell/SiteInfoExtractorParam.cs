//2020.02.22, czs, create in hongqing, SiteInfoExtractorParam 

using System;
using System.Collections.Generic;
using System.Text;
using Gdp.IO;
using System.IO;
using Gdp;

namespace Gdp
{
    public class SiteInfoExtractorParam
    {
        static ILog log = new Log(typeof(SiteInfoExtractorParam));

        public SiteInfoExtractorParam()
        {
            OutputDirectory = "./Temp"; 
            Type = ProcessType.E;   
        } 
        public string OutputDirectory { get; set; }

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
            sb.Append(" " + ParamMarker + SiteInfoExtractorParamType.OutputDirectory.ToString() + " "); 
         
            return sb.ToString();
        }
         

        #region 静态工具方法  
        static public SiteInfoExtractorParam ParseArgs(string[] args)
        {
            var dic = ParseCmdLineToDic(args);

            return ParseDic(dic);
        }

        private static SiteInfoExtractorParam ParseDic(Dictionary<string, List<string>> dic)
        {
            SiteInfoExtractorParam SiteInfoExtractorParam = new SiteInfoExtractorParam();
            foreach (var item in dic)
            {
                var type = Gdp.Utils.EnumUtil.TryParse<SiteInfoExtractorParamType>(item.Key);
                var listVal = item.Value;
                switch (type)
                {
                    case SiteInfoExtractorParamType._:
                        break;
                    case SiteInfoExtractorParamType.E:
                        break; 
                    case SiteInfoExtractorParamType.OutputDirectory:
                        SiteInfoExtractorParam.OutputDirectory = (listVal[0]);
                        break;
                    case SiteInfoExtractorParamType.InputPath: 
                        SiteInfoExtractorParam.InputPath = (listVal);
                        break;
                    default:
                        log.Warn(type + " Not implemented ");
                        break;
                }
            }
              
            return SiteInfoExtractorParam;
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
    public enum SiteInfoExtractorParamType
    { 
        _,
        /// <summary>
        /// ProcessType
        /// </summary>
        E,

        InputPath,
        OutputDirectory, 
    }
}
