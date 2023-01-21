// See https://aka.ms/new-console-template for more information

using Auto.Messages;
using Auto.OwnersEngine;
using EasyNetQ;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

public static class Program
{
    private static readonly IConfigurationRoot config = ReadConfiguration();
    private const string SUBSCRIBER_ID = "Auto.OwnersClient";
    private static readonly IBus bus = RabbitHutch.CreateBus(config.GetConnectionString("AutoRabbitMQ"));
    private static readonly OwnerService.OwnerServiceClient grpcClient = new(GrpcChannel.ForAddress("https://localhost:7261"));
    public static void Main(string[] args)
    {
        bus.PubSub.SubscribeAsync<NewOwnerMessage>(SUBSCRIBER_ID, OnMessagePublished);
        Console.WriteLine("Готово!");
        Console.ReadKey(true);
    }

    public static void OnMessagePublished(NewOwnerMessage newOwnerMessage)
    {
        var request = new VehicleByOwnerEmailRequest
        {
            Email = newOwnerMessage.Email
        };
        try
        {
            var reply = grpcClient.GetVehicleByOwnerEmail(request);
            if (!string.IsNullOrEmpty(reply.Error))
            {
                Thread.Sleep(1500);
                reply = grpcClient.GetVehicleByOwnerEmail(request);
            }
            Console.WriteLine(
                $"Ответ получен! ТС {reply.Model}, регистрационный номер {reply.Registration}\n");

            var message = new NewVehicleOfOwnerMessage(newOwnerMessage.Email, reply.Registration);
            bus.PubSub.PublishAsync(message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при получении ответа. {e.Message}\n{e.StackTrace}");
        }
    }
    private static IConfigurationRoot ReadConfiguration()
    {
        var basePath = Directory.GetParent(AppContext.BaseDirectory)?.FullName;
        return new ConfigurationBuilder()
            .SetBasePath(
                basePath ??
                throw new OperationCanceledException("Не удалось рассчитать путь до файла конфигурации")
            )
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }
}
