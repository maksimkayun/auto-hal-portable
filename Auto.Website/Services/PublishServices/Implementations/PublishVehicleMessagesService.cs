using Auto.Messages;
using Auto.Website.Services.PublishServices.Interfaces;
using EasyNetQ;

namespace Auto.Website.Services.PublishServices.Implementations;

public class PublishVehicleMessagesService : IVehicleEventPublisher
{
    private readonly IBus _bus;

    public PublishVehicleMessagesService(IBus bus)
    {
        _bus = bus;
    }

    public void PublishMessage<T>(T message) => _bus.PubSub.PublishAsync(message);

    public void PublishNewVehicleMessage(string regNumber)
    {
        var message = new NewVehicleMessage(regNumber);
        PublishMessage(message);
    }
}