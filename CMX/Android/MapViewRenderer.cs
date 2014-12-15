using System;
using Xamarin.Forms.Maps.Android;
using Android.Gms.Maps;
using Xamarin.Forms;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Maps;
using Android.Content;
using Android.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

[assembly: ExportRenderer (typeof(CensusMapper.CensusMap), typeof(CensusMapper.Android.MapViewRenderer))]
namespace CensusMapper.Android
{
	public class MapViewRenderer : MapRenderer
	{

		bool _isDrawnDone = false;

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

				INotifyCollectionChanged notifyCollectionChanged = _customMap.Pins as INotifyCollectionChanged;
				if (notifyCollectionChanged != null) {
					notifyCollectionChanged.CollectionChanged += (new NotifyCollectionChangedEventHandler (this.OnCollectionChanged));
				}													

				//				IList<Pin> formsPins = _customMap.Pins;
				//
				//				IList<Pin> formsPins = new List<Pin> ();
				//				foreach (var formsPin in _customMap.Pins) {
				//					formsPins.Add (formsPin);
				//				}			
				//				_customMap.Pins.Clear();
				//

				_isDrawnDone = true;
			}
		}

		IList<MarkerOptions> markers = new List<MarkerOptions>();

		private void OnCollectionChanged (object sender, NotifyCollectionChangedEventArgs args)
		{
			var androidMapView = (MapView)Control;

			foreach (Pin formsPin in args.NewItems) {
				var markerWithIcon = new MarkerOptions ();

				markerWithIcon.SetPosition (new LatLng (formsPin.Position.Latitude, formsPin.Position.Longitude));
				//markerWithIcon.SetTitle (formsPin.Label);
				//markerWithIcon.SetSnippet (formsPin.Address);


				//markerWithIcon.InvokeIcon (BitmapDescriptorFactory.FromResource (Resource.Drawable.pin));

//				if (!string.IsNullOrEmpty ("Logo"))
//					markerWithIcon.InvokeIcon (BitmapDescriptorFactory.FromAsset (String.Format ("{0}.png", "pin.9")));
//				else
//					markerWithIcon.InvokeIcon (BitmapDescriptorFactory.DefaultMarker ());

				markerWithIcon.InvokeIcon (CreateCensusMarker(formsPin));
				markers.Add (markerWithIcon);

				//androidMapView.Map.AddMarker(markerWithIcon);

			}

						androidMapView.Map.Clear ();
						foreach (MarkerOptions marker in markers) {
							androidMapView.Map.AddMarker(marker);
						}

		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);

			var targetElement = (MapView)Control;

			//NOTIFY CHANGE
			if (changed)
			{
				_isDrawnDone = false;
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

			Bitmap bmp = Bitmap.CreateBitmap(200, 100, Bitmap.Config.Rgb565);

			var options = new BitmapFactory.Options {
				InJustDecodeBounds = true,
			};
			// BitmapFactory.DecodeResource() will return a non-null value; dispose of it.
//			using (var bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.pin, options)) {
//				Canvas canvas = new Canvas(bmp);
//				canvas.DrawText (pin.Label, 30, 40, color);
//				return BitmapDescriptorFactory.FromBitmap (bmp);
//			}

			//Bitmap bmp = BitmapFactory.DecodeResource (Resources, Resource.Drawable.Logo);
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