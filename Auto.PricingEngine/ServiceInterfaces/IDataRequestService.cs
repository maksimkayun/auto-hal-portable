using Auto.Data.Entities;

namespace Auto.PricingEngine.ServiceInterfaces;

public interface IDataRequestService
{
    public Model? GetModel(string model);
}