using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper
{
    internal class Census
    {
        private string query = @"{0}?&key={1}&{2}";
        private string _apiKey = "";
        HttpClient client = null;

        public Census(string apiKey)
        {
            _apiKey = apiKey;
            DataSet = CensusDataSet.SF1;
            client = new HttpClient();
            client.BaseAddress = new Uri("http://api.census.gov/data/2010/");
        }

        public CensusDataSet DataSet { get; set; }

        public async Task<JArray> GetCensusData(string requestUri)
        {
            var task = client.GetAsync(string.Format(query, DataSet == CensusDataSet.SF1 ? "sf1" : "acs", _apiKey, requestUri));
            HttpResponseMessage response = await task;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(result);
                return JArray.Parse(result);
            }
            else
            {
                Debug.WriteLine(response.Content);
            }

            return null;
        }
    }

    internal enum CensusDataSet
    {
        SF1,
        ACS        
    }
}
