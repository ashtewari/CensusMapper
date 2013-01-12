namespace CensusMapper.Services
{
    public interface IApiKeyProvider
    {
        string BingMapsKey { get; }
        string CensusApiKey { get; }
        string AzureMobileServicesKey { get; }
    }
}