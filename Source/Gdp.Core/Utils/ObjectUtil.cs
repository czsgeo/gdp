using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.IO;
using  System.Reflection; 
using System.Data;

namespace Gdp.Utils
{
    /// <summary>
    /// 属性的名称和内容
    /// </summary>
    public class AttributeItem
    {
        public AttributeItem() { }

        /// <summary>
        /// 属性显示的名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 属性的名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }

        public override string ToString()
        {
            return DisplayName + " : " + Value;
        }
    }


    /// <summary>
    /// 对象工具
    /// </summary>
      public static class ObjectUtil
      { 
          /// <summary>
          /// 是否是数字，包含双精度，浮点数，整型等,不含char。
          /// </summary>
          /// <param name="obj"></param>
          /// <returns></returns>
          static public bool IsNumerial(Object obj)
          {
              if (obj == null) return false;

              return obj is double
                 || obj is Int32
                   || obj is float
                   || obj is Int16
                   || obj is Int64
                   || obj is short
                  //|| obj is char
                   || obj is decimal
                   || obj.ToString() == "NaN"
                   ;
          }
        /// <summary>
        /// 解析浮点数或整数，返回双精度。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultVal">失败返回默认</param>
        /// <returns></returns>
        static public double GetNumeral(object obj, double defaultVal =double.NaN)
          {
              if (obj == null) { return Double.NaN; }
              if (IsFloat(obj)) return (double)obj;
              if (IsInteger(obj))
              {
                  if (obj is long) { return (long)obj; }
                  if (obj is Int32) { return (int)obj; }
                  //if (obj.ToString().Length >=10) { return (long)obj; }
                  //return (int)obj;
              }
              return defaultVal;
          }

          /// <summary>
          /// 是否是浮点数
          /// </summary>
          /// <param name="obj"></param>
          /// <returns></returns>
          static public bool IsFloat(Object obj)
          {
              return obj is double 
                   || obj is float 
                   || obj is decimal
                   ;
          }
          /// <summary>
          /// 是否是整数，含 char
          /// </summary>
          /// <param name="obj"></param>
          /// <returns></returns>
          static public bool IsInteger(Object obj)
          {
              return 
                  obj is Int32 
                   || obj is Int16
                   || obj is Int64
                   || obj is short
                  || obj is char 
                   ;
          }

          /// <summary>
          /// 根据类型解析数值。
          /// </summary>
          /// <param name="valString"></param>
          /// <param name="type"></param>
          /// <returns></returns>
          public static object ParseValue(string valString, Type type)
          {
              if (String.IsNullOrWhiteSpace(valString))
              {
                  return null;
              }
              object newValue = valString;
              if (type == typeof(string)) //如此可以减少判断，对于string
              {
                  newValue = (valString);
              }
              else if (type == typeof(int))
              {
                  newValue = int.Parse(valString);
              }
              else if (type == typeof(double))
              {
                  newValue = double.Parse(valString);
              }
              else if (type == typeof(Boolean))
              { 
                  newValue = Boolean.Parse(valString);
              }
              else if (type == typeof(DateTime))
              {
                  newValue = DateTime.Parse(valString);
              }
              return newValue;
          }


        /// <summary>
        /// 遍历所有的属性，确保访问一次（加载到内存） 
        /// </summary>
        /// <param name="obj"></param>
        public static void VisitAllProperties(Object obj)
        {
            var type =  (obj).GetType();
            var ps = type.GetProperties();
            List<object> vals = new List<object>();
            foreach (var p in ps)
            {
                try
                {
                    //if (p.PropertyType.Name == "ConfigItem")
                    //    continue;

                    var val = p.GetValue(obj, null);
                    vals.Add(val);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        /// <summary>
        /// 属性名称列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static public List<string> GetPropertyNames(Type type)
          {
              List<string> properties = new List<string>();
              var ps = type.GetProperties();
              foreach (var item in ps)
              {
                  properties.Add(item.Name);
              }
              return properties;
          }
          static public List<string> GetPropertyNames(Object obj) { return GetPropertyNames(obj.GetType()); }
            
          /// <summary>
          /// 由字符串修改为对象。
          /// </summary>
          /// <typeparam name="T"></typeparam>
          /// <param name="str"></param>
          /// <param name="splitter"></param>
          /// <param name="useDisplayName"></param>
          /// <returns></returns>
          public static T GetObject<T>(string str, string splitter = "\t", bool useDisplayName = false) where T : new()
          {
              Dictionary<string, string> dic = GetPropertyDictionary(str, splitter);

              T t = new T();
              Type type = t.GetType();
              foreach (var kv in dic)
              {
                  var info = type.GetField(kv.Key);
                  string val = kv.Value;
                  //Convert.to

                  info.SetValue(t, val);//是否需要转换??2015.05.07
              }

              return t;
          }

          /// <summary>
          /// 获取属性字典。
          /// </summary>
          /// <typeparam name="T"></typeparam>
          /// <param name="str"></param>
          /// <param name="splitter"></param>
          /// <returns></returns>
          public static Dictionary<string, string> GetPropertyDictionary(string str, string splitter) 
          {
              char[] attriSplitter = new char[] { '\n', '\r' };
              string[] valSplitter = new string[] { splitter };

              string[] strs = str.Split(attriSplitter, StringSplitOptions.RemoveEmptyEntries);
              Dictionary<string, string> dic = new Dictionary<string, string>();
              foreach (var kv in strs)
              {
                  var attris = kv.Split(valSplitter, StringSplitOptions.RemoveEmptyEntries);
                  var key = attris[0];
                  var val = attris[1];

                  dic.Add(key, val);
              }
              return dic;
          }

          /// <summary>
          /// 将对象以文本形式
          /// </summary>
          /// <param name="obj">对象</param>
          /// <param name="splitter">属性值分隔符，默认为制表符\t</param>
          /// <returns></returns>
          public static string GetFormatedText(object obj, string splitter = "\t", bool useDisplayName = false)
          {
              var list = Gdp.Utils.ObjectUtil.GetAttributes(obj, useDisplayName);
              StringBuilder sb = new StringBuilder();
              foreach (var item in list)
              {
                  sb.AppendLine(item.DisplayName + splitter + item.Value);
              }
              return sb.ToString();
          }


          /// <summary>
          /// 对象的导航属性是否等于指定属性
          /// </summary>
          /// <typeparam name="TEntity">对象</typeparam>
          /// <typeparam name="TNavProperty">导航属性类型</typeparam>
          /// <param name="entity"></param>
          /// <param name="navProperty"></param>
          /// <returns></returns>
          public static bool EqualNavPropertiy<TEntity, TNavProperty>(TEntity entity, TNavProperty navProperty)
          {
              var navPropertyInfo = Gdp.Utils.ObjectUtil.GetNavPropertyInfo(typeof(TEntity), typeof(TNavProperty));
              if (navPropertyInfo == null) throw new Exception(typeof(TNavProperty) + " 没有导航属性 " + typeof(TEntity));

              return (navPropertyInfo.GetValue(entity, null).Equals(navProperty));
          }

          /// <summary>
          /// 返回第一个类型匹配的导航属性信息
          /// </summary>
          /// <param name="objType"></param>
          /// <param name="navPropertyType"></param>
          /// <returns></returns>
          static public PropertyInfo GetNavPropertyInfo(Type objType, Type navPropertyType)
          {
              if (objType == null || navPropertyType == null) return null;

              System.Reflection.PropertyInfo[] infos = objType.GetProperties();
              foreach (System.Reflection.PropertyInfo info in infos)
              {
                  if (info.PropertyType == navPropertyType)
                  {
                      return info;
                  }
              }
              return null;
          }
          /// <summary>
          /// 返回所有导航属性的信息。
          /// </summary>
          /// <param name="objType"></param>
          /// <param name="navPropertyType"></param>
          /// <returns></returns>
          static public PropertyInfo GetNavPropertyInfo(Type navPropertyType, string CollectionName)
          { 
              System.Reflection.PropertyInfo[] infos = navPropertyType.GetProperties();
              foreach (System.Reflection.PropertyInfo info in infos)
              {
                  if (info.Name == CollectionName)
                  {
                     return (info);
                  }
              }
              return null;
          }

          /// <summary>
          /// 返回所有导航属性的信息。
          /// </summary>
          /// <param name="objType"></param>
          /// <param name="navPropertyType"></param>
          /// <returns></returns>
          static public List<PropertyInfo> GetNavPropertyInfos(Type objType, Type navPropertyType, bool subIncluded = false)
          {
              List<PropertyInfo> pinfos = new List<PropertyInfo>();
              if (objType == null || navPropertyType == null) return pinfos;

              System.Reflection.PropertyInfo[] infos = objType.GetProperties();
              foreach (System.Reflection.PropertyInfo info in infos)
              {
                  if (subIncluded)
                  if (info.PropertyType.IsSubclassOf(navPropertyType))
                  {
                      pinfos.Add( info);
                  }
                  else
                  {
                      if (info.PropertyType == (navPropertyType))
                      {
                          pinfos.Add(info);
                      }

                  }
              }
              return pinfos;
          }


          /// <summary>
          /// 返回所有导航属性的信息。
          /// </summary>
          /// <param name="objType"></param>
          /// <param name="navPropertyType"></param>
          /// <returns></returns>
          static public List<PropertyInfo> GetEnumNavPropertyInfos(Type objType)
          {
              List<PropertyInfo> pinfos = new List<PropertyInfo>();
              if (objType == null ) return pinfos;

              System.Reflection.PropertyInfo[] infos = objType.GetProperties();
              foreach (System.Reflection.PropertyInfo info in infos)
              { 
                  if (info.PropertyType.IsEnum)
                  {
                      pinfos.Add( info);
                  } 
              }
              return pinfos;
          }
          public static Type GetPropertyType<T>(string propertyName)
          {
              var type = typeof(T);
              var item = type.GetProperty(propertyName);
              return item.PropertyType;
          }

         

          /// <summary>
          /// 对象制定属性的名称。如果有多个，只返回第一个；没有则返回null.
          /// </summary>
          /// <param name="objType">对象类型</param>
          /// <param name="propertyType">属性类型</param>
          /// <returns></returns>
          public static string GetPropertyName(Type objType, Type propertyType)
          {
              if (objType == null || propertyType == null) return null;
              string treeNodeName = null;

              System.Reflection.PropertyInfo[] infos = objType.GetProperties();
              foreach (System.Reflection.PropertyInfo info in infos)
              {
                  if (info.PropertyType == propertyType)
                  {
                      treeNodeName = info.Name; return treeNodeName;
                  }
              }
              return treeNodeName;
          }
          /// <summary>
          /// 对象制定属性的名称。如果有多个，只返回第一个；没有则返回null.
          /// </summary>
          /// <typeparam name="TEntity"></typeparam>
          /// <typeparam name="TNav"></typeparam>
          /// <returns></returns>
          public static string GetPropertyName<TEntity, TNav>()
          {
              return GetPropertyName(typeof(TEntity), typeof(TNav));
          }

          /// <summary>
          /// 对象制定属性的名称。如果有多个，只返回第一个；没有则返回null.
          /// </summary>
          /// <param name="objType"></param>
          /// <param name="propertyType"></param>
          /// <returns></returns>
          public static string GetPropertyName<T>(T t, Type propertyType)
          {
              if (t == null || propertyType == null) return null;
              string treeNodeName = null;

              System.Reflection.PropertyInfo[] infos = t.GetType().GetProperties();
              foreach (System.Reflection.PropertyInfo info in infos)
              {
                  if (info.PropertyType == propertyType)
                  {
                      treeNodeName = info.Name; return treeNodeName;
                  }
              }
              return treeNodeName;
          }
          public static List<string> GetPropertyNames( Type type, Type propertyType)
          {
              List<string> list = new List<string>(); 
              System.Reflection.PropertyInfo[] infos = type.GetProperties();
              foreach (System.Reflection.PropertyInfo info in infos)
              {
                  if (info.PropertyType == propertyType)
                  list.Add(info.Name);
              }
              return list;
          }
          /// <summary>
          /// 获取指定属性类型的属性值。
          /// </summary>
          /// <param name="objType"></param>
          /// <param name="propertyType"></param>
          /// <returns></returns>
          public static Object GetPropertyValue<T>(T t, Type propertyType)
          {
              string propertyname =  GetPropertyName( t.GetType(),  propertyType);
              if (propertyname == null) return null;
              return GetPropertyValue<T>(t, propertyname);
          }
          /// <summary>
          ///  获取指定属性类型的属性值。
          /// </summary>
          /// <typeparam name="T"></typeparam>
          /// <param name="t"></param>
          /// <param name="propertyname"></param>
          /// <returns></returns>
          public static Object GetPropertyValue<T>(T t, string propertyname)
          {
              Type type = typeof(T);
              System.Reflection.PropertyInfo property = type.GetProperty(propertyname);
              if (property == null) return null;
              return property.GetValue(t, null);               
          }
          /// <summary>
          ///  获取对象属性列表。默认具有属性名称的才获取。
          /// </summary>
          /// <param name="obj"></param>
          /// <returns></returns>
          public static List<AttributeItem> GetAttributes(object obj)
          {
              return  GetAttributes(obj, true);
          }
          /// <summary>
          /// 排序比较
          /// </summary>
          /// <param name="propertyName"></param>
          /// <param name="SortOrder"></param>
          /// <param name="a"></param>
          /// <param name="b"></param>
          /// <returns></returns>
          public static int Compare<T>(T a, T b, string propertyName, bool Ascending)
          {
              if (String.IsNullOrWhiteSpace(propertyName)) { return 0; }

              var val1 = Gdp.Utils.ObjectUtil.GetPropertyValue(a, propertyName);
              var val2 = Gdp.Utils.ObjectUtil.GetPropertyValue(b, propertyName);

              if (val1 is IComparable)
              {
                  var va1 = val1 as IComparable;
                  var va2 = val2 as IComparable;
                  if (Ascending)
                  {
                      return va1.CompareTo(va2);
                  }
                  else
                  {
                      return va2.CompareTo(va1);
                  }
              }
              return 0;
          }
          /// <summary>
          /// 获取对象属性列表。
          /// </summary>
          /// <param name="obj"></param>
          /// <param name="useDisplayName"></param>
          /// <returns></returns>
          public static List<AttributeItem> GetAttributes(object obj, bool useDisplayName)
        {
            List<AttributeItem> li = new List<AttributeItem>();
            if(obj  == null) { return li; }
            Type t = obj.GetType();// typeof(Geo.Domain.Entities.TreeNode);

              System.Reflection.PropertyInfo[] infos = t.GetProperties();
              foreach (System.Reflection.PropertyInfo info in infos)
              {
                  string attrName = info.Name;
                  if (useDisplayName)
                  {
                      string displayName = GetDisplayName(info);
                      if (displayName == null) continue;
                      attrName = displayName;
                  }
                  AttributeItem item = new AttributeItem();
                  item.PropertyName = info.Name; 
                  item.DisplayName = attrName;//
                  string str = "[.]";
                try
                {
                    var val = info.GetValue(obj, null);
                    str = StringUtil.ToString(val);
                }
                catch (Exception ex) {
                    Console.WriteLine("ObjectUtil.GetAttributes 错误 " + ex.Message);
                }

                item.Value = str;
                  li.Add(item);
              }
              return li;
          }
    

          static public List<string> GetDisplayNames(Type type)
          {
              List<string> properties = new List<string>();
              var ps = type.GetProperties();
              foreach (var item in ps)
              {
                  var name = GetDisplayName(item);
                  if(name == null)continue;
                  properties.Add(name);
              }
              return properties;
          }

          /// <summary>
          /// 如果没有DisplayName则返回 null
          /// </summary>
          /// <param name="info"></param>
          /// <returns></returns>
          public static string GetDisplayName(System.Reflection.PropertyInfo info)
          {
             

              object[] _DisplayList = info.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), true);
              if (_DisplayList.Length == 0) return null;

              System.ComponentModel.DisplayNameAttribute _Display = (System.ComponentModel.DisplayNameAttribute)_DisplayList[0];
              return _Display.DisplayName;
          }
           
          /// <summary>
          /// 创建默认值
          /// </summary>
          /// <param name="targetType"></param>
          /// <returns></returns>
          public static object Default(Type targetType)
          {
              return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
          }
          /// <summary>
          /// 对象是否为空,如果为null，或者字符串为空，则返回true。
          /// </summary>
          /// <param name="cell"></param>
          /// <returns></returns>
          public static bool IsEmpty(object cell)
          {
              return cell == null || String.IsNullOrEmpty( cell.ToString());
          }
      }
    } 
