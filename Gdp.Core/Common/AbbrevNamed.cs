//2014.05.24，czs, created

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gdp
{ 

    /// <summary>
    /// 具有名称和别称
    /// </summary>
    [Serializable]
    public class AbbrevNamed : Named
    {
        /// <summary>
        /// 名称简称
        /// </summary>
        public string Abbreviation { get; set; }
    }

     
}
