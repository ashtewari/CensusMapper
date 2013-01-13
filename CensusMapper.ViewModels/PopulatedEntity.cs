using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper.ViewModels
{
    public class PopulatedEntity : UsState
    {
        private bool _isVisible;

        public PopulatedEntity(UsState state)
            : base(state.Fips, state.Name, state.Center.Latitude, state.Center.Longitude)
        {
            IsVisible = true;
        }
        public int? Population { get; set; }
        public string FormattedPopulation
        {
            get { return string.Format("{0:0,0}", this.Population); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged("IsVisible");
            }
        }
    }
}
