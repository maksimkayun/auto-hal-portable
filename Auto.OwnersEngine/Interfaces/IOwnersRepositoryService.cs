using Auto.Data.Entities;

namespace Auto.OwnersEngine.Interfaces;

public interface IOwnersRepositoryService
{
    public Owner GetOwnerByRegNumber(string regNumber);
    public Vehicle GetVehicleByOwnerEmail(string email);
}