//2015.10.26, create in K998 达州到西安南, 具有取消属性的接口

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 具有取消属性的接口
    /// </summary>
    public interface ICancelAbale
    {
        /// <summary>
        /// 指示，是否取消
        /// </summary>
        bool IsCancel { get; set; }
    }
}
