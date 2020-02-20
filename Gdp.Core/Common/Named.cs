//2014.05.24，czs, created

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text; 
 
using Gdp.IO;

namespace Gdp
{
    /// <summary>
    /// 名称.封装了一个Name属性。实质上就是一个string对象。
    /// </summary>
    [Serializable]
    public class Named : Namable
    { 
        /// <summary>
        /// 构造函数，为名称赋值。默认为空。
        /// </summary>
        /// <param name="name"></param>
        public Named(string name = "")
        {
            this.Name = name;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }

        #region override 
        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
        /// <summary>
        /// 如果只是一个同名字符串，也会相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //if (obj is Named) 
            //    return Name.Equals(((Named)obj).Name);
            //if (obj is String)
            //    return Name.Equals((obj.ToString()));

            return Name.Equals(obj.ToString());
        }
        /// <summary>
        /// 哈希数
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="named"></param>
        /// <returns></returns>
        public static List<string> GetNames<T>(IEnumerable<T> named) where T : Namable
        {
            List<string> names = new List<string>();
            foreach (var item in named)
            {
                names.Add(item.Name);
            }

            return names;
        }
    }
}
