using System.Text.Json.Serialization;
using Auto.Data.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace Auto.SignalR.Client;

public static class Program
{
    private const string SIGNALR_HUB_URL = "https://localhost:5001/hub";
    private static HubConnection hub;

    public static async Task Main(string[] args)
    {
        hub = new HubConnectionBuilder().WithUrl(SIGNALR_HUB_URL).Build();
        await hub.StartAsync();
        Console.WriteLine("Hub started!");
        Console.WriteLine("Press any key to send a message (Ctrl-C to quit)");
        while (true)
        {
            var input = Console.ReadLine();
            var message = JsonConvert.SerializeObject(new Owner
            {
                FirstName = "TestFirName",
                MiddleName = "TestMidName",
                LastName = "TestLastName",
                Email = "test@email.ru",
                Vehicle = null
            });
            await hub.SendAsync("NotifyWebUsers", "Auto.Notifier", message);
            Console.WriteLine($"Sent: {message}");
        }
    }
}