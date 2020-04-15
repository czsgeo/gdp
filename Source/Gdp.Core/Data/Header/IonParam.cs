using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gdp.Data.Rinex
{
    /// <summary>
    /// 电离层参数。导航文件参数,电离层
    /// </summary>
    public struct IonParam 
    {
        public double AlfaA0 { get; set; }
        public double AlfaA1 { get; set; }
        public double AlfaA2 { get; set; }
        public double AlfaA3 { get; set; }
        public double BetaB0 { get; set; }
        public double BetaB1 { get; set; }
        public double BetaB2 { get; set; }
        public double BetaB3 { get; set; }

        public bool HasValue { get { return !this.IsZero; } }

        public bool IsZero
        {
            get
            {
                return (AlfaA0 == 0
                    && AlfaA1 == 0
                    && AlfaA2 == 0
                    && AlfaA3 == 0
                    && BetaB0 == 0
                    && BetaB1 == 0
                    && BetaB2 == 0
                    && BetaB3 == 0);
            }
        }
    }
}