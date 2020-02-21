//2016.02.18, czs, create in hongqing, 标识最高接口

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 可标识的，具有一个Id属性。
    /// </summary>
    public interface Identifiable<T>
    {
        /// <summary>
        /// 标识
        /// </summary>
        T Id { get; set; }
    }
    /// <summary>
    /// ID 为字符串类型接口
    /// </summary>
    public interface IStringId : Identifiable<String>
    {
    }
    /// <summary>
    /// ID 为整型接口
    /// </summary>
    public interface IIntId : Identifiable<int>
    {
    }
    /// <summary>
    /// 向量指示
    /// </summary>
    public interface IVectorStringId : IStringId
    {
        /// <summary>
        /// 目标ID
        /// </summary>
        string ToId { get; set; }
    }
    /// <summary>
    /// 向量ID
    /// </summary>
    public class VectorStringId : StringId, IVectorStringId
    {
        public VectorStringId()
        {

        }
        /// <summary>
        /// 目标ID
        /// </summary>
        public string ToId { get; set; } 

        public override string ToString()
        {
            return Id +"->"+ ToId;
        }

        public override bool Equals(object obj)
        {
            var o = obj as VectorStringId;
           return (o.Id == Id && o.ToId == ToId) ;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + ToId.GetHashCode() * 13;
        }
    }

    /// <summary>
    /// 字符串ID
    /// </summary>
    public class StringId : Identified<string>, IStringId
    { 
    }
    /// <summary>
    /// 整型数字ID
    /// </summary>
    public class IntId : Identified<int>,IIntId
    { 

    }

    /// <summary>
    /// 区别与标识
    /// </summary>
    public abstract class Identified<T> : Identifiable<T>
    { 
        /// <summary>
        /// 标识
        /// </summary>
        public virtual T Id { get; set; }

        public override string ToString()
        {
            return Id + "";
        }

        public override bool Equals(object obj)
        {
            Identifiable<T> o = obj as Identifiable<T>;
            if (o == null) return false;

            return Id.Equals(o.Id);
        }

        public override int GetHashCode()
        { 
            return Id.GetHashCode() * 13;
        }

        /// <summary>
        /// 获取所有的ID
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<T> GetIds(IEnumerable entities)
        {
            List<T> ids = new List<T>();
            foreach (var item in entities)
            {
                if (item is Identifiable<T>)
                {
                    ids.Add((item as Identifiable<T>).Id);
                }
            }
            return ids;
        }
    } 
}
