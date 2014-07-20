using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

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

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void SetApiCredentials()
        {
            map.MapServiceToken = keyBingMaps;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            await LoadAndDisplayStateData();

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void CenterMap()
        {
            map.Center = centerOfUs;
            map.ZoomLevel = InitialZoomLevel;            
        }

        private async void map_Tapped_1(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            bool result = await AddPushPinAtLocation(args.Location);

            if (result)
            {
                locations.Add(args.Location);
            }
        }

        private void InsertContentTemplateAtLocation(Geopoint location, ContentControl ctrl)
        {
            map.Children.Add(ctrl);

            MapControl.SetLocation(ctrl, location);
            MapControl.SetNormalizedAnchorPoint(ctrl, new Windows.Foundation.Point(0.5, 0.5));            
        }

        private void RemovePushpin(ContentControl ctrl)
        {
            map.Children.Remove(ctrl);
        }
    }
}
