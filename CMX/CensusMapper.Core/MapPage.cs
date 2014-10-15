using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CensusMapper
{
	public class MapPage : ContentPage 
	{
		private const string censusApiKey = "11174650921ed8c072d8ca2441e44475740f50c1";

		private Position raleighNC = new Position(35.772096000000000000, -78.638614500000020000);
		private Position centerOfUs = new Position(39.828127, -98.579404);

		private CensusMap map;

		public MapPage() 
		{
			// 35°46′50″N 78°38′20″W
			map = new CensusMap(
				MapSpan.FromCenterAndRadius(centerOfUs, Distance.FromMiles(1500))) 
			{
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand
			};					

//			var map = new Label () {
//				Text = "Hello, Forms!",
//				VerticalOptions = LayoutOptions.CenterAndExpand,
//				HorizontalOptions = LayoutOptions.CenterAndExpand,
//			};

			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (s, e) => {
				System.Diagnostics.Debug.WriteLine(e.ToString());
			};

			map.GestureRecognizers.Add(tapGestureRecognizer);

			var stack = new StackLayout { Spacing = 0 };
			stack.Children.Add(map);
			Content = stack;
		}	

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			await LoadStateData ();
		}

		private async Task LoadStateData()
		{
			CensusMapper.Census censusApi = new CensusMapper.Census(censusApiKey);

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
						AddPin (new Position(state.Center.Latitude, state.Center.Longitude), string.Format("{0} : {1}", state.Name, count));
					}
				}
			}
		}

		private void AddPin(Position position, string label)
		{
			var pin = new Pin () {
				Position = position,
				Label = label,
			};					

			map.Pins.Add (pin);
		}
	}
}

