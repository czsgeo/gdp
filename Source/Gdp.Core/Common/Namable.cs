//2014.05.24，czs, created
//2018.11.12, csz, edit in hmx, 取消set

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 名称接口。具有名称属性。
    /// </summary>
    public interface Namable
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
    }
}