//2014.10.29, czs, create in numu, 包含一个Stream属性，用于输入输出。

using System;
using System.IO;
using System.Text;

namespace Gdp.IO
{
    /// <summary>
    /// 包含Stream, Encoding属性，用于输入输出。
    /// </summary>
    public abstract class AbstractWriter<TProduct> : AbstractEncodedStreamer//, IWriter<TProduct>
    {
        /// <summary>
        /// 默认构造函数，什么都不做
        /// </summary>
        public AbstractWriter()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">数据流</param>
        /// <param name="encoding">编码</param>
        public AbstractWriter(string path, Encoding encoding) :
            this(Gdp.Utils.FileUtil.CheckOrCreateDirectory(Path.GetDirectoryName(path)) ? new FileStream(path, FileMode.Create, FileAccess.Write) : new FileStream(path, FileMode.Create, FileAccess.Write), encoding, Path.GetFileName(path))
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="encoding">编码</param>
        /// <param name="name">编码</param>
        public AbstractWriter(Stream stream, Encoding encoding, string name = "Stream") :
            base(stream, encoding, name)
        {
            Init();
        }
        /// <summary>
        /// 初始化，在构造函数之后执行
        /// </summary>
        protected virtual void Init()
        {
            this.StreamWriter = new StreamWriter(Stream, this.Encoding);
        }

        /// <summary>
        /// 写
        /// </summary>
        public StreamWriter StreamWriter { get; set; }
        /// <summary>
        /// 写入一个到数据流。
        /// </summary>
        /// <returns></returns>
        public abstract void Write(TProduct product);
    }
}
