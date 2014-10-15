using System;
using Xamarin.Forms;

namespace CensusMapper
{
	public class App
	{
		public static Page GetMainPage ()
		{	
			return new MapPage ();
		}
	}
}

