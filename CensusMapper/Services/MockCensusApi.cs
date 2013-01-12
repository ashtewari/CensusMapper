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
                result.Add((state == "*") ? "37" : state, 999999);
            }

            return result;
        }

        public async Task<int?> GetPopulationForZipCode(string state, string zipcode)
        {
            return 55555;
        }
    }
}
