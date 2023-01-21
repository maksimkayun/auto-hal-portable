using Auto.Data.Entities;
using Auto.Messages;
using Auto.Website.Services.PublishServices.Interfaces;
using EasyNetQ;

namespace Auto.Website.Services.PublishServices.Implementations;

public class PublishOwnerMessagesService : IOwnerEventPublisher
{
    private readonly IBus _bus;

    public PublishOwnerMessagesService(IBus bus)
    {
        _bus = bus;
    }

    public void PublishNewOwnerMessage(Owner owner)
    {
        var message = new NewOwnerMessage(owner.FirstName, owner.MiddleName, owner.LastName, owner.Email,
            owner.Vehicle?.Registration);
        PublishMessage(message);
    }
    
    public void PublishNewVehicleOfOwnerMessage(string email, string newVehicle, string oldVehicle)
    {
        var message = new NewVehicleOfOwnerMessage(email, newVehicle, oldVehicle);
        PublishMessage(message);
    }

    public void PublishMessage<T>(T message) => _bus.PubSub.Publish(message);
}