using System;
using System.Net.Http;
using System.Threading.Tasks;
//using Bing.Maps;
using CensusMapper.Models;
using CensusMapper.Services;
using Newtonsoft.Json;

namespace CensusMapper.Services
{
    public class BingMapsApi : IBingMapsApi
    {
        private string baseUri = @"{0}?&key={1}";
        private string _bingMapsKey = String.Empty;

        private readonly HttpClient client = new HttpClient();

        public BingMapsApi(IApiKeyProvider keyService)
	    {
            _bingMapsKey = keyService.BingMapsKey;

            client = new HttpClient();
            client.BaseAddress = new Uri("http://dev.virtualearth.net/REST/v1/Locations/");
	    }

        public async Task<Address> GetAddress(Location location)
        {
            string requestUri = String.Format(baseUri, string.Format("{0},{1}", location.Latitude, location.Longitude), _bingMapsKey);
            var task = client.GetAsync(requestUri);
            HttpResponseMessage response = await task;
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(json);

            /*
                {"authenticationResultCode":"ValidCredentials","brandLogoUri":"http:\/\/dev.virtualearth.net\/Branding\/logo_powered_by.png","copyright":"Copyright © 2012 Microsoft and its suppliers. All rights reserved. This API cannot be accessed and the content and any results may not be used, reproduced or transmitted in any manner without express written permission from Microsoft Corporation.","resourceSets":[{"estimatedTotal":1,"resources":[{"__type":"Location:http:\/\/schemas.microsoft.com\/search\/local\/ws\/rest\/v1","bbox":[38.342731475830078,-107.95529937744141,38.668247222900391,-107.63488006591797],"name":"81401, CO","point":{"type":"Point","coordinates":[38.505489349365234,-107.79508972167969]},"address":{"adminDistrict":"CO","adminDistrict2":"Montrose Co.","countryRegion":"United States","formattedAddress":"81401, CO","locality":"Montrose","postalCode":"81401"},"confidence":"Medium","entityType":"Postcode1","geocodePoints":[{"type":"Point","coordinates":[38.505489349365234,-107.79508972167969],"calculationMethod":"Rooftop","usageTypes":["Display"]}],"matchCodes":["Good","UpHierarchy"]}]}],"statusCode":200,"statusDescription":"OK","traceId":"a36c8d3f28c64c47938f0f100d098877|BL2M001304|02.00.149.2500|BL2MSNVM002819, BL2MSNVM003153"}             
             */

            Entity result = JsonConvert.DeserializeObject<Entity>(json);

            if (result != null && result.StatusCode == 200 
                && result.ResourceSets.Count > 0 && 
                result.ResourceSets[0].Resources.Count > 0
                && result.ResourceSets[0].Resources[0].EntityType == "Address")
            {
                return result.ResourceSets[0].Resources[0].Address;
            }

            return null;            
        }
    }
}
