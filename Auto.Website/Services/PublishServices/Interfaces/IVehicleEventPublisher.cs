using Auto.Messages;

namespace Auto.Website.Services.PublishServices.Interfaces;

public interface IVehicleEventPublisher : IPublisher
{
    public void PublishNewVehicleMessage(string regNumber, string ownerEmail);
}