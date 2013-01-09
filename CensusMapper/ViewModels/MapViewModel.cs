using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Bing.Maps;
using CensusMapper.Converters;
using GalaSoft.MvvmLight.Command;

namespace CensusMapper.ViewModels
{
    public class MapViewModel
    {
        private ObservableCollection<MapShapeLayer> _ShapeLayers;
        private ObservableCollection<PopulatedEntity> _items;
        private string bingMapsApiKey;
        private string censusApiKey;

        private BingMaps bingMaps;
        private Census census;

        public MapViewModel(string keyBingMaps, string keyCensus)
        {
            bingMapsApiKey = keyBingMaps;
            censusApiKey = keyCensus;

            bingMaps = new BingMaps(bingMapsApiKey);
            census = new Census(censusApiKey);

            _ShapeLayers = new ObservableCollection<MapShapeLayer>();

            _items = new ObservableCollection<PopulatedEntity>();
        }

        public ObservableCollection<MapShapeLayer> ShapeLayers
        {
            get { return _ShapeLayers; }
            set { _ShapeLayers = value; }
        }

        public ObservableCollection<PopulatedEntity> Items
        {
            get { return _items; }
            set { _items = value; }
        }

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
                    string fips = item[2].ToString();
                    if (statesList.ContainsKey(fips))
                    {
                        UsState state = statesList[fips];
                        int count;

                        if (int.TryParse(item[0].ToString(), out count))
                        {
                            Items.Add(new PopulatedState(state) { Population = count });
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }            
        }
    }
}
