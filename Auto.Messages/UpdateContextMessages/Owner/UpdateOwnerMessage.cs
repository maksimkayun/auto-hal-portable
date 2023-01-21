namespace Auto.Messages.UpdateContextMessages.Owner;

public class UpdateOwnerMessage : NewOwnerMessage
{
    public string OldEmail { get; set; }

    public UpdateOwnerMessage(string firstName, string middleName, string lastName, string newEmail, string oldEmail,
        string? regCodeVehicle = null) : base(firstName, middleName, lastName, newEmail, regCodeVehicle)
    {
        OldEmail = oldEmail;
    }
}