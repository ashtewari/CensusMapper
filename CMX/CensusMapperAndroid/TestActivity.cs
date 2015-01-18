
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
using Newtonsoft.Json;

namespace CensusMapperAndroid
{
	[Activity (Label = "Test Activity", MainLauncher = true, Icon = "@drawable/logo")]			
	public class TestActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			var detailActivity = new Intent (this, typeof(DetailActivity));

			var info = new LocationInformation () { GroupName = "California", GroupCount = 568985458, ItemName = "90210", ItemCount = 345825 };
			var payload = JsonConvert.SerializeObject (info);

			detailActivity.PutExtra ("LocationInfo", payload);
			StartActivity (detailActivity);
		}
	}
}

