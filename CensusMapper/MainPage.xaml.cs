using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bing.Maps;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
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
        public MainPage()
        {
            App.Current.Suspending += Current_Suspending;

            this.InitializeComponent();
            this.InitializeApis();
        }

        private void SetApiCredentials()
        {
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

        private void CenterMap()
        {
            map.Center = new Location(centerOfUs.Position.Latitude, centerOfUs.Position.Longitude);
            map.ZoomLevel = InitialZoomLevel;
        }

        private async void map_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var map = sender as Map;

            var pos = e.GetPosition(map);
            Location location;
            map.TryPixelToLocation(pos, out location);

            bool result = await AddPushPinAtLocation(new Geopoint(new BasicGeoposition() { Latitude = location.Latitude, Longitude = location.Longitude}));

            if (result)
            {
                locations.Add(new Geopoint(new BasicGeoposition() { Latitude = location.Latitude, Longitude = location.Longitude }));
            }
        }

        private void InsertContentTemplateAtLocation(Geopoint location, ContentControl ctrl)
        {
            MapLayer.SetPosition(ctrl, new Location(location.Position.Latitude, location.Position.Longitude));
            map.Children.Add(ctrl);
        }
    }
}
