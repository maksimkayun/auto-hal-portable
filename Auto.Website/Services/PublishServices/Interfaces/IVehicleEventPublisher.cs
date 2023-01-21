using Auto.Data.Entities;
using Auto.Messages;

namespace Auto.Website.Services.PublishServices.Interfaces;

public interface IVehicleEventPublisher : IPublisher
{
    public void PublishNewVehicleMessage(string regNumber, Vehicle vehicle);
    public void PublishUpdateVehicleMessage(string oldRegNumber, Vehicle vehicle);
    public void PublishDeleteVehicleMessage(string regNumber);
}