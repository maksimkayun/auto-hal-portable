namespace Auto.Messages;

public class NewVehicleOfOwnerMessage
{
    public NewVehicleOfOwnerMessage(string email, string? newVehicle, string? oldVehicle)
    {
        Email = email;
        NewVehicle = newVehicle;
        OldVehicle = oldVehicle;
        CreatedAt = DateTimeOffset.Now;
    }
    public string Email { get; set; }
    public string? NewVehicle { get; set; }
    public string? OldVehicle { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}