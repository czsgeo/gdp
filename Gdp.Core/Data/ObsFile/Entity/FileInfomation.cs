//2015.10.15, czs, create in 西安五路口袁记肉夹馍店, 提取文件信息类。

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileInfomation
    {
        #region Creatoin Info
        /// <summary>
        /// 路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get => Path.GetFileName(FilePath); }
        /// <summary>
        /// 创建该文档的程序
        /// </summary>
        public string CreationProgram { get; set; }
        /// <summary>
        /// 创建该文件的机构
        /// </summary>
        public string CreationAgence { get; set; }
        /// <summary>
        /// 创建该文件的日期
        /// </summary>
        public string CreationDate { get; set; }
        #endregion
    }
     
}
