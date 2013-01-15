using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bing.Maps;
using BingMapMVVM;
using CensusMapper.Services;
using CensusMapper.ViewModels;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace CensusMapper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Geolocator geolocator = null;

        public MainPage()
        {            
            App.Current.Suspending += Current_Suspending;            

            this.InitializeComponent();

            var keyService = new ApiKeyProvider();

            this.InitializeApis(keyService);

            var bingMapsApi = new BingMapsApi(keyService);
            var censusApi = new CensusApi(keyService);

            this.DataContext = new MainViewModel(bingMapsApi, censusApi);
        }

        async void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            //// SaveUserData();

            deferral.Complete();
        }

        private void InitializeApis(IApiKeyProvider keyService)
        {
            map.Credentials = keyService.BingMapsKey;
            geolocator = new Geolocator();
            geolocator.StatusChanged += geolocator_StatusChanged;          
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {            
            SetCurrentLocation();
        }

        private async void SetCurrentLocation()
        {
            if (!locationEnabled) return;

            var pos = await geolocator.GetGeopositionAsync();
            var location = new Location(pos.Coordinate.Latitude, pos.Coordinate.Longitude);            

            await SetLocation(map, location);
        }

        private async void map_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var bm = sender as BindableMap;

            if (bm == null) return;

            var pos = e.GetPosition(bm);

            Location location;

            if (bm.TryPixelToLocation(pos, out location))
            {
                await SetLocation(bm, location);
            }
        }

        private async Task SetLocation(BindableMap bm, Location location)
        {
            var vm = bm.DataContext as MapViewModel;

            if (vm == null) return;

            await vm.SelectLocation(new Coordinates(location.Latitude, location.Longitude));
        }

        // > 5 USA
        // 5 - 10 State
        // 10 - 12 County
        // 12 > City
    }
}
