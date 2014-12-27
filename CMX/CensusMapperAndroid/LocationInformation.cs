using System;
using CensusMapper;

namespace CensusMapperAndroid
{
	[Serializable]
	public class LocationInformation
	{
		public LocationInformation ()
		{
		}

		public int PostalCodePopulation {
			get;
			set;
		}

		public int StatePopulation {
			get;
			set;
		}

		public string PostalCode {
			get;
			set;
		}

		public string State {
			get;
			set;
		}
	}
}

