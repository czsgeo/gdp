//2014.10.29, czs, create in numu, 包含一个Stream属性，用于输入输出。

using System;
using System.IO;

namespace Gdp.IO
{
    /// <summary>
    /// 包含一个Stream属性，用于输入输出。
    /// </summary>
    public interface IStreamer: IDisposable
    {
        /// <summary>
        /// 数据流。
        /// </summary>
        Stream Stream { get; set; }
        /// <summary>
        /// 将清除该流的所有缓冲区，并使得所有缓冲数据被写入到基础设备。
        /// </summary>
        void Flush();
        /// <summary>
        /// 关闭当前流并释放与之关联的所有资源（如套接字和文件句柄）。
        /// </summary>
        void Close();
    }
}
