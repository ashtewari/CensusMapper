using System;

namespace CensusMapper
{
	public class ApiKeyService
	{
		private const string censusApiKey = "11174650921ed8c072d8ca2441e44475740f50c1";
		private const string bingMapsKey = "AjFaZyzK0GB6IrfCv0VQ7YJBj9vL9o1hpOWTDwpo-wS-61QEFM17jeRBA05GrpO5";

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

