//2014.10.24， czs, create in namu, 通用文件配置接口

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 

namespace Gdp
{ 
    /// <summary>
    /// 配置通用接口。
    /// 配置无处不在，通常具有：
    /// 1.数据源/文件；
    /// 2.配置文件;
    /// 3.执行程序
    /// 就可以得到产品/文件。
    /// 而一般程序会提供一个默认配置文件，则可以直接运行，然后输出结果。
    /// </summary>
    public interface IFileOption: IOption
    {
        /// <summary>
        /// 文件路径。
        /// </summary>
        string FilePath { get;}
        /// <summary>
        /// 文件路径集合
        /// </summary>
        List<String> FilePathes { get; set; }
    }
}