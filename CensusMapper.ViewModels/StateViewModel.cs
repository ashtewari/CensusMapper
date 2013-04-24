using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper.ViewModels
{
    public class StateViewModel : PopulatedEntityViewModel
    {
        public StateViewModel(CensusEntityViewModel state) : base(state)
        {
            
        }
    }
}
