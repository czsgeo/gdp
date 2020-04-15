//2016.03.22, czs, created in hongqing, 基于卫星结果的管理器 
//2016.03.29, czs, edit in hongqing, 名称修改为 NamedValueTableManager
//2016.08.05, czs, edit in fujian yongan, 重构
//2016.10.03, czs, edit in hongqing,增加缓存结果数量控制，便于控制内存大小
//2016.10.19, czs, edit in hongqing, 增加一些计算分析功能
//2016.10.26, czs, edit in hongqing, 表格值从字符串修改为 Object，减少转换损失
//2016.10.28, czs, edit in hongqing, 去掉了与输出相关的全局设置，后续可考虑输出单独分离成一个类。更名为TableObjectStorage
//2017.02.08, czs, edit in hongqing, 增加检索列的名称，增加列数据相减等方法
//2017.07.07, czs, edit in HMX, 增加一些统计分析功能
//2019.02.20, czs, edit in hongqing, TableObjectStorage 更名为 TableObjectStorage

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading; 
using Gdp.Utils; 
using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 参数数值表格管理器，用于表格化输出参数。适合用于存储稀疏表格数据，可以做适当的数据统计和分析。
    /// 当达到指定缓存大小后，将写入文件。而文件最后一行为全部的参数。
    /// </summary>
    public class ObjectTableStorage : IDisposable
    {
        public Log log = new Log(typeof(ObjectTableStorage));
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称，也用于文件识别和存储</param>
        public ObjectTableStorage(string name = "未命名表")
        {
            if (name == "未命名表")
            {
                int a = 0;
            }


            // this.OutputPath = outputPath;
            this.Name = name;
            this.NameListManager = new List<string>();
            this.BufferedValues = new List<Dictionary<string, Object>>();
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        public ObjectTableStorage(ObjectTableStorage table, string name = "未命名表")
        {
            this.Name = name;
            this.NameListManager = new List<string>();
            this.BufferedValues = new List<Dictionary<string, Object>>();
            foreach (var item in table.BufferedValues)
            {
                this.NewRow();
                this.AddItem(item);
            }
        }

        /// <summary>
        /// 采用系统表初始化
        /// </summary>
        /// <param name="table"></param>
        public ObjectTableStorage(DataTable table)
        {
            this.NameListManager = new List<string>();
            this.BufferedValues = new List<Dictionary<string, Object>>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                this.NewRow();
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    this.AddItem(table.Columns[j].ColumnName, table.Rows[i][j]);
                }
                this.EndRow();
            }
        }

        #region 属性
        /// <summary>
        /// 第一个检索
        /// </summary>
        public Object FirstIndex { get { return FirstRow[GetIndexColName()]; } }
        /// <summary>
        /// 第一个检索
        /// </summary>
        public Object SecondIndex { get { return SecondRow[GetIndexColName()]; } }
        /// <summary>
        /// 最后一个检索
        /// </summary>
        public Object LastIndex { get { return LastRow[GetIndexColName()]; } }
        /// <summary>
        /// 第一行
        /// </summary>
        public Dictionary<string, Object> FirstRow { get { return this.BufferedValues[0]; } }
        /// <summary>
        /// 第2行
        /// </summary>
        public Dictionary<string, Object> SecondRow { get { return this.BufferedValues[1]; } }
        /// <summary>
        /// 最后一行
        /// </summary>
        public Dictionary<string, Object> LastRow { get { return this.BufferedValues[RowCount - 1]; } }
        string _name = null;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name == null)
                {
                    _name = value;
                }
                else if (_name != value)
                {
                    _name = value;
                }
            }
        }
        /// <summary>
        /// 没有后缀名的名称
        /// </summary>
        public string NoExtensionName { get { return Path.GetFileNameWithoutExtension(Name); } }

        string _IndexColName { get; set; }
        /// <summary>
        /// 检索列名称。不返回 null
        /// </summary>
        public string IndexColName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_IndexColName))
                {
                    _IndexColName = this.ParamNames[0];
                }
                return _IndexColName;
            }
            set { _IndexColName = value; }
        }
        /// <summary>
        /// 获取检索列名称，如果没有指定，则采用第一列的名称。
        /// </summary>
        public string GetIndexColName()
        {
            if (String.IsNullOrEmpty(IndexColName))
            {
                if (this.ParamNames.Count == 0)
                {
                    return null;
                }
                return this.ParamNames[0];
            }
            return IndexColName;
        }
        /// <summary>
        /// 参数名称顺序
        /// </summary>
        public List<string> ParamNames { get { return NameListManager; } } 

        #region 核心存储
        /// <summary>
        /// 名称列表管理器
        /// </summary>
        public List<string> NameListManager { get; set; }
        /// <summary>
        /// 缓存的数据,核心存储
        /// </summary>
        public List<Dictionary<string, Object>> BufferedValues { get; set; }
        /// <summary>
        /// 当前行,保存在数据中的最新行。具体位置决定于采用的方法，如 Add or Insert 
        /// </summary>
        public Dictionary<string, Object> CurrentRow { get; set; }
        #endregion

        /// <summary>
        /// 参数数量。列数。
        /// </summary>
        public int ColCount { get { return this.ParamNames.Count; } }
        /// <summary>
        /// 行数。
        /// </summary>
        public int RowCount { get { return this.BufferedValues.Count; } }
        /// <summary>
        /// 最后一行的编号
        /// </summary>
        public int IndexOfLastRow { get { return RowCount - 1; } }
        #endregion

        #region 方法
        #region  索引
        /// <summary>
        /// 索引
        /// </summary>
        public Dictionary<string, Dictionary<object, object>> Indexes { get; set; }
        /// <summary>
        /// 是否建立了索引。
        /// </summary>
        public bool HasIndexes { get { return Indexes != null && Indexes.Count > 0; } }
        /// <summary>
        /// 手动建立索引
        /// </summary>
        public void BuildIndexes()
        {
            Indexes = new Dictionary<string, Dictionary<object, object>>();
            foreach (var name in this.ParamNames)
            {
                Indexes.Add(name, GetColObjectDic(name));
            }
        }

        /// <summary>
        /// 返回单元格数据，若无返回null,默认以第一个
        /// </summary>
        /// <param name="indexObject"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public object this[object indexObject, string colName]
        {
            get
            {
                var col = this[colName];

                if (col != null && col.ContainsKey(indexObject))
                {
                    return col[indexObject];
                }
                return null;
            }
        }

        /// <summary>
        ///获取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="indexObject"></param>
        /// <param name="colName"></param>
        /// <param name="defaultValue">失败后，默认返回。</param>
        /// <returns></returns>
        public TValue GetValue<TValue>(object indexObject, string colName, TValue defaultValue = default(TValue))
        {
            var col = this[colName];

            if (col != null && col.ContainsKey(indexObject) && col[indexObject] != null)
            {
                var obj = col[indexObject];
                if (obj is TValue)
                {
                    return (TValue)obj;
                }
                return defaultValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// 返回指定列的数据
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        private Dictionary<object, object> this[string colName]
        {
            get
            {
                if (!this.ParamNames.Contains(colName)) { return null; }
                if (!HasIndexes) { BuildIndexes(); }
                var col = Indexes[colName];
                return col;
            }
        }


        #endregion 
        #region 排序，转置 

        /// <summary>
        /// 反转行
        /// </summary>
        public void ReverseRows() { this.BufferedValues.Reverse(); }
        /// <summary>
        /// 排序参数名称
        /// </summary>
        public void SortColumns() { this.ParamNames.Sort(); }

        #endregion

        /// <summary>
        /// 获取指定列的类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type GetType(string name)
        {
            foreach (var dic in this.BufferedValues)
            {
                foreach (var item in dic)
                {
                    if (String.Equals(name, item.Key, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.Value != null)
                        {
                            return item.Value.GetType();
                        }
                    }
                }

            }
            return typeof(String);
        }

        #region 添加数据
        /// <summary>
        /// 新建一个行，并追加到末尾，如果当前行尚未写值，则不新建。
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Object> NewRow()
        {
            if (CurrentRow == null || CurrentRow.Count > 0)
            {
                CurrentRow = new Dictionary<string, Object>();
                BufferedValues.Add(CurrentRow);
            }
            return CurrentRow;
        } 
        #region 添加字典数据结构 
        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="dic"></param>
        public void AddItem(Dictionary<string, object> dic) { foreach (var item in dic) { this.AddItem(item.Key, item.Value); } }
 
        /// <summary>
        /// 结束行，若当前行没有数据，则删除之。
        /// </summary> 
        public void EndRow()
        {
            if (CurrentRow.Count == 0)
            {
                //  CurrentRow = new Dictionary<string, Object>();
                BufferedValues.Remove(CurrentRow);
                CurrentRow = null;
            }
        } 
        /// <summary>
        /// 添加一个项目
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        public void AddItem(object name, object val) { AddItem(name + "", val); }
        /// <summary>
        /// 添加一个项目
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        public void AddItem(string name, object val) { this.Regist(name); if (!CurrentRow.ContainsKey(name)) CurrentRow.Add(name, val); }

        #endregion
        #endregion

        #region 注册名称
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="names"></param>
        public void Regist(IEnumerable<string> names) { foreach (var item in names) { Regist(item); } }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="colName"></param>
        public void Regist(string colName) { if (!NameListManager.Contains(colName)) { NameListManager.Add(colName); } }

        #endregion


        #region 计算、转换、统计分析 

        #region 提取新表

        /// <summary>
        /// 获取列对象字典，key为索引，Value为值。
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="fromIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Dictionary<object, object> GetColObjectDic(string colName, int fromIndex = 0, int count = int.MaxValue)
        {
            Dictionary<object, object> dic = new Dictionary<object, object>();
            var indexColName = this.GetIndexColName();
            int endIndex = fromIndex + count;
            //endIndex = endIndex < this.RowCount ? endIndex : this.RowCount - 1;
            //lly修改，以前是this.RowCount - 1
            endIndex = endIndex < this.RowCount ? endIndex : this.RowCount;
            for (int i = fromIndex; i < endIndex; i++)
            {
                var item = this.BufferedValues[i];
                if (item.ContainsKey(colName))
                {
                    var key = item[indexColName];
                    var val = item[colName];
                    dic[key] = val;
                }
            }
            return dic;
        }


        /// <summary>
        /// 获取指定行
        /// </summary>
        /// <param name="index"></param> 
        /// <returns></returns>
        public Dictionary<string, object> GetRow(int index)
        {
            return this.BufferedValues[index];
        }

        #endregion


        #region 列的统计

        /// <summary>
        /// 获取列区域内最大的值
        /// </summary>
        /// <typeparam name="TIndexType"></typeparam>
        /// <param name="colName"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double GetMaxValue<TIndexType>(string colName, TIndexType start, TIndexType end)
        {
            var rowFrom = this.GetRowIndexOfIndexCol(start);
            var rowTo = this.GetRowIndexOfIndexCol(end);

            return GetMaxValue(colName, rowFrom, rowTo);
        }
         

        /// <summary>
        /// 获取索引的行号编号，从0开始。
        /// </summary>
        /// <param name="indexValue"></param>
        /// <returns></returns>
        public int GetRowIndexOfIndexCol(object indexValue)
        {
            var indexs = GetIndexValues();
            return indexs.IndexOf(indexValue);
        }
           

        /// <summary>
        /// 统计指定列有效数据（行）个数。
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public int GetValidRowCount(string colName)
        {
            return GetVector(colName, 0, this.RowCount, true).Count;
        }

        /// <summary>
        /// 获取指定列最后不为空的值。
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public Object GetLastValue(string colName)
        {
            int rowCount = this.RowCount;
            for (int i = rowCount - 1; i >= 0; i--)
            {
                var row = this.BufferedValues[i];
                if (row.ContainsKey(colName) && row[colName] != null) { return row[colName]; }
            }
            return null;
        }

        #endregion

        #region 提取向量

        /// <summary>
        /// 返回列数据，变量数组.如果无法解析，则返回为空数组。
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="from">起始编号</param>
        /// <param name="count">截取数量</param>
        /// <param name="defaultValue">默认数值，如果无效数值，则解析失败后以其填充</param>
        /// <param name="isSkipNonNumerial">是否忽略非浮点数数据</param>
        /// <returns></returns>
        public List<double> GetVector(string paramName, int from = 0, int count = int.MaxValue, bool isSkipNonNumerial = false, double defaultValue = Double.NaN)
        {
            if (!this.ParamNames.Contains(paramName)) { return new List<double>(); }
            List<double> Vector = new List<double>(this.BufferedValues.Count);

            //表内容 
            int i = -1;
            int endIndex = from + count;
            foreach (var values in BufferedValues.ToArray())
            {
                i++;
                if (i < from) { continue; }
                if (i >= endIndex) { break; }

                if (values.ContainsKey(paramName))
                {
                    var obj = values[paramName];
                    if (Gdp.Utils.ObjectUtil.IsNumerial(obj))
                    {
                        var valu = Gdp.Utils.ObjectUtil.GetNumeral(obj);
                        if (Gdp.Utils.DoubleUtil.IsValid(valu))
                        {
                            Vector.Add(valu);
                        }
                    }
                    else if (obj is RmsedNumeral)
                    {
                        var val = (RmsedNumeral)obj;
                        Vector.Add(val.Value);
                    }
                    else if (!isSkipNonNumerial)
                    {
                        Vector.Add(defaultValue);
                    }
                }
                else if (!isSkipNonNumerial)
                {
                    Vector.Add(defaultValue);
                }
            }
            return Vector;
        }
        #endregion
        #endregion

        #region 转换输出，外部显示

        /// <summary>
        /// 文本显示
        /// </summary>
        /// <returns></returns>
        public string ToTextTable()
        {
            StringBuilder writer = new StringBuilder();
            writer.AppendLine(ToSplitedTitleString());
            foreach (var item in BufferedValues)
            {
                writer.AppendLine(ToSplitedValueString(item));
            }
            return writer.ToString();
        }


        /// <summary>
        /// 制表位分隔的题目
        /// </summary>
        /// <returns></returns>
        public String ToSplitedTitleString(string Spliter = "\t", bool isTotal = true)
        {
            var paramNames = GetTitleNames(isTotal);
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in paramNames)
            {
                if (i != 0) { sb.Append(Spliter); }
                sb.Append(item);
                i++;
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取文本表格
        /// </summary>
        /// <returns></returns>
        public string GetTextTable(string spliter = "\t", string defaultStr = " ", string NumeralValFormat = "0.00000")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ToSplitedTitleString(spliter));
            foreach (var item in BufferedValues.ToArray()) { sb.AppendLine(ToSplitedValueString(item, defaultStr, spliter, NumeralValFormat)); }
            return sb.ToString();
        }
        /// <summary>
        /// 命名的类型集合。
        /// </summary>
        public List<NamedType> GetNamedTypes()
        {
            List<NamedType> nameTypes = new List<NamedType>();
            var paramNames = GetTitleNames();
            foreach (var item in paramNames)
            {
                var type = GetType(item);
                var namedType = new NamedType(item, type);
                nameTypes.Add(namedType);
            }
            return nameTypes;
        }
        /// <summary>
        /// 返回数据表格，用于显示。
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="isDefaultEmpty">默认是否是空</param>
        /// <param name="isTotal">是否所有</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName = "表格", bool isDefaultEmpty = false, bool isTotal = true)
        {
            DataTable table = new DataTable(tableName);
            //表名称
            var typedNames = GetNamedTypes();
            var paramNames = this.ParamNames;
            foreach (var item in typedNames) { table.Columns.Add(item.Name, item.Type); }
            //表内容
            foreach (var values in BufferedValues.ToArray())
            {
                int i = 0;
                object[] vals = new object[paramNames.Count];

                foreach (var name in typedNames)
                {
                    if (values.ContainsKey(name.Name)) { vals[i] = values[name.Name]; }
                    else if (isDefaultEmpty) { vals[i] = null; }
                    else { vals[i] = ObjectUtil.Default(name.Type); }
                    i++;
                }

                var row = table.Rows.Add(vals);
            }
            return table;
        }
        #endregion
         
        /// <summary>
        /// 制表符数值
        /// </summary>
        /// <param name="values"></param>
        /// <param name="defaultValue"></param>
        /// <param name="Spliter"></param>
        /// <param name="NumeralFormat"></param>
        /// <param name="NumeralValFormat"></param>
        /// <param name="isTotal"></param>
        /// <returns></returns>
        public String ToSplitedValueString(Dictionary<string, Object> values, String defaultValue = " ", string Spliter = "\t", string NumeralFormat = "0.00000", string NumeralValFormat = "0.00000", bool isTotal = true)
        {
            Regist(values.Keys);

            var paramNames = GetTitleNames(isTotal);

            return ObjectTableWriter.ToSplitedValueString(values, defaultValue, Spliter, NumeralFormat, paramNames);
        }

        /// <summary>
        /// 取得所有标题。
        /// </summary>
        /// <param name="isTotal"></param>
        /// <returns></returns>
        public List<string> GetTitleNames(bool isTotal = true)
        {
            var paramNames = isTotal ? NameListManager : ParamNames;
            return paramNames;
        }


        #region 移除或清空数据

        /// <summary>
        /// 移除指定列
        /// </summary>
        /// <param name="colNames"></param>
        public void RemoveCols(IEnumerable<string> colNames)
        {
            foreach (var name in colNames)
            {
                NameListManager.Remove(name);
            }
            foreach (var item in this.BufferedValues)
            {
                foreach (var name in colNames)
                {
                    item.Remove(name);
                }
            }
        }

        #endregion

        #region 通用接口方法
        /// <summary>
        /// 清空重新来过
        /// </summary>
        public void Clear() { this.ParamNames.Clear(); BufferedValues.Clear(); }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            NameListManager.Clear();
            this.BufferedValues.Clear();
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name + " , Row: " + RowCount + " × Col:" + ColCount;
        }

        #endregion

        #region  遍历并计算单元数据后，返回新表
        /// <summary>
        /// 获取检索列表
        /// </summary>
        /// <returns></returns>
        public List<Object> GetIndexValues(bool ifToDateTime = false)
        {
            string indexColName = GetIndexColName();
            List<Object> list = new List<object>();
            if (indexColName == null) { return list; }

            for (int i = 0; i < RowCount; i++)
            {
                var row = this.BufferedValues[i];
                if (!row.ContainsKey(indexColName)) { continue; }

                var index = row[indexColName];
                list.Add(index);
            }
            return list;
        }

        /// <summary>
        /// 获取检索列表
        /// </summary>
        /// <returns></returns>
        public List<T> GetIndexValues<T>()
        {
            return GetColValues<T>(GetIndexColName());
        }

        /// <summary>
        /// 获取列值，所无，则以默认值替代
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public List<T> GetColValues<T>(string colName, T defaultValue = default(T))
        {
            List<T> list = new List<T>();
            foreach (var row in this.BufferedValues)
            {
                if (row.Count == 0) { continue; }
                if (row.ContainsKey(colName))
                {
                    var obj = (row[colName]);
                    list.Add((T)(obj));
                }
                else
                {
                    list.Add(defaultValue);
                }
            }
            return list;
        }
        #endregion
        #endregion
    }
}