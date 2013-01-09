using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;

namespace CensusMapper
{
    public class UsState
    {
        public UsState()
        {
            
        }

        public UsState(string fips, string name, double lat, double lng)
        {
            Fips = fips;
            Name = name;
            Center = new Location(lat, lng);
        }
        public string Fips { get; set; }
        public string Name { get; set; }
        public Location Center { get; set; }
    }
}
