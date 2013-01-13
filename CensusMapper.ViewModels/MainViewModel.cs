using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CensusMapper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MapViewModel _mapViewModel;
        private ICommand _searchCommand;

        public MainViewModel(IBingMapsApi bingMapsApi, ICensusApi censusApi)
        {
            _mapViewModel = new MapViewModel(bingMapsApi, censusApi);

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
            this.MapViewModel.Search(param);
        }

        private bool CanDoSearch(string arg)
        {
            return true;
        }
    }
}
