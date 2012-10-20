using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper
{
    /*
                SbaApi sba = new SbaApi();
                var county = await sba.GetCountyData(address.AdminDistrict);     
     
     */
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

        public async Task<SbaData[]> GetCountyData(string state)
        {
            var json = await GetData(state, queryCounties);
            SbaData[] county = JsonConvert.DeserializeObject<SbaData[]>(json);
            return county;
        }

        public async Task<SbaData[]> GetCityData(string state)
        {
            var json = await GetData(state, queryCities);
            SbaData[] county = JsonConvert.DeserializeObject<SbaData[]>(json);
            return county;
        }

        private async Task<string> GetData(string state, string query)
        {
            var task = client.GetAsync(string.Format(query, state));
            HttpResponseMessage response = await task;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(result);
                return result;
            }

            return null;
        }
    }
}
