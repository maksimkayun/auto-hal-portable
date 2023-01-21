using Auto.Data;
using Auto.Data.Entities;
using Auto.OwnersEngine.Interfaces;

namespace Auto.OwnersEngine.Services;

public class OwnersRepositoryService : IOwnersRepositoryService
{
    private readonly IAutoDatabase _context;

    public OwnersRepositoryService(IAutoDatabase context)
    {
        _context = context;
    }

    public Owner? GetOwnerByRegNumber(string regNumber)
    {
        return _context.ListOwners()
            .SingleOrDefault(o => o.Vehicle != null && o.Vehicle.Registration.Equals(regNumber));
    }

    public Vehicle? GetVehicleByOwnerEmail(string email) =>
        _context.ListOwners()
            .Where(e => e.Vehicle != null)
            .SingleOrDefault(e => e.Email == email)
            ?.Vehicle;
}