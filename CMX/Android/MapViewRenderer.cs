using System;
using Xamarin.Forms.Maps.Android;
using Android.Gms.Maps;
using Xamarin.Forms;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Maps;
using Android.Content;
using Android.Graphics;
using System.Collections.Generic;

[assembly: ExportRenderer (typeof(CensusMapper.CensusMap), typeof(CensusMapper.Android.MapViewRenderer))]
namespace CensusMapper.Android
{
	public class MapViewRenderer : MapRenderer
	{

		bool _isDrawnDone;

		CensusMap _customMap { get { return this.Element as CensusMap; } }

		Grid _customMapGrid { get { return _customMap.Parent as Grid; } }

		//CustomMapContentView _customMapContentView { get { return _customMap.Parent.Parent as CustomMapContentView; } }

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			var androidMapView = (MapView)Control;

			if (e.PropertyName.Equals ("VisibleRegion") && !_isDrawnDone) {
				androidMapView.Map.Clear ();

				androidMapView.Map.MapClick += HandleMapClick;
				androidMapView.Map.MyLocationEnabled = _customMap.IsShowingUser;

				//The footer overlays the zoom controls
				androidMapView.Map.UiSettings.ZoomControlsEnabled = true;

//				IList<Pin> formsPins = _customMap.Pins;
//
//				//IList<Pin> formsPins = new List<Pin> ();
//				//foreach (var formsPin in formsPins) {
//				//	formsPins.Add (formsPin);
//				//}
//
//				//_customMap.Pins.Clear();
//
//				foreach (var formsPin in formsPins) {
//					var markerWithIcon = new MarkerOptions ();
//
//					markerWithIcon.SetPosition (new LatLng (formsPin.Position.Latitude, formsPin.Position.Longitude));
//					//markerWithIcon.SetTitle (formsPin.Label);
//					//markerWithIcon.SetSnippet (formsPin.Address);
//
////					if (!string.IsNullOrEmpty ("Logo"))
////						markerWithIcon.InvokeIcon (BitmapDescriptorFactory.FromAsset (String.Format ("{0}.png", "Logo")));
////					else
////						markerWithIcon.InvokeIcon (BitmapDescriptorFactory.DefaultMarker ());
//
//					markerWithIcon.InvokeIcon (CreateCensusMarker(formsPin));
//					androidMapView.Map.AddMarker (markerWithIcon);
//				}

				_isDrawnDone = true;
			}
		}

		Marker _previouslySelectedMarker {
			get;
			set;
		}

		private BitmapDescriptor CreateDefaultMarker()
		{
			return BitmapDescriptorFactory.DefaultMarker ();
		}

		private BitmapDescriptor CreateCensusMarker(Pin pin)
		{
			Paint color = new Paint();
			color.TextSize = 18;
			color.Color = global::Android.Graphics.Color.Black;

			Bitmap bmp = Bitmap.CreateBitmap(200, 100, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas(bmp);

			canvas.DrawText (pin.Label, 30, 40, color);

			return BitmapDescriptorFactory.FromBitmap (bmp);
		}
			

		void HandleMapClick (object sender, GoogleMap.MapClickEventArgs e)
		{
			ResetPrevioslySelectedMarker ();
		}

		void ResetPrevioslySelectedMarker ()
		{
			//todo : This should reset to the default icon for the pin (right now the icon is hard coded)
			if (_previouslySelectedMarker != null) {
				//_previouslySelectedMarker.SetIcon (BitmapDescriptorFactory.FromAsset (String.Format ("{0}.png", "Logo"))); 
				_previouslySelectedMarker = null;
			}
		}
	}
}