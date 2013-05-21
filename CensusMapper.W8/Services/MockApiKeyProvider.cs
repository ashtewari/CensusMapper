using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusMapper.Services
{
    public class MockApiKeyProvider : IApiKeyProvider
    {
        public string BingMapsKey
        {
            get { return "BingMapsKey"; }
        }
        public string CensusApiKey
        {
            get { return "CensusApiKey"; }
        }

        public string AzureMobileServicesKey
        {
            get { return "AzureMobileServicesKey"; }
        }
    }
}
