
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Newtonsoft.Json;
using CensusMapper;

namespace CensusMapperAndroid
{
	[Activity (Label = "Location Information")]			
	public class DetailActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.LocationInfo);

			// Create your application here
			var payload = Intent.GetStringExtra("LocationInfo");
			if (string.IsNullOrWhiteSpace (payload) == false) {

				try
				{
					var info = JsonConvert.DeserializeObject<LocationInformation> (payload);

					if (info != null) {
						CreateTable (info);
						CreatePieChart (info);
					} else {
						Log.Debug ("CensusMapper", "payload deserialization returned null.");
					}	

				} catch (Exception ex) {								
					Log.Debug ("CensusMapper", ex.ToString());
				}
			}
		}

		void CreateTable (LocationInformation info)
		{
			var stateLabel = FindViewById<TextView> (Resource.Id.stateLabel);
			var statePop = FindViewById<TextView> (Resource.Id.statePop);
			var zipLabel = FindViewById<TextView> (Resource.Id.zipLabel);
			var zipPop = FindViewById<TextView> (Resource.Id.zipPop);

			stateLabel.Text = info.GroupName;
			statePop.Text = PopulationFormatter.Convert (info.GroupCount);
			zipLabel.Text = string.Format ("{0}", info.ItemName);
			zipPop.Text = PopulationFormatter.Convert (info.ItemCount);
		}

		void CreatePieChart (LocationInformation info)
		{
			var plotview = FindViewById<OxyPlot.XamarinAndroid.PlotView> (Resource.Id.plotview);

			var itemSlice = new OxyPlot.Series.PieSlice (info.ItemName, info.ItemCount) {
				IsExploded = true,
				Fill = OxyPlot.OxyColors.DarkOrange
			};
			var groupSlice = new OxyPlot.Series.PieSlice (info.GroupName, info.GroupCount) {
				Fill = OxyPlot.OxyColors.SkyBlue
			};

			var series = CreateSeries ();
			series.Slices.Add (itemSlice);
			series.Slices.Add (groupSlice);

			var model = new OxyPlot.PlotModel ();
			model.Background = OxyPlot.OxyColors.Transparent;
			model.Series.Add (series);

			plotview.Model = model;
		}

		static OxyPlot.Series.PieSeries CreateSeries ()
		{
			var series = new OxyPlot.Series.PieSeries ();

			series.Diameter = 0.7;
			series.StartAngle = 270;
			series.InsideLabelFormat = null;
			series.OutsideLabelFormat = "{1}, {2:0.##} %";
			series.TickRadialLength = 12;
			series.TextColor = OxyPlot.OxyColors.White;
			series.FontSize = 18;

			return series;
		}
	}
}

