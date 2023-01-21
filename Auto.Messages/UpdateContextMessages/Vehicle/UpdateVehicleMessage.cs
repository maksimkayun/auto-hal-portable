using System.Text.Json.Serialization;

namespace Auto.Messages.UpdateContextMessages.Vehicle;

public class UpdateVehicleMessage : NewVehicleMessage
{
    public string OldRegNumber { get; set; }
}