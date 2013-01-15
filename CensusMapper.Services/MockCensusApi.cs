using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper.Services
{
    public class MockCensusApi : ICensusApi
    {
        public MockCensusApi(IApiKeyProvider keyService)
        {
            
        }

        public CensusDataSet DataSet { get; set; }
        public async Task<IDictionary<string, int?>> GetPopulationForStates(IList<string> states)
        {
            var result = new Dictionary<string, int?>();

            foreach (var state in states)
            {
                if (state == "*")
                {
                    result.Add("37", 9535483);
                    result.Add("45", 4625364);
                    result.Add("51", 8001024);
                }
                else
                {
                    result.Add("01", 999999);
                }
            }

            return result;
        }

        public async Task<int?> GetPopulationForZipCode(string state, string zipcode)
        {
            return 55555;
        }
    }
}
