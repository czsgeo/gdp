//2014.10.29, czs, create in numu, 包含一个Stream属性，用于输入输出。

using System;
using System.IO;
using System.Text;

namespace Gdp.IO
{
    /// <summary>
    /// 包含Stream, Encoding属性，用于输入输出。
    /// 适用于整存争取的小文件。
    /// </summary>
    public abstract class AbstractReader<TProduct> : AbstractEncodedStreamer
    {
        /// <summary>
        /// 本地文件读取
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        public AbstractReader(string path, Encoding encoding = null)
            : this(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), encoding, Path.GetFileName(path))
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="encoding">编码</param>
        /// <param name="name">编码</param>
        public AbstractReader(Stream stream, Encoding encoding, string name = "Stream") :
        base(stream, encoding, name)
        {

        }
        /// <summary>
        /// 读取一个到数据流。
        /// </summary>
        /// <returns></returns>
        public abstract TProduct Read();
    }
    /// <summary>
    /// 包含Stream, Encoding属性，用于输入输出。
    /// </summary>
    public abstract class AbstractReader<TProduct, TOption> : AbstractEncodedStreamer
    {
        /// <summary>
        /// 本地文件读取
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        public AbstractReader(string path, Encoding encoding)
            : this(new FileStream(path, FileMode.Open), encoding, Path.GetFileName(path))
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="encoding">编码</param>
        /// <param name="name">编码</param>
        public AbstractReader(Stream stream, Encoding encoding, string name) :
        base(stream, encoding, name)
        {

        }
        /// <summary>
        /// 读取一个到数据流。
        /// </summary>
        /// <returns></returns>
        public abstract TProduct Read(TOption option);
    }
}
