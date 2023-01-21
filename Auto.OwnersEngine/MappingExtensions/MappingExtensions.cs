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

    public static VehicleByOwnerEmailResult? ToVehicleByOwnerEmailResult(this Vehicle? vehicle)
    {
        if (vehicle != null)
        {
            return new VehicleByOwnerEmailResult
            {
                Model = vehicle.VehicleModel.Name,
                ModelCode = vehicle.ModelCode,
                Registration = vehicle.Registration,
                Year = vehicle.Year.ToString(),
                Color = vehicle.Color
            };
        }

        return new VehicleByOwnerEmailResult()
        {
            Error = "ТС, зарегистрированное за владельцем не найдено"
        };
    }
}