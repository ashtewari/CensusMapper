using System.Windows.Input;
using CensusMapper.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CensusMapper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MapViewModel _mapViewModel;
        private ICommand _searchCommand;
        private ICommand _clearCommand;
        private ICommand _refreshCommand;

        public MainViewModel(IBingMapsApi bingMapsApi, ICensusApi censusApi)
        {
            _mapViewModel = new MapViewModel(bingMapsApi, censusApi);

            _searchCommand = new RelayCommand<string>(DoSearch, CanDoSearch);
            _refreshCommand = new RelayCommand(DoRefresh);
            _clearCommand = new RelayCommand(DoClear, CanDoClear);
        }

        private bool CanDoClear()
        {
            return this.MapViewModel.Items.Count > 0;
        }

        private void DoClear()
        {
            this.MapViewModel.Items.Clear();
        }

        private async void DoRefresh()
        {
            await this.MapViewModel.LoadStateData();
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

        public ICommand RefreshCommand
        {
            get { return _refreshCommand; }
            set
            {
                _refreshCommand = value;
                RaisePropertyChanged("RefreshCommand");
            }
        }

        public ICommand ClearCommand
        {
            get { return _clearCommand; }
            set
            {
                _clearCommand = value;
                RaisePropertyChanged("ClearCommand");
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
