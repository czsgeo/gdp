//2014.10.29, czs, create in numu, 包含一个Stream属性，用于输入输出。

using System;
using System.IO;

namespace Gdp.IO
{
    /// <summary>
    /// 包含一个Stream属性，用于输入输出。
    /// </summary>
    public abstract class AbstractStreamer: IStreamer
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AbstractStreamer()
        { 
            this.Name = "Stream";
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="name">数据流</param>
        public AbstractStreamer(Stream stream,string name="NoName")
        {
            this.Name = name;
            this.Stream = stream; 
        }
        /// <summary>
        /// 数据流。
        /// </summary>
        public global::System.IO.Stream Stream { get; set; }
        /// <summary>
        /// 数据流名称或标识。通常为文件名。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 将清除该流的所有缓冲区，并使得所有缓冲数据被写入到基础设备。
        /// </summary>
        public void Flush()
        {
            Stream.Flush();
        }
        /// <summary>
        /// 关闭当前流并释放与之关联的所有资源（如套接字和文件句柄）。
        /// </summary>
        public void Close()
        {
            Stream.Close();
        }
        /// <summary>
        /// 关闭当前流并释放与之关联的所有资源
        /// </summary>
        public virtual void Dispose()
        {
            Stream.Dispose();
        }
         
    }
}
