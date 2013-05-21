using System.Threading.Tasks;
using CensusMapper.Models;

namespace CensusMapper.Services
{
    public interface IBingMapsApi
    {
        Task<Address> GetAddress(Location location);
    }
}