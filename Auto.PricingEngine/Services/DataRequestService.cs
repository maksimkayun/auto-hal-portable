using System.Net;
using Auto.Data.Entities;
using Auto.PricingEngine.ServiceInterfaces;
using Newtonsoft.Json;

namespace Auto.PricingEngine.Services;

public class DataRequestService : IDataRequestService
{
    private readonly HttpClient _client;
    private readonly string _baseUri = "http://localhost:5000";
    
    public DataRequestService()
    {
        _client = new HttpClient(new HttpClientHandler
        {
            UseDefaultCredentials = true
        });
    }

    public Model? GetModel(string model)
    {
        try
        {
            var response = _client
                .SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri($"{_baseUri}/api/models/{model}")))
                .GetAwaiter()
                .GetResult();
            var resultModel =
                JsonConvert.DeserializeObject<Model>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            return resultModel;
        }
        catch (Exception e)
        {
            // ignored
        }

        return null;
    }
}