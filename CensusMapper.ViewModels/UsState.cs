using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace CensusMapper.ViewModels
{
    public class UsState : ViewModelBase
    {
        public UsState()
        {
            
        }

        public UsState(string fips, string name, double lat, double lng)
        {
            Fips = fips;
            Name = name;
            Center = new Coordinates(lat, lng);
        }

        public string Fips { get; set; }
        public string Name { get; set; }
        public Coordinates Center { get; set; }
    }
}
