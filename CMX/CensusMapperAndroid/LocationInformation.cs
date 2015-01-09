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

		public int ItemCount {
			get;
			set;
		}

		public int GroupCount {
			get;
			set;
		}

		public string ItemName {
			get;
			set;
		}

		public string GroupName {
			get;
			set;
		}
	}
}

