//2018.07.10, czs, create in hmx, 多项式拟合生成器
//2018.07.13, czs, edit in HMX, 改进中


using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using Gdp.IO;

namespace Gdp
{
    
    /// <summary>
    /// 在窗口的位置
    /// </summary>
    public enum PositionOfMovingWindow
    {
        /// <summary>
        /// 在前置外面
        /// </summary>
        FormerOutside = 0,
        /// <summary>
        /// 前面缓冲边界中
        /// </summary>
        FormerMargin = 1,
        /// <summary>
        /// 前面重叠
        /// </summary>
        FormerOverlap = 3,
        /// <summary>
        /// 在内部
        /// </summary>
        Inside = 4,
        /// <summary>
        /// 后面重叠
        /// </summary>
        LatterOverlap = 5,
        /// <summary>
        /// 后面缓冲边界中
        /// </summary>
        LatterMargin = 6,
        /// <summary>
        /// 在后置外面
        /// </summary>
        LatterOutside = 7,
    }
}