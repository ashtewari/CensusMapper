using CensusMapper.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace CensusMapper
{
    public class CensusApi : ICensusApi
    {
        private string query = @"{0}?&key={1}&{2}";
        private string _apiKey = "";
        HttpClient client = null;

        public CensusApi(IApiKeyProvider keyService)
        {
            _apiKey = keyService.CensusApiKey;
            DataSet = CensusDataSet.SF1;
            client = new HttpClient();
            client.BaseAddress = new Uri("http://api.census.gov/data/2010/");
        }

        public CensusDataSet DataSet { get; set; }

        private async Task<JArray> GetCensusData(string requestUri)
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

        public async Task<IDictionary<string, int?>> GetPopulationForStates(IList<string> states)
        {
            var results = new Dictionary<string, int?>();

            foreach (var state in states)
            {
                string requestUri = string.Format("get=P0010001,NAME&for=state:{0}", state);
                var array = await this.GetCensusData(requestUri);

                if (array == null) return results;

                foreach (var item in array)
                {
                    string fips = item[2].ToString();

                    int count;
                    if (int.TryParse(item[0].ToString(), out count))
                    {
                        results.Add(fips, count);
                    }
                    else
                    {
                        results.Add(fips, null);
                    }
                }
            }

            return results;
        }

        public async Task<int?> GetPopulationForZipCode(string state, string zipcode)
        {           
            string requestUri = string.Format("get=P0010001&for=zip+code+tabulation+area:{0}&in=state:{1}", zipcode, state);
            var array = await this.GetCensusData(requestUri);

            if (array == null) return null;

            int count;

            if (int.TryParse(array[1][0].ToString(), out count))
            {
                return count;
            }

            return null;
        }
    }

    public enum CensusDataSet
    {
        SF1,
        ACS        
    }
}
