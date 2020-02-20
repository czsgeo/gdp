using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    public class NEU
    {
        public NEU(double n=0, double e=0, double u=0)
        {
            this.U = u;
            this.E = e;
            this.N = n;
        }

        public double N { get; set; }
        public double E { get; set; }
        public double U { get; set; }
        public double Length => Math.Sqrt(N*N + E*E +U*U);
        public ENU ENU => new ENU(e:E, n:N, u:U);
    }
}
