//2014.10.29, czs, create in numu, 包含一个Stream属性，用于输入输出。

using System;
using System.IO;
using System.Text;

namespace Gdp.IO
{
    /// <summary>
    /// 包含Stream, Encoding属性，用于输入输出。
    /// </summary>
    public abstract class AbstractEncodedStreamer : AbstractStreamer
    {
        public AbstractEncodedStreamer()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="encoding">编码</param>
        /// <param name="name">名称</param>
        public AbstractEncodedStreamer(Stream stream, Encoding encoding, string name):base(stream, name)
        {
            if (encoding == null) { encoding = Encoding.Default; }
            this.Encoding = encoding;
        }
        /// <summary>
        /// 字符编码
        /// </summary>
        public Encoding Encoding { get; set; }
    }
}
