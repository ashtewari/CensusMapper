using System;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Animation;

namespace CensusMapperAndroid
{
	class LatLngEvaluator : Java.Lang.Object, ITypeEvaluator
	{
		public Java.Lang.Object Evaluate (float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue)
		{
			var start = (LatLng)startValue;
			var end = (LatLng)endValue;
			return new LatLng (start.Latitude + fraction * (end.Latitude - start.Latitude),
				start.Longitude + fraction * (end.Longitude - start.Longitude));
		}
	}
}

