//2016.10.27, czs,  create in hongqing, 数据处理流程控制命令类型

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 数据处理流程控制命令类型
    /// </summary>
    public enum ProcessCommandType
    {
        /// <summary>
        /// 取消
        /// </summary>
        Cancel,
        /// <summary>
        /// 运行
        /// </summary>
        Run,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
    }
}
