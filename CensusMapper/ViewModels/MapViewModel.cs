using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Bing.Maps;
using CensusMapper.Converters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json.Linq;

namespace CensusMapper.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        private ObservableCollection<PopulatedEntity> _items;        

        private string bingMapsApiKey;
        private string censusApiKey;

        private BingMaps bingMaps;
        private Census census;
        private Location _centerOfUs;
        private double _zoomLevel;

        private ICommand _command;

        public MapViewModel(string keyBingMaps, string keyCensus)
        {
            bingMapsApiKey = keyBingMaps;
            censusApiKey = keyCensus;

            bingMaps = new BingMaps(bingMapsApiKey);
            census = new Census(censusApiKey);            

            _items = new ObservableCollection<PopulatedEntity>();

            _centerOfUs = new Location(39.833333, -98.583333);
            _zoomLevel = 5.0;
        }

        public Location CenterOfUs
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

        public ObservableCollection<PopulatedEntity> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public async Task<bool> SelectLocation(Location location)
        {
            Address address = await bingMaps.GetAddress(location);

            if (address != null)
            {
                string fips = Mappings.StateAbbreviationToFips(address.AdminDistrict);

                if (string.IsNullOrEmpty(fips))
                {
                    return false;
                }

                string requestUri = string.Format("get=P0010001&for=zip+code+tabulation+area:{0}&in=state:{1}", address.PostalCode, fips);

                var array = await census.GetCensusData(requestUri);

                if (array == null)
                {
                    return false;
                }

                int count;

                if (int.TryParse(array[1][0].ToString(), out count))
                {
                    Items.Add(new PopulatedZipCode(new UsState(fips, string.Format("{0} ({1})", address.Locality, address.PostalCode), location.Latitude, location.Longitude))
                        {
                            Population = count
                        }
                        );
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public async Task LoadStateDataOneByOne()
        {
            try
            {
                Dictionary<string, UsState> statesList = Mappings.GetStatesList();

                foreach (var usState in statesList)
                {
                    string requestUri = string.Format("get=P0010001,NAME&for=state:{0}", usState.Key);
                    var array = await census.GetCensusData(requestUri);

                    if (array == null) return;

                    foreach (var item in array)
                    {
                        string fips = item[2].ToString();
                        if (statesList.ContainsKey(fips))
                        {
                            UsState state = statesList[fips];
                            int count;

                            if (int.TryParse(item[0].ToString(), out count))
                            {
                                Items.Add(new PopulatedState(state) {Population = count});
                            }
                        }
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
                Dictionary<string, UsState> statesList = Mappings.GetStatesList();

                string requestUri = string.Format("get=P0010001,NAME&for=state:*", censusApiKey);
                var array = await census.GetCensusData(requestUri);

                if (array == null) return;

                foreach (var item in array)
                {
                    AddStateItem(item, statesList);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
        }

        private void AddStateItem(JToken item, Dictionary<string, UsState> statesList)
        {
            string fips = item[2].ToString();
            if (statesList.ContainsKey(fips))
            {
                UsState state = statesList[fips];
                int count;

                if (int.TryParse(item[0].ToString(), out count))
                {
                    Items.Add(new PopulatedState(state) {Population = count});
                }
            }
        }
    }
}
