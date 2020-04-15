//2014.09.27, czs, create, 具有权值 Rms 属性。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 具有权值 Rms 属性。
    /// </summary>
    public  interface IRmsed <TValue> 
    {
        /// <summary>
        /// Rms 属性
        /// </summary>
        TValue Rms { get; set; }
    }
}
