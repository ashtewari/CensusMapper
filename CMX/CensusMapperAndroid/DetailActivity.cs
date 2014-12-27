
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

namespace CensusMapperAndroid
{
	[Activity (Label = "Location Information")]			
	public class DetailActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			var payload = Intent.GetStringExtra("LocationInfo");
			if (string.IsNullOrWhiteSpace (payload) == false) {
				var info = JsonConvert.DeserializeObject<LocationInformation> (payload);
				Log.Info("Info", string.Format ("{0}", info.PostalCode));
			}
		}
	}
}

