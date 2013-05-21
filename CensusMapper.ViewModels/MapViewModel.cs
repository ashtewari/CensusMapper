using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CensusMapper.Models;
using CensusMapper.Services;
using GalaSoft.MvvmLight;

namespace CensusMapper.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        private ObservableCollection<PopulatedEntityViewModel> _items;        

        private IBingMapsApi bingMaps;
        private ICensusApi censusApi;

        private object _centerOfUs;
        private double _zoomLevel;

        public MapViewModel(IBingMapsApi bingMaps, ICensusApi censusApi)
        {
            this.bingMaps = bingMaps;
            this.censusApi = censusApi;

            _items = new ObservableCollection<PopulatedEntityViewModel>();
            _centerOfUs = new Coordinates(39.833333, -98.583333);
            _zoomLevel = 5.0;
        }

        public object CenterOfUs
        {
            get { return _centerOfUs; }
            set
            {
                _centerOfUs = value;                
                RaisePropertyChanged("CenterOfUs");
            }
        }

        public double CurrentZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                _zoomLevel = value;
                RaisePropertyChanged("CurrentZoomLevel");
            }
        }

        public ObservableCollection<PopulatedEntityViewModel> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public async Task SelectLocation(Coordinates coordinates)
        {
            Address address = await bingMaps.GetAddress(new Location(coordinates.Latitude, coordinates.Longitude));
            if (address == null) return;
            
            string fips = Mappings.StateAbbreviationToFips(address.AdminDistrict);
            if (string.IsNullOrEmpty(fips)) return;

            var population = await censusApi.GetPopulationForZipCode(fips, address.PostalCode);
            if (population == null) return;

            Items.Add(new ZipCodeViewModel(new CensusEntityViewModel(fips, string.Format("{0} ({1})", address.Locality, address.PostalCode), coordinates.Latitude, coordinates.Longitude))
            {
                Population = population
            });
        }

        public async Task LoadStateDataOneByOne()
        {
            try
            {
                Dictionary<string, CensusEntityViewModel> statesList = Mappings.GetStatesList();

                foreach (var usState in statesList.Keys)
                {
                    var array = await censusApi.GetPopulationForStates(new List<string>() {usState} );
                    if (array == null) return;

                    foreach (var item in array)
                    {
                        CensusEntityViewModel state = statesList[item.Key];
                        Items.Add(new StateViewModel(state) {Population = item.Value});
                    }
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }            
        }

        public async Task LoadStateData()
        {
            try
            {                
                var array = await censusApi.GetPopulationForStates(new List<string>() {"*"});
                if (array == null) return;

                Dictionary<string, CensusEntityViewModel> statesList = Mappings.GetStatesList();

                foreach (var item in array)
                {
                    if (statesList.ContainsKey(item.Key))
                    {
                        CensusEntityViewModel state = statesList[item.Key];
                        Items.Add(new StateViewModel(state) {Population = item.Value});
                    }
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
        }        

        public void Search(string text)
        {
            foreach (var item in this.Items)
            {
                if (string.IsNullOrEmpty(text) || (item.Name.ToLowerInvariant().Contains(text.ToLowerInvariant())))
                {
                    item.IsVisible = true;
                }
                else
                {
                    item.IsVisible = false;
                }
            }
        }
    }
}
