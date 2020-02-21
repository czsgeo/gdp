using System;
using System.Collections.Generic;
using System.Text;

namespace Gdp
{
    public class GeoCoord : LonLat
    {
        public GeoCoord(double lon, double lat, double height)
        {
            this.Lon = lon;
            this.Lat = lat;
            this.Height = height;
        }
         
        public double Height { get; set; }
    }
}
