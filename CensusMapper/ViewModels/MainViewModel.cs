using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bing.Maps;
using GalaSoft.MvvmLight.Command;

namespace CensusMapper.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            ZoomLevel = 5.0;
        }

        public double ZoomLevel { get; set; }

        public ICommand DoSomethingCommand
        {
            get
            {
                return new RelayCommand<string>((p) =>
                {
                    System.Diagnostics.Debug.WriteLine("Hi there {0}", p);
                });
            }
        }
    }
}
