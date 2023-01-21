using System.Text.RegularExpressions;

namespace Auto.Messages;

public class NewVehicleMessage
{
    private string registration;

    public NewVehicleMessage(string regNumber)
    {
        Registration = regNumber;
    }

    public string Registration
    {
        get => NormalizeRegistration(registration);
        set => registration = value;
    }
    private static string NormalizeRegistration(string reg) => Regex.Replace(reg.ToUpperInvariant(), "[^A-Z0-9]", "");
}