

namespace Auto.Messages;

public class NewOwnerMessage
{
    public NewOwnerMessage(string firstName, string middleName, string lastName, 
        string email, string? regCodeVehicle = null)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
        CreatedAt = DateTimeOffset.Now;
    }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    
    public string Email { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}