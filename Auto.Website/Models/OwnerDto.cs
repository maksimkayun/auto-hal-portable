namespace Auto.Website.Models;

public class OwnerDto
{
    public OwnerDto()
    {
    }
    public OwnerDto(string firstName, string middleName, string? lastName, string email, string regCodeVehicle = null)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Email = email;
        RegCodeVehicle = regCodeVehicle;
    }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string? LastName { get; set; }
    
    public string Email { get; set; }
    
    public string? RegCodeVehicle { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string GetFullName => $"{FirstName}&{MiddleName}&{LastName}";
}