//2014.05.24，czs, created

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 可标识接口。具有 Id 和 Name 属性。
    /// </summary>
    public interface IStringIdName : IdentifiableNamed<String>, IStringId
    {
    }
    public interface IIntIdName : IdentifiableNamed<int>, IIntId
    {
    }

    public interface IdentifiableNamed<TKey> : Namable, Identifiable<TKey>
    {
    }

    public abstract class IntIdName : IdentifyNamed<int>, IIntIdName
    {

    }

    public abstract class StringIdName : IdentifyNamed<string>, IStringIdName
    {

    }


    /// <summary>
    /// 具有ID和Name的基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class IdentifyNamed<TKey> : Identified<TKey>, IdentifiableNamed<TKey>
    {
        public IdentifyNamed()
        {

        }

        /// <summary>
        /// 标识
        /// </summary>
        public virtual string Name { get; set; }

        public override string ToString()
        {
            return Id + ":" + Name;
        }

        public override bool Equals(object obj)
        {
            IdentifyNamed<TKey> o = obj as IdentifyNamed<TKey>;
            if (o == null) return false;

            return Id.Equals(o.Id) && Name == o.Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() * 13 + Name.GetHashCode();
        }
    }
      

}
