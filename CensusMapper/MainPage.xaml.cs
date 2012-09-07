using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bing.Maps;
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

namespace CensusMapper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const double InitialZoomLevel = 5.0;

        HttpClient client = null;  
      
        private string keyCensus = "";
        private string keyBingMaps = "";

        Geolocator geolocator = null;

        public MainPage()
        {
            this.InitializeComponent();

            SetApiKeys();

            geolocator = new Geolocator();
            geolocator.StatusChanged += geolocator_StatusChanged;

            client = new HttpClient();
            client.BaseAddress = new Uri("http://api.census.gov/data/2010/");
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

                ToggleStatusDisplay();
            }
        }

        private void ToggleStatusDisplay()
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, 
                new Windows.UI.Core.DispatchedHandler(() => 
                {
                    // txtStatus.Text = this.Status;
                    // txtStatus.Visibility = locationEnabled ? Visibility.Collapsed : Visibility.Visible;
                })).AsTask().Wait();            
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

        private void SetApiKeys()
        {
            keyCensus = ApiKeys.CensusApiKey;
            keyBingMaps = ApiKeys.BingMapsKey;
            map.Credentials = keyBingMaps;
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {                                               
                var centerOfUs = new Location(39.833333, -98.583333);

                map.Center = centerOfUs;                
                map.ZoomLevel = InitialZoomLevel;

                Dictionary<string, UsState> statesList = GetStatesList();

                string requestUri = string.Format("sf1?key={0}&get=P0010001,NAME&for=state:*",keyCensus);
                var array = await GetCensusData(requestUri);

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
                            ctrl.DataContext = new PopulatedState(state) {Population = count};
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

        private async Task<JArray> GetCensusData(string requestUri)
        {
            var task = client.GetAsync(requestUri);
            HttpResponseMessage response = await task;
            
            // response.EnsureSuccessStatusCode();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(result);
                return JArray.Parse(result);
            }

            return null;
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

        private Dictionary<string, UsState> GetStatesList()
        {
            var states = new Dictionary<string, UsState>();

            states.Add("01", new UsState("01", "Alabama", 33.001, -86.766233));
            states.Add("02", new UsState("02", "Alaska", 61.288254, -148.716968));

            states.Add("04", new UsState("04", "Arizona", 33.373506, -111.828711));
            states.Add("05", new UsState("05", "Arkansas", 35.080251, -92.576816));
            states.Add("06", new UsState("06", "California", 35.458606, -119.355165));

            states.Add("08", new UsState("08", "Colorado", 39.500656, -105.203628));
            states.Add("09", new UsState("09", "Connecticut", 41.494852, -72.874365));
            states.Add("10", new UsState("10", "Delaware", 39.397164, -75.561908));
            states.Add("11", new UsState("11", "District of Columbia", 38.910092, -77.014001));
            states.Add("12", new UsState("12", "Florida", 27.795850, -81.634622));
            states.Add("13", new UsState("13", "Georgia", 33.332208, -83.868887));

            states.Add("15", new UsState("15", "Hawaii", 21.146768, -157.524452));
            states.Add("16", new UsState("16", "Idaho", 44.242605, -115.133222));
            states.Add("17", new UsState("17", "Illinois", 41.278216, -88.380238));
            states.Add("18", new UsState("18", "Indiana", 40.163935, -86.261515));
            states.Add("19", new UsState("19", "Iowa", 41.960392, -93.049161));
            states.Add("20", new UsState("20", "Kansas", 38.454303, -96.536052));
            states.Add("21", new UsState("21", "Kentucky", 37.808159, -85.241819));
            states.Add("22", new UsState("22", "Louisiana", 30.699270, -91.457133));
            states.Add("23", new UsState("23", "Maine", 44.313614, -69.719931));
            states.Add("24", new UsState("24", "Maryland", 39.145653, -76.797396));
            states.Add("25", new UsState("25", "Massachusetts", 42.271831, -71.363628));
            states.Add("26", new UsState("26", "Michigan", 42.866412, -84.170753));
            states.Add("27", new UsState("27", "Minnesota", 45.210782, -93.583003));
            states.Add("28", new UsState("28", "Mississippi", 32.566420, -89.593164));
            states.Add("29", new UsState("29", "Missouri", 38.437715, -92.153770));
            states.Add("30", new UsState("30", "Montana", 46.813302, -111.209708));
            states.Add("31", new UsState("31", "Nebraska", 41.183753, -97.403875));
            states.Add("32", new UsState("32", "Nevada", 37.165965, -116.304648));
            states.Add("33", new UsState("33", "New Hampshire", 43.153046, -71.463342));
            states.Add("34", new UsState("34", "New Jersy", 40.438458, -74.428055));
            states.Add("35", new UsState("35", "New Mexico", 34.623012, -106.342108));
            states.Add("36", new UsState("36", "New York", 41.507548, -74.645228));
            states.Add("37", new UsState("37", "North Carolina", 35.553334, -79.667654));
            states.Add("38", new UsState("38", "North Dakota", 47.375168, -99.334736));
            states.Add("39", new UsState("39", "Ohio", 40.480854, -82.749366));
            states.Add("40", new UsState("40", "Oklahoma", 35.597940, -96.834653));
            states.Add("41", new UsState("41", "Oregon", 44.732273, -122.579524));
            states.Add("42", new UsState("42", "Pennsylvania", 40.463528, -77.075925));
            
            states.Add("44", new UsState("44", "Rhode Island", 41.753318, -71.448902));
            states.Add("45", new UsState("45", "South Carolina", 34.034551, -81.032387));
            states.Add("46", new UsState("46", "South Dakota", 44.047502, -99.043799));
            states.Add("47", new UsState("47", "Tennessee", 35.795862, -86.397772));
            states.Add("48", new UsState("48", "Texas", 30.943149, -97.388631));
            states.Add("49", new UsState("49", "Utah", 40.438987, -111.900160));
            states.Add("50", new UsState("50", "Vermont", 44.081127, -72.814309));
            states.Add("51", new UsState("51", "Virginia", 37.750345, -77.835857));
            
            states.Add("53", new UsState("53", "Washington", 47.341728, -121.624501));
            states.Add("54", new UsState("54", "West Virginia", 38.767195, -80.820221));
            states.Add("55", new UsState("55", "Wisconsin", 43.728544, -89.001006));
            states.Add("56", new UsState("56", "Wyoming", 42.675762, -107.008835));

            return states;
        }

        private async void map_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var map = sender as Map;

            var pos = e.GetPosition(map);
            Location location;
            map.TryPixelToLocation(pos, out location);

            // AddPushPin(map, location);

            var ctrl = new ContentControl();
            ctrl.Template = Application.Current.Resources["ZipCodeTemplate"] as ControlTemplate;

            MapLayer.SetPosition(ctrl, location);
            map.Children.Add(ctrl);

            BingMaps bingMaps = new BingMaps(keyBingMaps);
            Address address = await bingMaps.GetAddress(location);
           
            AddPushPin(map, address, location, ctrl);

        }

        private async void AddPushPin(Map map, Address address, Location location, ContentControl ctrl)
        {
            if (address != null)
            {
                /// http://api.census.gov/data/2010/sf1?get=P0010001&for=zip+code+tabulation+area:99501&in=state:02

                string fips = StateAbbreviationToFips(address.AdminDistrict);

                if (string.IsNullOrEmpty(fips))
                {
                    RemovePushpin(ctrl);
                    return;
                }

                string requestUri = string.Format("sf1?key={0}&get=P0010001&for=zip+code+tabulation+area:{1}&in=state:{2}", keyCensus, address.PostalCode, fips);
                var array = await GetCensusData(requestUri);

                if (array == null)
                {
                    RemovePushpin(ctrl);
                    return;
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
                }
            }
            else
            {
                RemovePushpin(ctrl);
            }
        }

        private void RemovePushpin(ContentControl ctrl)
        {
            map.Children.Remove(ctrl);     
        }

        private string StateAbbreviationToFips(string abbreviation)
        {
            string name = StateAbbreviationToName(abbreviation);
            if (string.IsNullOrEmpty(name)) return string.Empty;

            var states = GetStatesList();
            var state = states.FirstOrDefault(s => s.Value.Name.ToLowerInvariant().CompareTo(name.ToLowerInvariant()) == 0);

            if (string.IsNullOrEmpty(state.Key)) return string.Empty;

            return state.Key;
        }

        private string StateAbbreviationToName(string abbreviation)
        {
            Dictionary<string, string> states = new Dictionary<string, string>();

            states.Add("AL", "Alabama");
            states.Add("AK", "Alaska");
            states.Add("AZ", "Arizona");
            states.Add("AR", "Arkansas");
            states.Add("CA", "California");
            states.Add("CO", "Colorado");
            states.Add("CT", "Connecticut");
            states.Add("DE", "Delaware");
            states.Add("DC", "District of Columbia");
            states.Add("FL", "Florida");
            states.Add("GA", "Georgia");
            states.Add("HI", "Hawaii");
            states.Add("ID", "Idaho");
            states.Add("IL", "Illinois");
            states.Add("IN", "Indiana");
            states.Add("IA", "Iowa");
            states.Add("KS", "Kansas");
            states.Add("KY", "Kentucky");
            states.Add("LA", "Louisiana");
            states.Add("ME", "Maine");
            states.Add("MD", "Maryland");
            states.Add("MA", "Massachusetts");
            states.Add("MI", "Michigan");
            states.Add("MN", "Minnesota");
            states.Add("MS", "Mississippi");
            states.Add("MO", "Missouri");
            states.Add("MT", "Montana");
            states.Add("NE", "Nebraska");
            states.Add("NV", "Nevada");
            states.Add("NH", "New Hampshire");
            states.Add("NJ", "New Jersey");
            states.Add("NM", "New Mexico");
            states.Add("NY", "New York");
            states.Add("NC", "North Carolina");
            states.Add("ND", "North Dakota");
            states.Add("OH", "Ohio");
            states.Add("OK", "Oklahoma");
            states.Add("OR", "Oregon");
            states.Add("PA", "Pennsylvania");
            states.Add("RI", "Rhode Island");
            states.Add("SC", "South Carolina");
            states.Add("SD", "South Dakota");
            states.Add("TN", "Tennessee");
            states.Add("TX", "Texas");
            states.Add("UT", "Utah");
            states.Add("VT", "Vermont");
            states.Add("VA", "Virginia");
            states.Add("WA", "Washington");
            states.Add("WV", "West Virginia");
            states.Add("WI", "Wisconsin");
            states.Add("WY", "Wyoming");
            if (states.ContainsKey(abbreviation))
                return (states[abbreviation]);
            
            return string.Empty;
        }      
    }
}
