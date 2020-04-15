//2014.10.06， czs, create in hailutu, 配置通用接口

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{ 
    /// <summary>
    /// 配置通用接口。
    /// 配置无处不在，一般具有：
    /// 1.数据源/文件；
    /// 2.配置文件;
    /// 3.执行程序
    /// 就可以得到产品/文件。
    /// 而一般程序会提供一个默认配置文件，则可以省略之。
    /// </summary>
    public class BaseOption : Named, IOption
    {
        /// <summary>
        /// 构造函数。输入名称。
        /// </summary>
        /// <param name="name"></param>
        public BaseOption(string name="配置") :base(name)
        { }
    }
}