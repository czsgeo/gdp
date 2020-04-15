//2014.10.06， czs, create in hailutu, 配置通用接口
//2015.05.11， czs, edit in namu, 增加list构造函数

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{ 
    /// <summary>
    /// 文件配置，需要至少提供一个路径。
    /// </summary>
    public class FileOption : BaseOption, IFileOption
    {
        /// <summary>
        /// 构造函数。输入名称。
        /// </summary>
        /// <param name="FilePath">单文件</param>
        /// <param name="name">名称</param>
        public FileOption(string FilePath, string name = "单文件配置")
            : this(new List<string>() { FilePath }, name) { }

        /// <summary>
        /// 构造函数。输入名称。顺序与输出一致。
        /// </summary>
        /// <param name="FilePathes">集合文件</param>
        /// <param name="name">名称</param>
        public FileOption(string[] FilePathes, string name = "集合文件配置")
            : this(new List<string>(FilePathes), name) { }

        /// <summary>
        /// 构造函数。输入名称。顺序与输出一致。
        /// </summary>
        /// <param name="FilePathes">集合文件</param>
        /// <param name="name">名称</param>
        public FileOption(List<string> FilePathes, string name = "集合文件配置")
            : base(name)
        {
            this.FilePathes = FilePathes;
        }

        /// <summary>
        /// 文件路径集合
        /// </summary>
        public List<String> FilePathes { get; set; }
        /// <summary>
        /// 第一个文件路径
        /// </summary>
        public string FilePath { get { return FilePathes[0]; } }
    }



}