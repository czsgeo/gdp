//2014.05.24，czs, created

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 名称
    /// </summary>
    [Serializable]
    public class IdNamed : IStringIdName
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 是否相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is IdNamed) )return false;
            IdNamed name = obj as IdNamed;

            return Id == name.Id && Name == name.Name;
        }
        /// <summary>
        /// 哈希数。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int code = 0;
            if(Id!=null) code += Id.GetHashCode() * 3;
            if (Name != null) code += Name.GetHashCode() * 13;

            return code;
        }
        /// <summary>
        /// 默认以逗号隔开的字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id + "," + Name;
        }

    }
}
