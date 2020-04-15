using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    public class HEN
    {
        public HEN() { }
        public HEN(double h, double e, double n)
        {
            this.H = h;
            this.E = e;
            this.N = n;
        }

        public double H { get; set; }
        public double E { get; set; }
        public double N { get; set; }
    }
}
