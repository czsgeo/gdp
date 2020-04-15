//2020.02.03, czs, create in hongqing, UrlDownloaderParam 

using System;
using System.Collections.Generic;
using System.Text;
using Gdp.IO;
using System.IO;
using Gdp;

namespace Gdp
{
    public class UrlDownloaderParam
    {
        static ILog log = new Log(typeof(UrlDownloaderParam));

        public UrlDownloaderParam()
        { 
            Type = ProcessType.D;
            DownloadDirectory = "./Download";
            UrlTextPath = "BuildUrl.txt";
            IsOverWrite = false;
        }
          
        /// <summary>
        /// 计算类型
        /// </summary>
        public ProcessType Type { get; set; }
        public string DownloadDirectory { get; private set; }
        public string UrlTextPath { get; private set; }
        public bool IsOverWrite { get; internal set; }

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
            sb.Append(" " + ParamMarker + UrlDownloaderParamType.DownloadDirectory.ToString() + " ");
            sb.Append(" " + ParamMarker + DownloadDirectory + " ");
         
            return sb.ToString();
        }



        #region 静态工具方法  
        static public UrlDownloaderParam ParseArgs(string[] args)
        {
            var dic = ParseCmdLineToDic(args);

            return ParseDic(dic);
        }

        private static UrlDownloaderParam ParseDic(Dictionary<string, List<string>> dic)
        {
            UrlDownloaderParam UrlDownloaderParam = new UrlDownloaderParam();
   
            foreach (var item in dic)
            {
                var type = Gdp.Utils.EnumUtil.TryParse<UrlDownloaderParamType>(item.Key);
                var listVal = item.Value;
                switch (type)
                {
                    case UrlDownloaderParamType._:
                        break;
                    case UrlDownloaderParamType.D:
                        break; 
                    case UrlDownloaderParamType.DownloadDirectory:
                        UrlDownloaderParam.DownloadDirectory = (listVal[0]);
                        break;
                    case UrlDownloaderParamType.UrlTextPath:
                        UrlDownloaderParam.UrlTextPath = (listVal[0]);
                        break;
                    case UrlDownloaderParamType.IsOverWrite:
                        UrlDownloaderParam.IsOverWrite = Boolean.Parse(listVal[0]);
                        break;
                    default:
                        log.Warn(type + " Not implemented ");
                        break;
                }
            }
              
            return UrlDownloaderParam;
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
    public enum UrlDownloaderParamType
    { 
        _,
        /// <summary>
        /// ProcessType
        /// </summary>
        D,

        UrlTextPath,
        DownloadDirectory,
        IsOverWrite,
    }
}
