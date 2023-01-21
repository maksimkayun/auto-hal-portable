using Auto.Data.Entities;
using Auto.Messages;

namespace Auto.Website.Services.PublishServices.Interfaces;

public interface IOwnerEventPublisher : IPublisher
{
    public void PublishNewOwnerMessage(Owner owner);
    public void PublishNewVehicleOfOwnerMessage(string email, string newVehicle, string oldVehicle);
}