using Auto.Data.Entities;

namespace Auto.OwnersEngine.MappingExtensions;

public static class MappingExtensions
{
    public static OwnerByRegNumberResult? ToOwnerByRegNumberResult(this Owner? owner)
    {
        if (owner != null)
        {
            return new OwnerByRegNumberResult
            {
                Fullname = owner.GetFullName.Replace("&", " "),
                Email = owner.Email,
                RegCodeVehicle = owner.Vehicle?.Registration
            };
        }

        return null;
    }
}