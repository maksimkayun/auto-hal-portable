using System.Text.Json.Serialization;

namespace Auto.Messages.UpdateContextMessages.Vehicle;

public class UpdateVehicleMessage : NewVehicleMessage
{
    public UpdateVehicleMessage(string regNumber) : base(regNumber)
    {
    }
    public string OldRegNumber { get; set; }
}