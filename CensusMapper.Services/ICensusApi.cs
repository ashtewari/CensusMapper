using System.Collections.Generic;
using System.Threading.Tasks;

namespace CensusMapper.Services
{
    public interface ICensusApi
    {
        CensusDataSet DataSet { get; set; }
        Task<IDictionary<string, int?>> GetPopulationForStates(IList<string> states);
        Task<int?> GetPopulationForZipCode(string state, string zipcode);
    }
}