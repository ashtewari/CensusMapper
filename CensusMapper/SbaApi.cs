using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper
{
    public class SbaApi
    {
        // All counties by state
        // http://api.sba.gov/geodata/county_links_for_state_of/nc.json

        // All cities by state
        // http://api.sba.gov/geodata/city_links_for_state_of/nc.json

        private string queryCounties = @"county_links_for_state_of/{0}.json";
        private string queryCities = @"city_links_for_state_of/{0}.json";

        HttpClient client = null;

        public SbaApi()
        {            
            client = new HttpClient();
            client.BaseAddress = new Uri("http://api.sba.gov/geodata/");
        }

        public async Task<JArray> GetCountyData(string state)
        {
            return await GetData(state, queryCounties);
        }

        public async Task<JArray> GetCityData(string state)
        {
            return await GetData(state, queryCities);
        }

        private async Task<JArray> GetData(string state, string query)
        {
            var task = client.GetAsync(string.Format(query, state));
            HttpResponseMessage response = await task;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(result);
                return JArray.Parse(result);
            }

            return null;
        }
    }
}
