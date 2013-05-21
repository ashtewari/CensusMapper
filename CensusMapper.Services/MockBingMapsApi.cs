using System.Threading.Tasks;
using CensusMapper.Models;

namespace CensusMapper.Services
{
    public class MockBingMapsApi : IBingMapsApi
    {
        public MockBingMapsApi(IApiKeyProvider keyService)
        {
            
        }

        public async Task<Address> GetAddress(Location location)
        {
            var result = new Address()
                        {
                            AdminDistrict = "NC",
                            AdminDistrict2 = "",
                            CountryRegion = "",
                            PostalCode = "27501",
                            Locality = "Raleigh",
                            FormattedAddress = "123 First Street"
                        };

            return await Task.FromResult(result);
        }
    }
}
