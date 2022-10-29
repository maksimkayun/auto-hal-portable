using Auto.PricingEngine.ServiceInterfaces;
using Grpc.Core;

namespace Auto.PricingEngine.Services;

public class PricerService : Pricer.PricerBase {
    private readonly ILogger<PricerService> logger;
    private readonly IDataRequestService _service;
    public PricerService(ILogger<PricerService> logger, IDataRequestService service)
    {
        this.logger = logger;
        _service = service;
    }
    public override Task<PriceReply> GetPrice(PriceRequest request, ServerCallContext context)
    {
        if (TryFindModel(request.Model))
        {
            if (request.Manufacturer.StartsWith("m"))
                return Task.FromResult(new PriceReply
                {
                    Price = new Random().Next(5_000_000, 900_000_000),
                    CurrencyCode = "RUB"
                });
        
            return Task.FromResult(new PriceReply
            {
                Price = new Random().Next(1_000_000, 5_000_000),
                CurrencyCode = "RUB"
            });
        }
        
        return Task.FromResult(new PriceReply
        {
            Price = -1,
            CurrencyCode = "Такой модели нет"
        });

    }

    private bool TryFindModel(string byModel) => _service.GetModel(byModel) != null;
}