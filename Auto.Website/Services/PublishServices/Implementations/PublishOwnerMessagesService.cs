using Auto.Data.Entities;
using Auto.Messages;
using Auto.Messages.UpdateContextMessages.Owner;
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

    public void PublishUpdateOwnerMessage(string oldEmail, Owner owner)
    {
        var message = new UpdateOwnerMessage
        {
            FirstName = owner.FirstName,
            MiddleName = owner.MiddleName,
            LastName = owner.LastName,
            Email = owner.Email,
            OldEmail = oldEmail,
            RegCode = owner.Vehicle?.Registration,
            CreatedAt = default
        };
        PublishMessage(message);
    }

    public void PublishDeleteOwnerMessage(string email)
    {
        var message = new DeleteOwnerMessage
        {
            Email = email
        };
        PublishMessage(message);
    }

    public void PublishNewVehicleOfOwnerMessage(string email, string newVehicle, string oldVehicle)
    {
        var message = new NewVehicleOfOwnerMessage(email, newVehicle, oldVehicle);
        PublishMessage(message);
    }

    public void PublishMessage<T>(T message) => _bus.PubSub.Publish(message);
}