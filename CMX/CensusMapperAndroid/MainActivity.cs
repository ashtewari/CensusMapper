using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;

using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

using CensusMapper;
using Android.Animation;
using Java.IO;
using Newtonsoft.Json;

namespace CensusMapperAndroid
{
	[Activity (Label = "Census Mapper", Icon = "@drawable/logo")]
	public class MainActivity : Activity
	{	
		private ApiKeyService keys = new  ApiKeyService();

		private LatLng raleighNC = new LatLng(35.772096000000000000, -78.638614500000020000);
		private LatLng centerOfUs = new LatLng(39.828127, -98.579404);

		MapFragment mf;
		GoogleMap _map;

		public static readonly int InstallGooglePlayServicesId = 1000;
		private bool _isGooglePlayServicesInstalled;

		private IDictionary<string, LocationInformation> locations = new Dictionary<string, LocationInformation>();
		private IDictionary<string, int> stateInformation = new Dictionary<string, int>();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			_isGooglePlayServicesInstalled = TestIfGooglePlayServicesIsInstalled ();

			SetContentView(Resource.Layout.Main);

			if (_isGooglePlayServicesInstalled) {
				InitMapFragment();
			}
		}

		protected async override void OnResume ()
		{
			base.OnResume();
			await SetupMapIfNeeded();		
		}
			
		private void InitMapFragment()
		{
			mf = FragmentManager.FindFragmentByTag("map") as MapFragment;
			if (mf == null)
			{
				GoogleMapOptions mapOptions = new GoogleMapOptions()
					.InvokeMapType(GoogleMap.MapTypeHybrid)
					.InvokeZoomControlsEnabled(true)
					.InvokeCompassEnabled(true);

				FragmentTransaction fragTx = FragmentManager.BeginTransaction();
				mf = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.Id.map, mf, "map");
				fragTx.Commit();
			}
		}

		private async Task SetupMapIfNeeded()
		{
			if (_map == null)
			{
				_map = mf.Map;
				if (_map != null)
				{
					CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(centerOfUs, 7);
					_map.MoveCamera(cameraUpdate);

					await LoadStateData ();

					_map.MapClick += HandleMapClick;
					_map.MarkerClick += HandleMarkerClick;
				}
			}
		}

		void HandleMarkerClick (object sender, GoogleMap.MarkerClickEventArgs e)
		{
			// locate LocationInfo
			if (locations.ContainsKey (e.Marker.Id)) {
				// Show details
				var detailActivity = new Intent (this, typeof(DetailActivity));

				var payload = JsonConvert.SerializeObject (locations [e.Marker.Id]);

				detailActivity.PutExtra ("LocationInfo", payload);
				StartActivity (detailActivity);
			}
		}			
			
		// TODO : Animate touching markers, should behave like a button press.
		async void HandleMapClick (object sender, GoogleMap.MapClickEventArgs e)
		{
			LatLng position = new LatLng (e.Point.Latitude, e.Point.Longitude);

			BingMaps bing = new BingMaps (keys.BingMapsKey);
			Address address = await bing.GetAddress (new Coordinates() { Latitude = e.Point.Latitude, Longitude = e.Point.Longitude });

			if (address != null) {
				CensusMapper.Census censusApi = new CensusMapper.Census(keys.CensusKey);
				var population = await censusApi.GetPopulationForPostalCode (address);

				// TODO: Move parsing of CensusApi data to a common service. Should not have to access data like this - population[1][0]
				Marker marker = AddMarker (position, string.Format("Postal Code: {0}\nPopulation: {1}", address.PostalCode, PopulationFormatter.Convert(population[1][0])), Color.Blue, true);

				var stateFipsCode = censusApi.StateAbbreviationToFips (address.AdminDistrict);
				if (stateInformation.ContainsKey (stateFipsCode)) {
					locations.Add (marker.Id, new LocationInformation () {
						GroupName = censusApi.StateAbbreviationToName(address.AdminDistrict),
						GroupCount = stateInformation[stateFipsCode],
						ItemName = address.PostalCode,
						ItemCount = (int)population [1] [0]
					});
				}
			}				
		}			

		private bool TestIfGooglePlayServicesIsInstalled()
		{
			int queryResult = GooglePlayServicesUtil.IsGooglePlayServicesAvailable (this);
			if (queryResult == ConnectionResult.Success)
			{
				Log.Info ("CensusMapper", "Google Play Services is installed on this device.");
				return true;
			}

			if (GooglePlayServicesUtil.IsUserRecoverableError (queryResult)) 
			{
				string errorString = GooglePlayServicesUtil.GetErrorString (queryResult);
				Log.Error ("CensusMapper", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
				Dialog errorDialog = GooglePlayServicesUtil.GetErrorDialog(queryResult, this, InstallGooglePlayServicesId);
				ErrorDialogFragment dialogFrag = new ErrorDialogFragment(errorDialog);

				dialogFrag.Show (FragmentManager, "GooglePlayServicesDialog");
			}
			return false;
		}

		private async Task LoadStateData()
		{
			CensusMapper.Census censusApi = new CensusMapper.Census(keys.CensusKey);

			Dictionary<string, UsState> states = censusApi.GetStatesList();

			var statePop = await censusApi.GetPopulationForAllStates();

			if (statePop == null)
				return;

			foreach (var item in statePop) 
			{
				string fips = item [2].ToString ();
				if (states.ContainsKey (fips)) {
					UsState state = states [fips];

					int count;
					if (int.TryParse (item [0].ToString (), out count)) {
						var marker = AddMarker (new LatLng(state.Center.Latitude, state.Center.Longitude), string.Format("State: {0}\nPopulation: {1}", state.Name, PopulationFormatter.Convert(count)), Color.Brown, false);

						stateInformation.Add (fips, count);

						locations.Add (marker.Id, new LocationInformation () {
							GroupName = "USA",
							GroupCount = 308745538,
							ItemName = state.Name,
							ItemCount = count
						});
					}
				}
			}
		}
			
		// TODO : Save user marked locations; Integrate with Azure Mobile Services
		private Marker AddMarker(LatLng position, string label, Color color, bool withAnimation)
		{
			var point = _map.Projection.ToScreenLocation (position);
			MarkerOptions marker1 = new MarkerOptions();
			marker1.Anchor(0.0f, 1.0f);
			marker1.SetPosition(position);
			marker1.InvokeIcon (CreateCensusMarker(point, label, color));

			var marker = _map.AddMarker(marker1);

			if (withAnimation) {
				var startPoint = point;
				// We will start 35dp above the final position
				startPoint.Offset (0, -35);
				var startPos = _map.Projection.FromScreenLocation (startPoint);

				var evaluator = new LatLngEvaluator ();
				var animator = ObjectAnimator.OfObject (marker, "position", evaluator, startPos, position);
				animator.SetDuration (1000);
				animator.SetInterpolator (new Android.Views.Animations.BounceInterpolator ());
				animator.Start ();
			}	

			return marker;
		}

		private BitmapDescriptor CreateDefaultMarker()
		{
			return BitmapDescriptorFactory.DefaultMarker ();
		}

		private BitmapDescriptor CreateCensusMarker(Android.Graphics.Point point, string label, Color bgColor)
		{
			Paint color = new Paint();
			color.TextSize = 18;
			color.Color = global::Android.Graphics.Color.White;

			Bitmap bmp = Bitmap.CreateBitmap (220, 120, Bitmap.Config.Argb4444);

			var options = new BitmapFactory.Options {
				InJustDecodeBounds = true,
			};

			Canvas canvas = new Canvas(bmp);
			canvas.DrawColor (Color.Transparent);

			Paint tr = new Paint ();
			tr.Color = bgColor;
			tr.SetStyle(Paint.Style.Fill);
			tr.AntiAlias = true;

			Android.Graphics.Point a = new Android.Graphics.Point (0, 60);
			Android.Graphics.Point b = new Android.Graphics.Point (30, 60);
			Android.Graphics.Point c = new Android.Graphics.Point (0, 120);

			Path path = new Path();
			path.MoveTo(a.X, a.Y);
			path.LineTo(b.X, b.Y);
			path.LineTo(c.X, c.Y);
			path.LineTo(a.X, a.Y);
			path.Close();


			canvas.DrawRect (0, 0, 200, 60, tr);
			DrawMultiLineText (label, 10, 20, color, canvas);
			canvas.DrawPath (path, tr);

			return BitmapDescriptorFactory.FromBitmap (bmp);
		}

		private void DrawMultiLineText(String str, float x, float y, Paint paint, Canvas canvas) {
			String[] lines = str.Split('\n');
			float txtSize = -paint.Ascent() + paint.Descent();       

			if (paint.GetStyle() == Paint.Style.FillAndStroke || paint.GetStyle() == Paint.Style.Stroke){
				txtSize += paint.StrokeWidth; //add stroke width to the text size
			}
			float lineSpace = txtSize * 0.2f;  //default line spacing

			for (int i = 0; i < lines.Length; ++i) {
				canvas.DrawText(lines[i], x, y + (txtSize + lineSpace) * i, paint);
			}
		}

		internal class ErrorDialogFragment :DialogFragment
		{
			public ErrorDialogFragment(Dialog dialog) 
			{
				Dialog = dialog;
			}

			public Dialog Dialog { get; private set; }

			public override Dialog OnCreateDialog (Bundle savedInstanceState)
			{
				return Dialog;
			}				
		}
	}		
}