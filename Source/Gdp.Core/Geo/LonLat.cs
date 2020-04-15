using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    public class LonLat
    {
        public LonLat(double lon = 0, double lat=0)
        {
            this.Lon = lon;
            this.Lat = lat; 
        }

        public double Lon { get; set; }
        public double Lat { get; set; } 
    }
}
