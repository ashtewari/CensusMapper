Windows 8 Metro App to visualize Census data on Bing maps.

* Windows 8
* WinRT
* Metro
* C#
* Census API
* Bing Maps
* JSON.Net

Before compiling - 

1. Get your Census Api key here - http://www.census.gov/developers/tos/key_request.html
2. Get your Bing Maps API key here - http://msdn.microsoft.com/en-us/library/ff428642.aspx
3. Add a file ApiKeys.cs to CensusMapper project with the following content. 
       
// ******************* ApiKeys.cs Begin **************************************    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper
{
    internal static class ApiKeys
    {	
        public const string CensusApiKey = YOUR_CENSUS_API_Key;	
        public const string BingMapsKey = YOUR_BING_MAPS_API_Key;            
    }
}

// ******************* ApiKeys.cs End **************************************

4. Replace the following values in ApiKeys.cs with your own keys -
YOUR_CENSUS_API_Key; 
YOUR_BING_MAPS_API_Key; 
