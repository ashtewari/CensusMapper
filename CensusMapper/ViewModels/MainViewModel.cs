using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bing.Maps;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CensusMapper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MapViewModel _mapViewModel;
        private ICommand _searchCommand;

        public MainViewModel(string keyBingMaps, string keyCensus)
        {
            _mapViewModel = new MapViewModel(keyBingMaps, keyCensus);

            _searchCommand = new RelayCommand<string>(DoSearch, CanDoSearch);
        }

        public MapViewModel MapViewModel
        {
            get { return _mapViewModel; }
            set
            {
                _mapViewModel = value;
                RaisePropertyChanged("MapViewModel");
            }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand; }
            set
            {
                _searchCommand = value;
                RaisePropertyChanged("SearchCommand");
            }

        }

        public ICommand LoadData
        {
            get { return new RelayCommand<string>(async (arg) =>
                {
                    System.Diagnostics.Debug.WriteLine(arg);
                    await _mapViewModel.LoadStateData();
                }
                ); 
            }
        }

        private void DoSearch(string param)
        {
            System.Diagnostics.Debug.WriteLine("Hi there {0}", param);
        }

        private bool CanDoSearch(string arg)
        {
            return !string.IsNullOrEmpty(arg);
        }
    }
}
