using Auto.Data.Entities;
using Auto.Messages;
using Auto.Messages.UpdateContextMessages.Vehicle;
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

    public void PublishNewVehicleMessage(string regNumber, Vehicle vehicle)
    {
        var message = new NewVehicleMessage
        {
            Registration = vehicle.Registration,
            ModelCode = vehicle.ModelCode,
            Color = vehicle.Color,
            Year = vehicle.Year,
            VehicleModel = vehicle.VehicleModel.Name
        };
        PublishMessage(message);
    }

    public void PublishUpdateVehicleMessage(string oldRegNumber, Vehicle vehicle)
    {
        var message = new UpdateVehicleMessage
        {
            Registration = vehicle.Registration,
            ModelCode = vehicle.ModelCode,
            Color = vehicle.Color,
            Year = vehicle.Year,
            VehicleModel = vehicle.VehicleModel.Name,
            OldRegNumber = oldRegNumber
        };
        PublishMessage(message);
    }

    public void PublishDeleteVehicleMessage(string regNumber)
    {
        var message = new DeleteVehicleMessage
        {
            RegistrationCode = regNumber
        };
        PublishMessage(message);
    }
}