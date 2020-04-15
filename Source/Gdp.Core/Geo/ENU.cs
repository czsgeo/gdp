using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    public class ENU
    {
        public ENU(double e=0, double n = 0, double u =0)
        {
            this.U = u;
            this.E = e;
            this.N = n;
        }

        public double N { get; set; }
        public double E { get; set; }
        public double U { get; set; }
        public double Length => Math.Sqrt(N * N + E * E + U * U);
    }
}
