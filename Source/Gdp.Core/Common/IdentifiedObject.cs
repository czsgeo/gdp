using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{
    /// <summary>
    /// 通用基础信息，包含名称、ID等。
    /// </summary>
    public class IdentifiedObject : AbbrevIdNamed
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public IdentifiedObject() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">Name</param>
        /// <param name="abbreviation">简称</param>
        public IdentifiedObject( string id, string name, string abbreviation = null)
        {
            this.Name = name;
            this.Id = id;
            if (abbreviation == null) this.Abbreviation = abbreviation;
            else this.Abbreviation = abbreviation;
        }
    }
}
