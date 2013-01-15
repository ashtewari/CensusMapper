using System.Threading.Tasks;
using CensusMapper.Models;

namespace CensusMapper
{
    public interface IBingMapsApi
    {
        Task<Address> GetAddress(Location location);
    }
}