using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Bing.Maps;

namespace CensusMapper.Services
{
    public class MockBingMapsApi : IBingMapsApi
    {
        public MockBingMapsApi(IApiKeyProvider keyService)
        {
            
        }

        public async Task<Address> GetAddress(Coordinates location)
        {
            return new Address() {AdminDistrict = "NC", AdminDistrict2 = "", CountryRegion = "", PostalCode = "27501", Locality = "Raleigh", FormattedAddress = "123 First Street"};
        }
    }
}
