//2016.03.22, czs, created in hongqing, 基于卫星结果的管理器 
//2016.03.29, czs, edit in hongqing, 名称修改为 NamedValueTableManager
//2016.05.09, czs, create in hongqing, 表数据管理器
//2016.08.05, czs, edit in fujian yongan, 重构
//2016.10.03, czs, eidt in hongqing, 增加实时输出选项，以减少内存消耗
//2017.03.08, czs, edit in hongqing, 增加了大量并行化数据表处理方法
//2019.02.20, czs, edit in hongqing, TableObjectManager 更名为 ObjectTableManager

using Gdp.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 表数据管理器。
    /// </summary>
    public class ObjectTableManager : BaseDictionary<string, ObjectTableStorage>
    {
        ILog log = new Log(typeof(ObjectTableManager));
        /// <summary>
        /// 采用字典数据直接初始化
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="OutputBufferCount">缓存数量，达到后将写入文件</param>
        /// <param name="directory">存储目录</param>
        /// <param name="Name">表名称</param>
        public ObjectTableManager(IDictionary<string, ObjectTableStorage> dic, int OutputBufferCount = 10000, string directory = null, string Name = "未命名")
            : this(OutputBufferCount, directory, Name)
        {
            this.SetData(dic);
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="OutputBufferCount">缓存数量，达到后将写入文件</param>
        /// <param name="directory">存储目录</param>
        /// <param name="Name">表名称</param>
        public ObjectTableManager(string directory, int OutputBufferCount = 10000, string Name = "未命名")
            : this(OutputBufferCount, directory, Name)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="OutputBufferCount">缓存数量，达到后将写入文件</param>
        /// <param name="directory">存储目录</param>
        /// <param name="Name">表名称</param>
        public ObjectTableManager(int OutputBufferCount = 10000, string directory = null, string Name = "未命名")//, bool IsRealTimeOutput)
        {
            this.OutputBufferCount = OutputBufferCount;
            this.Name = Name;
            if (directory == null)
            {
                directory = Setting.TempDirectory;
            }
            this.OutputDirectory = directory;
            if (String.IsNullOrWhiteSpace(directory)) { Gdp.Utils.FileUtil.CheckOrCreateDirectory(directory); }

            log.Debug("新建了表管理器 " + this.Name + " " + this.OutputDirectory);
        }

        #region 属性
        /// <summary>
        /// 后缀
        /// </summary>
        public static string TableExtention = Setting.TextTableFileExtension;// ".xls";
        /// <summary>
        /// 输出缓存数量大小
        /// </summary>
        public int OutputBufferCount { get; set; }
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputDirectory { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 检索器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override ObjectTableStorage this[string key]
        {
            get
            {
                var name = key;
                if (!this.Contains(key))
                {
                    name += TableExtention;
                }
                return Data[key];
            }
            set
            {
                this.Set(key, value);
            }
        }
        /// <summary>
        /// 逆序所有行
        /// </summary>
        public void ReverseAllRows()
        {
            Parallel.ForEach<ObjectTableStorage>(this, item =>
            {
                item.ReverseRows();
            });
        }


        /// <summary>
        /// 清空存储。
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            ClearParamNames();
        }
        /// <summary>
        /// 清空已经注册的名称列表
        /// </summary>
        public void ClearParamNames()
        {
            foreach (var item in this) { item.Clear(); }
        }
        /// <summary>
        /// 添加一个表，以表名为关键字。
        /// </summary>
        /// <param name="table"></param>
        public void Add(ObjectTableStorage table) { this.Add(table.Name, table); }
        /// <summary>
        /// 添加一个表。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public override void Add(string key, ObjectTableStorage val)
        {
            base.Add(key, val);
            if (String.IsNullOrWhiteSpace(val.Name))
            {
                val.Name = key;
            }
        }


        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override bool Contains(string tableName)
        {
            if (base.Contains(tableName)) { return true; }
            if (base.Contains(tableName + TableExtention)) return true;
            return false;
        }

        /// <summary>
        /// 创建一个表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override ObjectTableStorage Create(string tableName)
        {
            AddTable(tableName); return this[tableName];
        }
        /// <summary>
        /// 新建，添加一个数据表,自动设置存储路径。
        /// </summary>
        /// <param name="tableName">表名称，以此写为文件，如 Params.xls;如果没有xls后缀，系统将自动添加一个。</param>
        public ObjectTableStorage AddTable(string tableName)
        {
            var table = new ObjectTableStorage(tableName);
            Add(tableName, table);
            return table;
        }
        /// <summary>
        /// 添加表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        public void AddTable(string tableName, ObjectTableStorage table)
        {
            table.Name = tableName;
            Add(tableName, table);
        }
        /// <summary>
        /// 添加表
        /// </summary> 
        /// <param name="table"></param>
        public void AddTable(ObjectTableStorage table)
        {
            Add(table.Name, table);
        }
        /// <summary>
        /// 添加表
        /// </summary> 
        /// <param name="tables"></param>
        public void AddTable(IEnumerable<ObjectTableStorage> tables)
        {
            foreach (var table in tables)
            {
                Add(table);
            }
        }

        #region 写文件
        /// <summary>
        /// 写入AllInOne文件。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="headerMarker"></param>
        /// <param name="spliter"></param>
        public void WriteAsOneFile(string fileName = null, string headerMarker = "#", string spliter = "\t")
        {
            if (this.Count == 0) { return; }

            if (fileName == null) { fileName = this.Name + ".aio"; }
            if (!TableExtention.Equals(Path.GetExtension(fileName), StringComparison.CurrentCultureIgnoreCase))
            {
                fileName += TableExtention;
            }

            var path = Path.Combine(OutputDirectory, fileName);

            Stream stream = new FileStream(path, FileMode.Create);

            WriteAsOneFile(stream, headerMarker, spliter);
        }
        /// <summary>
        /// 写入AllInOne文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="headerMarker"></param>
        /// <param name="spliter"></param>
        /// <param name="encoding"></param>
        public void WriteAsOneFile(Stream stream, string headerMarker = "#", string spliter = "\t", Encoding encoding = null)
        {
            if (encoding == null) { encoding = Encoding.Default; }

        using (StreamWriter writer = new System.IO.StreamWriter(stream, encoding))
            {
                var length = GetMaxTableRowCount();
                for (int i = -1; i < length; i++)
                {
                    foreach (var table in this)
                    {
                        if (table.RowCount == 0) { continue; }

                        if (table.RowCount > i)
                        {
                            if (i == -1)
                            {
                                writer.WriteLine(headerMarker + table.Name + spliter + table.ToSplitedTitleString(spliter));
                                continue;
                            }
                            var row = table.BufferedValues[i];

                            writer.WriteLine(table.Name + spliter + ObjectTableWriter.ToSplitedValueString(row, "", spliter, "G", table.ParamNames));
                        }
                    }
                }
                writer.Flush();
            }
        }

        /// <summary>
        /// 获取最长表行
        /// </summary>
        /// <returns></returns>
        public int GetMaxTableRowCount()
        {
            int row = 0;
            foreach (var table in this)
            {
                if (table.RowCount > row) { row = table.RowCount; }
            }
            return row;
        }


        /// <summary>
        /// 采用默认文件名，写入并关闭数据流
        /// </summary>
        public void WriteAllToFileAndCloseStream()
        {
            Parallel.ForEach<string>(this.Keys, item =>
            {
                var storage = this[item];
                storage.Name = item;//名称避免不同
                WriteTable(storage);
            });
        }

        /// <summary>
        /// 采用默认文件名，写入,并清空缓存。
        /// </summary>
        /// <param name="exceptClear">不清空的一个</param>
        /// <param name="isOverWrite">是否覆盖</param>
        public void WriteAllToFileAndClearBuffer(ObjectTableStorage exceptClear = null, bool isOverWrite = true)
        {
            Parallel.ForEach<string>(this.Keys, item =>
            {
                var storage = this[item];
                storage.Name = item;//名称避免不同
                WriteTable(storage, isOverWrite);

                if (exceptClear != storage)
                {
                    storage.Clear();
                }
            });
        }

        /// <summary>
        /// 写一个表
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="isOverWrite">是否覆盖</param>
        public void WriteTable(ObjectTableStorage storage, bool isOverWrite = true)
        {
            if (storage.RowCount == 0) { log.Info(storage.Name + " 没有数据，不输出。"); return; }
            var directory = this.OutputDirectory;
            WriteTable(storage, directory, isOverWrite);
        }
        /// <summary>
        /// 写一个表
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="directory"></param>
        /// <param name="isOverWrite">是否覆盖</param>
        public static void WriteTable(ObjectTableStorage storage, string directory, bool isOverWrite = true)
        {
            var OutputPath = Path.Combine(directory, storage.Name);
            if (!OutputPath.EndsWith(TableExtention, StringComparison.CurrentCultureIgnoreCase))
            {
                OutputPath += TableExtention;
            }

            if (!isOverWrite && File.Exists(OutputPath))
            {
                storage.log.Warn("忽略了已存在文件 " + OutputPath);
                return;
            }

            var writer = new ObjectTableWriter(OutputPath);
            writer.Write(storage);
            writer.CloseStream();
        }
        #endregion


        #endregion


        /// <summary>
        /// 获取表格，如果匹配失败，则分割后继续比较，分隔符在 TableNameHelper 中指定，通常为下划线“_”
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override ObjectTableStorage Get(string key)
        {
            var table = base.Get(key);
            if (table == null)
            {
                foreach (var item in this.Data)
                {
                    if (TableNameHelper.IsPrefixEquals(item.Key, key)) { return item.Value; }
                }
            }
            return table;
        }
        /// <summary>
        /// 获取包含所有表行的Index列。
        /// </summary>
        /// <returns></returns>
        public List<Object> GetIndexesOfAllTables() { return GetIndexesOfAllTables<object>(); }

        /// <summary>
        /// 获取包含所有表行的Index列。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetIndexesOfAllTables<T>()
        {
            var indexName = this.First.GetIndexColName();
            List<T> indexes = new List<T>();
            int i = -1;
            foreach (var item in this.Data)
            {
                i++;

                var oneIndexe = item.Value.GetIndexValues<T>();
                if (i == 0)
                {
                    indexes.AddRange(oneIndexe);
                }

                indexes = Gdp.Utils.ListUtil.Emerge<T>(indexes, oneIndexe);
            }
            indexes.Sort();
            return indexes;
        }

        /// <summary>
        /// 获取包含所有表列名称。
        /// </summary> 
        /// <returns></returns>
        public List<string> GetColNamesOfAllTables()
        {
            var indexName = this.First.GetIndexColName();
            List<string> names = new List<string>();
            int i = -1;
            foreach (var item in this.Data)
            {
                i++;

                var oneNames = item.Value.ParamNames;
                if (i == 0)
                {
                    names.AddRange(oneNames);
                }

                names = Gdp.Utils.ListUtil.Emerge<String>(names, oneNames);
            }
            // names.Sort();
            return names;
        }

        /// <summary>
        /// 读取返回表集合。移除下划线以后的字符串。
        /// </summary>
        /// <param name="inputPathes"></param>
        /// <param name="nameSplitter"></param>
        /// <returns></returns>
        public static ObjectTableManager Read(string[] inputPathes, string nameSplitter = "_")
        {
            new Log(typeof(ObjectTableManager)).Info("准备并行读取 " + inputPathes.Length + " 个文件！");
            DateTime start = DateTime.Now;
            var data = new ConcurrentDictionary<string, ObjectTableStorage>();
            Parallel.ForEach(inputPathes, (path) =>
            {
                if (!File.Exists(path)) { return; }

                ObjectTableReader reader = new ObjectTableReader(path);

                var storage = reader.Read();
                var key = Path.GetFileNameWithoutExtension(path);
                var fistDivChar = key.LastIndexOf(nameSplitter);
                var name = key;
                if (fistDivChar != -1)
                {
                    name = name.Substring(0, fistDivChar);
                }
                storage.Name = name;
                data.TryAdd(storage.Name, storage);

            });


            var tableManager = new ObjectTableManager(data);
            tableManager.OutputDirectory = Path.GetDirectoryName(inputPathes[0]);

            var span = DateTime.Now - start;
            new Log(typeof(ObjectTableManager)).Info("并行读取了" + tableManager.Count + " 个文件，耗时：" + span);

            return tableManager;
        }

    }


    /// <summary>
    /// 表名称帮助器
    /// </summary>
    public class TableNameHelper
    {
        /// <summary>
        /// 表名称分割
        /// </summary>
        static public string TableSpliter = "_";

        /// <summary>
        /// 两个名称的前缀是否相等。
        /// </summary>
        /// <param name="nameA"></param>
        /// <param name="nameB"></param>
        /// <param name="StringComparison"></param>
        /// <returns></returns>
        public static bool IsPrefixEquals(string nameA, string nameB, StringComparison StringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            var a = ParseName(nameA);
            var b = ParseName(nameB);
            return a.Equals(b, StringComparison);
        }

        /// <summary>
        /// 解析名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isTrimSpliter"></param>
        /// <returns></returns>
        public static string ParseName(string name, bool isTrimSpliter = true)
        {
            if (!isTrimSpliter) { return name; }

            int indexOfSpliter = name.IndexOf(TableSpliter);
            if (indexOfSpliter == -1) { return name; }
            return name.Substring(0, indexOfSpliter);
        }

        /// <summary>
        /// 构建新名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namePostfix"></param>
        /// <param name="isReplacePrevPosfix"></param>
        /// <returns></returns>
        public static string BuildName(string name, string namePostfix, bool isReplacePrevPosfix = true)
        {
            var oldName = name;
            if (isReplacePrevPosfix)
            {
                oldName = ParseName(name);
            }
            if (!String.IsNullOrWhiteSpace(namePostfix))
            {
                return oldName + TableSpliter + namePostfix;
            }
            return oldName;
        }
    }
}