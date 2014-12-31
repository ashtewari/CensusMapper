using System;

namespace CensusMapper
{
	public class PopulationFormatter
	{
		public static string Convert (object value)
		{
			if (value == null) {
				return string.Empty;
			}

			long population;
			if (Int64.TryParse (value.ToString(), out population)) {
				return population.ToString("N0");
			}

			return string.Empty;
		}
	}
}

