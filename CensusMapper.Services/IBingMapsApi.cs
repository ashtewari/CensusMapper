using System.Threading.Tasks;
//using Bing.Maps;

namespace CensusMapper
{
    public interface IBingMapsApi
    {
        Task<Address> GetAddress(Coordinates location);
    }
}