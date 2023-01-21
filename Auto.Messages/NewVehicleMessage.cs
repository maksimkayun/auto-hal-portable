using System.Text.RegularExpressions;

namespace Auto.Messages;

public class NewVehicleMessage
{
    private string _registration;

    public NewVehicleMessage(string registration, string ownerEmail)
    {
        Registration = registration;
        OwnerEmail = ownerEmail;
    }

    public string Registration
    {
        get => NormalizeRegistration(_registration);
        set => _registration = value;
    }

    private static string NormalizeRegistration(string reg) => 
        Regex.Replace(reg.ToUpperInvariant(), "[^A-Z0-9]", "");

    public string OwnerEmail { get; set; }
}