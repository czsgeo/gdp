//2016.11.25, czs, create in hongqing, 具有数据类型的值
//2017.08.28, czs, edit in hongqing, 去掉Int类型，统一为Long类型。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Gdp
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public enum ValueType
    {
        Unknown,Long, Float, Str, Time, Bool, RmsedNumeral//Int,
    }

    /// <summary>
    /// 帮助器
    /// </summary>
    public class ValueTypeHelper
    {
        /// <summary>
        /// 尝试分析字符串的数据类型
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ValueType GetValueType(string val, bool isIntOrFloatFirst = false)
        {
            if (String.IsNullOrWhiteSpace(val)) { return ValueType.Unknown; }
            if(val.Contains("(")) { return ValueType.RmsedNumeral; }
            if (isIntOrFloatFirst)
            {
                if (Gdp.Utils.StringUtil.IsNumber(val)) { return ValueType.Long; }
                if (Gdp.Utils.StringUtil.IsDecimal(val)) { return ValueType.Float; } 
            }
            else
            {
                if (Gdp.Utils.StringUtil.IsDecimal(val)) { return ValueType.Float; }
                if (Gdp.Utils.StringUtil.IsNumber(val)) { return ValueType.Long; }
            }
            //if (Geo.Utils.StringUtil.IsNumber(currentVal) && currentVal.Length <= int.MaxValue.ToString().Length )
            //{
            //    return ValueType.Int;
            //}            

           if(val.Equals("true", StringComparison.CurrentCultureIgnoreCase) ||
           val.Equals("false", StringComparison.CurrentCultureIgnoreCase))
           {
               return ValueType.Bool;
           }
            DateTime datetime = DateTime.MinValue;
            if (DateTime.TryParse(val, out datetime))
            {
                return ValueType.Time;
            }

           return ValueType.Str;
        }
    }

    /// <summary>
    /// 命名的类型
    /// </summary>
    public class NamedType : Gdp.Named
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public NamedType()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public NamedType(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public NamedType(string name, Object obj)
        {
            this.Name = name;
            this.Type = obj.GetType();
        } 

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + " " + Type;
        }

        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            NamedType o = obj as NamedType;
            if (o == null) { return false; }

            return o.Name == this.Name && o.Type == this.Type;
        }
        /// <summary>
        /// 哈希数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        } 
    }
    /// <summary>
    /// 值 类型管理器
    /// </summary>
    public class NamedValueTypeManager : BaseDictionary<string, NamedValueType>
    {

        public void InitNames(string[] names)
        {
            Titles =new string[names.Length];
            int i = 0;
            foreach (var item in names)
            {
                var key = item.Trim();
                this[key] = Create(key);
                Titles[i++] = key;
            }
        }

        public string [] Titles { get; set; }
 
        public override NamedValueType Create(string key)
        {
            return new NamedValueType(key );
        }
    }

    /// <summary>
    /// 命名的类型
    /// </summary>
    public class NamedValueType
    {
        public NamedValueType() { }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public NamedValueType(string name, ValueType type= Gdp.ValueType.Str)
        {
            this.Name = name;
            this.ValueType = type;
        }
        #region 属性 
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数值类型
        /// </summary>
        public ValueType ValueType { get; set; }
        #endregion

        /// <summary>
        /// 转换值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public object GetValue(string val)
        { 
            return Convert(val, this.ValueType); 
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Object Convert(string str, ValueType type)
        {
            if (String.IsNullOrWhiteSpace(str) 　|| String.Equals("NULL", str.Trim(), StringComparison.CurrentCultureIgnoreCase)) { return null; }
            switch (type)
            {
                case ValueType.Float:
                    return Gdp.Utils.DoubleUtil.TryParse(str, null);
                //case ValueType.Int:
                //    return int.Parse(str);
                case ValueType.Long:
                    return long.Parse(str);
                case ValueType.Time:
                    return new Time( DateTime.Parse(str));
                case ValueType.Bool:
                    return Boolean.Parse(str);
                case ValueType.RmsedNumeral:
                    return RmsedNumeral.Parse(str);
                default:
                    return str;
            }
        }
    }

    /// <summary>
    /// 参数和数值，以及类型
    /// </summary>
    public class TypedValue : NamedValueType
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TypedValue()
        {

        }
        /// <summary>
        /// 采用字符串初始化
        /// </summary>
        /// <param name="str"></param>
        public TypedValue(string str)
        {
            var strs = str.Split(new char[] { '=', '|' }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            SetValue(strs[i++], strs[i++], strs[i++]);

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="type"></param>
        public TypedValue(string name, string val, string type)
        {
            SetValue(name, val, type);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="type"></param>
        private void SetValue(string name, string val, string type)
        {
            Name = name;
            ValueType = Gdp.Utils.EnumUtil.Parse<ValueType>(type);
            Value = Convert(val, ValueType);
        }
        #region 属性
        /// <summary>
        /// 数值
        /// </summary>
        public Object Value { get; set; } 
        #endregion

        /// <summary>
        /// Name=thisName|Int;
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append("=");
            sb.Append(Value);
            sb.Append("|");
            sb.Append(ValueType);

            return sb.ToString();
        }
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public TypedValue Parse(string str)
        {
            TypedValue val = new TypedValue(str);
            return val;

        }

    }
}
