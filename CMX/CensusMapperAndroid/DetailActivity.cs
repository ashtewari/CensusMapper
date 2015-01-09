﻿
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
				var info = JsonConvert.DeserializeObject<LocationInformation> (payload);

				var stateLabel = FindViewById<TextView> (Resource.Id.stateLabel);
				var statePop = FindViewById<TextView> (Resource.Id.statePop);
				var zipLabel = FindViewById<TextView> (Resource.Id.zipLabel);
				var zipPop = FindViewById<TextView> (Resource.Id.zipPop);

				stateLabel.Text = info.GroupName;
				statePop.Text = PopulationFormatter.Convert(info.GroupCount);

				zipLabel.Text = string.Format("{0}", info.ItemName);
				zipPop.Text = PopulationFormatter.Convert(info.ItemCount);
			}
		}
	}
}

