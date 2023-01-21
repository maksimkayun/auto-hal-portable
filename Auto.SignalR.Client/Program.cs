using System.Text.Json.Serialization;
using Auto.Data.Entities;
using Auto.Messages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Auto.SignalR.Client;

public static class Program
{
    private const string SIGNALR_HUB_URL = "https://localhost:5001/hub";
    private static HubConnection hub;
    
    private const string SUBSCRIBER_ID = "Auto.SignalR";


    public static async Task Main(string[] args)
    {
        hub = new HubConnectionBuilder().WithUrl(SIGNALR_HUB_URL).Build();
        await hub.StartAsync();
        Console.WriteLine("Hub started!");
        Console.WriteLine("Press any key to send a message (Ctrl-C to quit)");
        
        using var bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("CONNECTION_STRING_RABBITMQ", EnvironmentVariableTarget.Machine));
        Console.WriteLine("Connected! Listening for NewVehicleMessage messages.");
        await bus.PubSub.SubscribeAsync<NewVehicleOfOwnerMessage>(SUBSCRIBER_ID, HandleNewVehicleOfOwnerMessage);
        Console.ReadKey(true);
    }

    private static async Task HandleNewVehicleOfOwnerMessage(NewVehicleOfOwnerMessage obj)
    {
        var message = JsonConvert.SerializeObject(obj);
        await hub.SendAsync("NotifyWebUsers", "Auto.Notifier", message);
        Console.WriteLine($"Sent: {message}");   
    }
}