//2017.02.06, czs, create in hongqing, 表对象读取器。
//2018.09.30, czs, edit in hmx, 增加写入静态函数

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Threading;
 
using Gdp.Utils; 
using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 表对象读取器
    /// </summary>
    public class ObjectTableWriter : AbstractWriter<ObjectTableStorage>
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        public ObjectTableWriter(string path, Encoding encoding = null)
            : base(path, encoding)
        {
            this.FilePath = path;
            Spliters = new char[] { '\t' };


            this.OutputBufferCount = int.MaxValue;
            this.WrittingCount = 0;
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        public ObjectTableWriter(Stream path, Encoding encoding = null)
            : base(path, encoding)
        {
          //  this.FilePath = path.;
            Spliters = new char[] { '\t' };


            this.OutputBufferCount = int.MaxValue;
            this.WrittingCount = 0;
        }

        #region 数据流输出选项
        /// <summary>
        /// 统计写缓存的次数
        /// </summary>
        public int WrittingCount { get; protected set; }

        /// <summary>
        /// 采用缓存控制输出。
        /// </summary>
        public int OutputBufferCount { get; set; }
        #endregion

        /// <summary>
        /// 文件完整路径
        /// </summary>
        public string FilePath { get; set; }

        public bool IsJustWroteTitleRow { get; set; }
        /// <summary>
        /// 分割器
        /// </summary>
        public char[] Spliters { get; set; } 
        /// <summary>
        /// 初始化，在构造函数之后执行
        /// </summary>
        protected override void Init()
        {
            this.StreamWriter = new StreamWriter(Stream,this.Encoding);
        }
        /// <summary>
        /// 待写
        /// </summary>
        public ObjectTableStorage TableObjectStorage { get; set; }

        public override void Write(ObjectTableStorage product)
        {
            this.TableObjectStorage = product;
            this.WriteBufferToFile();
            this.CloseStream();
        }

        #region  写入文件
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (StreamWriter != null)
            {
                if (StreamWriter.BaseStream != null && StreamWriter.BaseStream.CanWrite)
                {
                    this.WriteToFileAndClearBuffer();
                    EndOfWriting(); //最后，写入标题行
                    this.CloseStream();
                    StreamWriter.Dispose();
                }
            }
        } 
        /// <summary>
        /// 检查缓存，如果超出最大缓存，则写入到文件并清空。
        /// </summary>
        private void CheckBufferOrWriteToFile()
        {
            if ( this.TableObjectStorage. BufferedValues.Count >= OutputBufferCount) { this.WriteToFileAndClearBuffer(); }
        }

        /// <summary>
        /// 如果写入次数多于1次，则写一行标题，结束。
        /// </summary>
        public void EndOfWriting()
        {
            if (WrittingCount > 1) { WriteTitleRow(); }
        }
        /// <summary>
        /// 追加到文件,并清空缓存。
        /// </summary> 
        public void WriteToFileAndClearBuffer()
        {
            WriteBufferToFile();

            TableObjectStorage.BufferedValues.Clear();
        }

        /// <summary>
        /// 写缓存到文件
        /// </summary>
        public void WriteBufferToFile()
        {
            CheckOrCreateFile();

            if (WrittingCount == 0)
            {
                WriteTitleRow();
            }

            this.IsJustWroteTitleRow = false;
            int count = TableObjectStorage.RowCount;
            for (int i = 0; i < count; i++)
            {
                var item = TableObjectStorage.BufferedValues[i];
                StreamWriter.WriteLine(ToSplitedValueString(item));                
            }
            //foreach (var key in TableObjectStorage.BufferedValues)
            //{
            //    StreamWriter.WriteLine(ToSplitedValueString(key));
            //}
            StreamWriter.Flush();
            WrittingCount++;
        }
        /// <summary>
        /// 检查输出文件的存在性，如果否，则创建文件。
        /// </summary>
        private void CheckOrCreateFile()
        {
            if (StreamWriter == null)
            {
                ///建立文件
                if (!String.IsNullOrWhiteSpace(FilePath))
                {
                    Gdp.Utils.FileUtil.CheckOrCreateDirectory(System.IO.Path.GetDirectoryName(FilePath));
                    StreamWriter = new StreamWriter(FilePath, false, Encoding.Default);
                }
            }
        }

        /// <summary>
        /// 关闭数据流
        /// </summary>
        public void CloseStream()
        {
            StreamWriter.Close();
        }

        #region 实现细节
        /// <summary>
        /// 写头文件
        /// </summary>
        public void WriteTitleRow()
        {
            if (!IsJustWroteTitleRow)
            {
                this.CheckOrCreateFile();
                StreamWriter.WriteLine(this.TableObjectStorage. ToSplitedTitleString()); StreamWriter.Flush();
                IsJustWroteTitleRow = true;
            }
        }


        /// <summary>
        ///  输出为行
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public String ToSplitedValueString(Dictionary<string, double> values)
        {
            var vals = new Dictionary<string, Object>();
            foreach (var item in values)
            {
                vals[item.Key.ToString()] = item.Value;
            }

            return ToSplitedValueString(vals);
        }

        /// <summary>
        /// 制表符数值
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isTotal"></param>
        /// <returns></returns>
        public String ToSplitedValueString(Dictionary<string, Object> values, String defaultValue = " ", string Spliter = "\t", string floatFormat = "G10", string floatValFormat = "0.00000", bool isTotal = true)
        {
           this.TableObjectStorage. Regist(values.Keys);

            var paramNames = this.TableObjectStorage.GetTitleNames(isTotal);

            return ToSplitedValueString(values, defaultValue, Spliter, floatFormat, paramNames);
        }

        /// <summary>
        /// 终极文本转换方法。
        /// </summary>
        /// <param name="values"></param>
        /// <param name="defaultValue"></param>
        /// <param name="Spliter"></param>
        /// <param name="floatFormat"></param>
        /// <param name="paramNames"></param>
        /// <returns></returns>
        public static string ToSplitedValueString(Dictionary<string, Object> values, String defaultValue, string Spliter, string floatFormat, List<string> paramNames)
        {
            //首先进行注册
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in paramNames)
            {
                if (i != 0) { sb.Append(Spliter); }
                if (values.ContainsKey(item))
                {
                    var val = values[item];
                    string formatedText = val + "";
                    if (Gdp.Utils.ObjectUtil.IsFloat(val))
                    {
                        var floatVal = (double)val;
                        formatedText = floatVal.ToString(floatFormat);// Geo.Utils.StringUtil.GetForamtedString(floatVal, 13, floatFormat);
                        //此处规定浮点数必须有小数点，除了科学计数法，如 1E20
                        if (!formatedText.Contains(".") && !formatedText.Contains("E") && formatedText!=("NaN")) { formatedText += ".0"; }
                    }else  if ( val is DateTime )
                    {
                        var DateTime = (DateTime)val;
                        formatedText = Gdp.Utils.DateTimeUtil.GetFormatedDateTime( DateTime );// Geo.Utils.StringUtil.GetForamtedString(floatVal, 13, floatFormat);
                    }
                    sb.Append(formatedText);
                }
                else
                {
                    sb.Append(defaultValue);
                }
                i++;
            }
            return sb.ToString();
        }


        #endregion
        #endregion

        /// <summary>
        /// 写入后就释放资源
        /// </summary>
        /// <param name="table"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        public static void Write(ObjectTableStorage table, string path, Encoding encoding = null)
        {
        using (ObjectTableWriter writer = new ObjectTableWriter(path, encoding))
            {
                writer.Write(table);
            }
        }
        /// <summary>
        /// 写入后就释放资源
        /// </summary>
        /// <param name="table"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        public static void Write(ObjectTableStorage table, Stream path, Encoding encoding = null)
        {
        using (ObjectTableWriter writer = new ObjectTableWriter(path, encoding))
            {
                writer.Write(table);
            }
        }

    }
}