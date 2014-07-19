using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace CensusMapper
{
    class UsState
    {
        public UsState(string fips, string name, double lat, double lng)
        {
            Fips = fips;
            Name = name;
            Center = new Geopoint(new BasicGeoposition() { Latitude = lat, Longitude = lng});
        }
        public string Fips { get; set; }
        public string Name { get; set; }
        public Geopoint Center { get; set; }
    }
}
