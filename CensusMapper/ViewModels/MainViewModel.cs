using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;

namespace CensusMapper.ViewModels
{
    class MainViewModel
    {
        public MainViewModel()
        {
            ZoomLevel = 5.0;
        }

        public double ZoomLevel { get; set; }
    }
}
