using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper
{
    public class UsState
    {
        public UsState(string fips, string name, double lat, double lng)
        {
            Fips = fips;
            Name = name;
            Center = new Coordinates() { Latitude = lat, Longitude = lng};
        }
        public string Fips { get; set; }
        public string Name { get; set; }
        public Coordinates Center { get; set; }
    }
}
