//2016.02.18, czs, create in hongqing, 枚举工具

using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp.Utils
{
    /// <summary>
    /// 枚举工具
    /// </summary>
    public class EnumUtil
    {
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        static public List<TEnum> GetList<TEnum>()
        {
            List<TEnum> enums = new List<TEnum>();
            var names = Enum.GetValues(typeof(TEnum));
            foreach (var item in names)
            {
                enums.Add((TEnum)item);
            }

            return enums;
        }

        /// <summary>
        /// 获取其它
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="shouldNotBes"></param>
        /// <returns></returns>
        static public List<TEnum> GetOthers<TEnum>(List<TEnum> shouldNotBes)
        {
            var all = new List<TEnum>( GetList<TEnum>());
            //List<TEnum> enums = new List<TEnum>();
            //foreach (var key in shouldNotBes)
            //{
            //    if(!all.Contains(key)){
            //        enums.Add(key);
            //    }
            //}
            return all.FindAll(m => !shouldNotBes.Contains(m));

            //return enums;
        }

        /// <summary>
        /// 解析名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Parse<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }
        /// <summary>
        /// 尝试解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryParse<T>(string name, T defaultValue= default(T))
        {
            try
            {
                return (T)Enum.Parse(typeof(T), name, true);
            }catch(Exception ex)
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetNames(Type type)
        {
            return Enum.GetNames(type);
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetNames<TEnum>()
        {
            return GetNames(typeof(TEnum));
        }
    }
}
