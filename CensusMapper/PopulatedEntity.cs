using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper
{
    public class PopulatedEntity : UsState
    {
        public PopulatedEntity()
        {
            
        }

        public PopulatedEntity(UsState state)
            : base(state.Fips, state.Name, state.Center.Latitude, state.Center.Longitude)
        {
            
        }
        public int Population { get; set; }
        public string FormattedPopulation
        {
            get { return string.Format("{0:0,0}", this.Population); }
        }
    }
}
