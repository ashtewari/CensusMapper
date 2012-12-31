using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bing.Maps;
using CensusMapper.Converters;
using Newtonsoft.Json.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

using System.Xml;
using System.Xml.Linq;
using System.Reflection;

using Windows.UI.Popups;
using Newtonsoft.Json;

namespace CensusMapper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string API_KEYS_FILE = "ApiKeys.xml";
        const double InitialZoomLevel = 5.0;

        private double currentZoomLevel = 5.0;
      
        private string keyCensus = "";
        private string keyBingMaps = "";
        private string keyAzureMobile = "";

        Census census = null;

        Geolocator geolocator = null;

        private IList<Location> locations;
        private CountyFips counties = null;

        public MainPage()
        {
            App.Current.Suspending += Current_Suspending;
            this.InitializeComponent();
            this.InitializeApis();

            locations = new List<Location>();
        }

        async void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await SaveUserData();

            deferral.Complete();
        }

        private void InitializeApis()
        {
            SetApiKeys();

            geolocator = new Geolocator();
            geolocator.StatusChanged += geolocator_StatusChanged;

            census = new Census(keyCensus);
        }

        private string _status = "";
        private bool locationEnabled = false;

        private string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
            }
        }

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            locationEnabled = false;
            switch (args.Status)
            {
                case Windows.Devices.Geolocation.PositionStatus.Ready:
                    // Location data is available
                    Status = "Location is available.";
                    locationEnabled = true;
                    break;
                case Windows.Devices.Geolocation.PositionStatus.Initializing:
                    // This status indicates that a GPS is still acquiring a fix
                    Status =  "A GPS device is still initializing.";
                    break;
                case Windows.Devices.Geolocation.PositionStatus.NoData:
                    // No location data is currently available
                    Status = "Data from location services is currently unavailable.";
                    break;
                case Windows.Devices.Geolocation.PositionStatus.Disabled:
                    // The app doesn't have permission to access location,
                    // either because location has been turned off.
                    Status = "Your location is currently turned off. " +
                        "Change your settings through the Settings charm " +
                        " to turn it back on.";
                    locationEnabled = false;
                    break;
                case Windows.Devices.Geolocation.PositionStatus.NotInitialized:
                    // This status indicates that the app has not yet requested
                    // location data by calling GetGeolocationAsync() or
                    // registering an event handler for the positionChanged event.
                    Status = "Location status is not initialized because " +
                                  "the app has not requested location data.";
                    locationEnabled = false;
                    break;
                case Windows.Devices.Geolocation.PositionStatus.NotAvailable:
                    // Location is not available on this version of Windows
                    Status = "You do not have the required location services " +
                        "present on your system.";
                    locationEnabled = false;
                    break;
                default:
                    Status = "Unknown status";
                    break;
            }
            
        }

        private async Task<string> ReadTextFile(string filename)
        {                       
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(filename);                         
            var text = await Windows.Storage.FileIO.ReadTextAsync(file);
            return text;
        }

        private void SetApiKeys()
        {            
            string fileName = API_KEYS_FILE;

            var data = XDocument.Load(System.IO.Path.Combine(fileName));
            var keys = data.Descendants("Keys").First();

            var census = from key in keys.Elements("Key") where key.Attribute("name").Value == "Census" select key.Attribute("value").Value;
            var bing = from key in keys.Elements("Key") where key.Attribute("name").Value == "Bing" select key.Attribute("value").Value;
            var azureMobile = from key in keys.Elements("Key") where key.Attribute("name").Value == "AzureMobile" select key.Attribute("value").Value;                              

            keyCensus = census.ElementAt(0);
            keyBingMaps = bing.ElementAt(0);
            keyAzureMobile = azureMobile.ElementAt(0);

            map.Credentials = keyBingMaps;
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadAndDisplayStateData();
        }

        private async Task LoadCountyData()
        {
            string countyFipsJson = await ReadTextFile("CountyFips.json.txt");
            counties = await JsonConvert.DeserializeObjectAsync<CountyFips>(countyFipsJson);
        }

        private async Task LoadAndDisplayStateData()
        {
            try
            {
                var centerOfUs = new Location(39.833333, -98.583333);

                map.Center = centerOfUs;
                map.ZoomLevel = InitialZoomLevel;

                Dictionary<string, UsState> statesList = Mappings.GetStatesList();

                string requestUri = string.Format("get=P0010001,NAME&for=state:*", keyCensus);
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
                            var ctrl = new ContentControl();
                            ctrl.Template = Application.Current.Resources["StateTemplate"] as ControlTemplate;
                            ctrl.DataContext = new PopulatedState(state) { Population = count };
                            MapLayer.SetPosition(ctrl, state.Center);
                            map.Children.Add(ctrl);
                        }
                    }
                }

                SetCurrentLocation();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
        }

        private async Task SaveUserData()
        {
            if (locations.Count > 0)
            {
                var locs = new List<string>();
                foreach (var loc in locations)
                {
                    locs.Add(string.Format("{0},{1}", loc.Latitude, loc.Longitude));
                }
            }
        }

        internal static void AddPushPin(Map map, Location location)
        {
            Pushpin pin = new Pushpin();
            MapLayer.SetPosition(pin, location);
            map.Children.Add(pin);        
        }

        private async void SetCurrentLocation()
        {
            if (!locationEnabled) return;

            var pos = await geolocator.GetGeopositionAsync();
            Location location = new Location(pos.Coordinate.Latitude, pos.Coordinate.Longitude);            
            map.SetView(location, 6.0f);

            AddPushPin(map, location);            
        }

        internal void AddExtendedPushPin(Map map, Address address, Location location)
        {
            var ctrl = new ContentControl();
            ctrl.Template = Application.Current.Resources["PushpinTemplate"] as ControlTemplate;
            ctrl.DataContext = new { Name = string.Format("{0}", address.Locality) };
            MapLayer.SetPosition(ctrl, location);
            map.Children.Add(ctrl);
        }

        private async void map_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var map = sender as Map;

            var pos = e.GetPosition(map);
            Location location;
            map.TryPixelToLocation(pos, out location);

            bool result = await AddPushPinAtLocation(map, location);

            if (result)
            {
                locations.Add(location);
            }
        }

        private async Task<bool> AddPushPinAtLocation(Map map, Location location)
        {
            var ctrl = new ContentControl();
            ctrl.Template = Application.Current.Resources["ZipCodeTemplate"] as ControlTemplate;

            MapLayer.SetPosition(ctrl, location);
            map.Children.Add(ctrl);

            BingMaps bingMaps = new BingMaps(keyBingMaps);
            Address address = await bingMaps.GetAddress(location);

            return await AddPushPin(map, address, location, ctrl);
        }

        private async Task<bool> AddPushPin(Map map, Address address, Location location, ContentControl ctrl)
        {
            if (address != null)
            {
                string fips = Mappings.StateAbbreviationToFips(address.AdminDistrict);

                if (string.IsNullOrEmpty(fips))
                {
                    RemovePushpin(ctrl);
                    return false;
                }

                string requestUri = string.Format("get=P0010001&for=zip+code+tabulation+area:{0}&in=state:{1}", address.PostalCode, fips);
            
                var array = await census.GetCensusData(requestUri);

                if (array == null)
                {
                    RemovePushpin(ctrl);
                    return false;
                }

                int count;

                if (int.TryParse(array[1][0].ToString(), out count))
                {
                    ctrl.DataContext = new
                    {
                        Name = string.Format("{0} ({1})", address.Locality, address.PostalCode),
                        FormattedPopulation = string.Format("{0:0,0}", count)
                    };
                }
                else
                {
                    RemovePushpin(ctrl);
                    return false;
                }
            }
            else
            {
                RemovePushpin(ctrl);
                return false;
            }

            return true;
        }

        private void RemovePushpin(ContentControl ctrl)
        {
            map.Children.Remove(ctrl);     
        }

        private void map_ViewChanged_1(object sender, ViewChangedEventArgs e)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low,
                new Windows.UI.Core.DispatchedHandler(() =>
                {
                    //this.txtZoom.Text = this.map.ZoomLevel.ToString();
                }));              
            System.Diagnostics.Debug.WriteLine("{0}", this.map.ZoomLevel);

            currentZoomLevel = this.map.ZoomLevel;

            // > 5 USA
            // 5 - 10 State
            // 10 - 12 County
            // 12 > City
        }

        private async void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            await SaveUserData();  
        }
    }
}
