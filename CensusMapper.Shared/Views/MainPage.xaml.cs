using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;

using CensusMapper.ViewModels;

#if WINDOWS_APP
using Bing.Maps;
using Windows.UI.Xaml.Navigation;
#endif

#if WINDOWS_PHONE_APP
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
#endif


namespace CensusMapper
{
    public partial class MainPage
    {
        const double InitialZoomLevel = 5.0;

        private string keyCensus = "";
        private string keyBingMaps = "";

        private Census census = null;

        private IList<Geopoint> locations;

        private Geopoint centerOfUs = new Geopoint(new BasicGeoposition() { Latitude = 39.833333, Longitude = -98.583333 });

        private void InitializeApis()
        {
            locations = new List<Geopoint>();

            SetApiKeys();

            census = new Census(keyCensus);
        }

        private void SetApiKeys()
        {
            //keyCensus = "ABC";
            //keyBingMaps = "DEF";

            SetApiCredentials();
        }

        async void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // Save User Data

            deferral.Complete();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            CenterMap();

            //await LoadAndDisplayStateData();
        }

        private async Task LoadAndDisplayStateData()
        {
            try
            {                
                Dictionary<string, UsState> statesList = census.GetStatesList();

                var array = await census.GetPopulationForAllStates();

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
                            InsertStatePushpin(state, count);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
        }

        private void InsertStatePushpin(UsState state, int count)
        {
            var ctrl = new ContentControl();
            ctrl.Template = Application.Current.Resources["StateTemplate"] as ControlTemplate;
            ctrl.DataContext = new PopulatedState(state) { Population = count };

            InsertContentTemplateAtLocation(new Coordinates() { Latitude = state.Center.Latitude, Longitude = state.Center.Longitude }, ctrl);
        }

        private async Task<bool> AddPushPinAtLocation(Coordinates location)
        {
            BingMaps bingMaps = new BingMaps(keyBingMaps);
            Address address = await bingMaps.GetAddress(location);

            return await AddPushPin(address, location);
        }

        private void InsertContentTemplateAtLocation(Coordinates location, ContentControl ctrl)
        {
            #if WINDOWS_PHONE_APP
                map.Children.Add(ctrl);
                MapControl.SetLocation(ctrl, new Geopoint(new BasicGeoposition() { Latitude = location.Latitude, Longitude = location.Longitude }));
                MapControl.SetNormalizedAnchorPoint(ctrl, new Windows.Foundation.Point(0.0, 0.0));
            #endif

            #if WINDOWS_APP        
                MapLayer.SetPosition(ctrl, new Location(location.Latitude, location.Longitude));
                map.Children.Add(ctrl);        
            #endif
        }

        private async Task<bool> AddPushPin(Address address, Coordinates location)
        {
            if (address != null)
            {                            
                var array = await census.GetPopulationForPostalCode(address);

                if (array != null)
                {
                    int count;

                    if (int.TryParse(array[1][0].ToString(), out count))
                    {
                        var ctrl = new ContentControl();
                        ctrl.Template = Application.Current.Resources["ZipCodeTemplate"] as ControlTemplate;

                        InsertContentTemplateAtLocation(location, ctrl);

                        ctrl.DataContext = new
                        {
                            Name = string.Format("{0} ({1})", address.Locality, address.PostalCode),
                            FormattedPopulation = string.Format("{0:0,0}", count)
                        };
                        
                        return true;
                    }
                }
            }

            return false;
        }

        private async Task<string> ReadTextFile(string filename)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(filename);
            var text = await Windows.Storage.FileIO.ReadTextAsync(file);
            return text;
        }
    }
}
