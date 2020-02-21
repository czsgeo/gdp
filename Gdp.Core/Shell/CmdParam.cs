//2019.12.16, czs, create in xian->zhengzhou train G2052,  命令行参数类

using System;
using System.Collections.Generic;
using System.Text;
using Gdp.IO;
using System.IO;
using Gdp;

namespace Gdp
{
    public class CmdParam
    {
        static ILog log = new Log(typeof(CmdParam));

        public CmdParam()
        {  
        }  

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
        /// <summary>
        /// 计算类型
        /// </summary>
        public ProcessType SolveType { get; set; }

        public double Version = 1.0; 

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
            sb.Append(" " + ParamMarker +   SolveType.ToString());
            sb.Append(" " + ParamMarker + " ");
            int i = -1; 

            return sb.ToString();
        }



        #region 静态工具方法  
        static public CmdParam ParseArgs(string[] args)
        {
            var dic = ParseCmdLineToDic(args);

            CmdParam cmdParam = new CmdParam()
            {
                SolveType = dic
            };

            return cmdParam;
        }
         
        static public ProcessType  ParseCmdLineToDic(string [] args)
        { 
            ProcessType currentType = ProcessType._;
            foreach (var item in args)
            {
                if (item.StartsWith(ParamMarker.ToString()))
                {
                    currentType =  Gdp.Utils.EnumUtil.TryParse(item.TrimStart(ParamMarker), ProcessType._);
                    if(currentType != ProcessType._)
                       return currentType; 
                } 
            }
            return ProcessType.H;
        }
          
         
        #endregion

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static CmdParam ParseParams(string[] args)
        {
            //参数解析
            CmdParam cmdParam = null;
            if (args != null && args.Length > 0)
            {
                //test
                //foreach (var item in args)
                //{
                //    Console.WriteLine(item);
                //}

                cmdParam = CmdParam.ParseArgs(args);
           //     log.Info("参数解析成功！" + cmdParam.ToString());
            } 
            return cmdParam;
        } 
    }
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum ProcessType
    {
        _,
        /// <summary>
        /// Help
        /// </summary>
        H,
        //Version
        V,
        /// <summary>
        ///Build Urls 
        /// </summary>
        B,
        /// <summary>
        /// download
        /// </summary>
        D,
        /// <summary>
        /// formate
        /// </summary>
        F,
        /// <summary>
        /// select filter
        /// </summary>
        S,
        /// <summary>
        /// View One table
        /// </summary>
        C,
        /// <summary>
        /// Extract Site info
        /// </summary>
        E
    }
     
}
