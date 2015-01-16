using System;

namespace CensusMapper
{
	public class ApiKeyService
	{
		private const string censusApiKey = "11174******";
		private const string bingMapsKey = "AjFaZ******";

		public ApiKeyService ()
		{
		}

		public string CensusKey {
			get
			{
				return censusApiKey;
			}
		}

		public string BingMapsKey {
			get
			{
				return bingMapsKey;
			}
		}
	}
}

