using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Auto.Messages;

public class NewVehicleMessage
{
    private string registration;

    public NewVehicleMessage()
    {
    }

    public NewVehicleMessage(string regNumber)
    {
        Registration = regNumber;
    }
    
    public string ModelCode { get; set; }
    public string Color { get; set; }
    public int Year { get; set; }

    [JsonIgnore]
    public string VehicleModel { get; set; }

    public string Registration
    {
        get => NormalizeRegistration(registration);
        set => registration = value;
    }
    private static string NormalizeRegistration(string reg) => Regex.Replace(reg.ToUpperInvariant(), "[^A-Z0-9]", "");
}