using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CensusMapper.Services
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        const string ApiKeysFile = "ApiKeys.xml";

        public ApiKeyProvider()
        {
            var filepath = System.IO.Path.Combine(ApiKeysFile);
            var data = XDocument.Load(filepath);
            var keys = data.Descendants("Keys").First();

            var census = from key in keys.Elements("Key") where key.Attribute("name").Value == "Census" select key.Attribute("value").Value;
            var bing = from key in keys.Elements("Key") where key.Attribute("name").Value == "Bing" select key.Attribute("value").Value;
            var azureMobile = from key in keys.Elements("Key") where key.Attribute("name").Value == "AzureMobile" select key.Attribute("value").Value;

            CensusApiKey = census.ElementAt(0);
            BingMapsKey = bing.ElementAt(0);
            AzureMobileServicesKey = azureMobile.ElementAt(0);            
        }
        public string BingMapsKey { get; private set; }

        public string CensusApiKey { get; private set; }

        public string AzureMobileServicesKey { get; private set; }
    }
}
