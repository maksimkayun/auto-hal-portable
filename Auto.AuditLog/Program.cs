using Auto.Messages;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace Auto.AuditLog
{
    class Program
    {
        private static readonly IConfigurationRoot config = ReadConfiguration();

        private const string SUBSCRIBER_ID = "Auto.AuditLog";

        static async Task Main(string[] args)
        {
            using var bus = RabbitHutch.CreateBus(config.GetConnectionString("AutoRabbitMQ"));
            Console.WriteLine("Connected! Listening for NewVehicleMessage messages.");
            await bus.PubSub.SubscribeAsync<NewOwnerMessage>(SUBSCRIBER_ID, HandleNewOwnerMessage);
            await bus.PubSub.SubscribeAsync<NewVehicleOfOwnerMessage>(SUBSCRIBER_ID, HandleNewVehicleOfOwnerMessage);
            Console.ReadKey(true);
        }

        private static void HandleNewOwnerMessage(NewOwnerMessage message)
        {
            var csv =
                $"{message.FirstName},{message.MiddleName},{message.LastName},{message.Email},{message.CreatedAt}";
            Console.WriteLine(csv);
        }
        
        private static void HandleNewVehicleOfOwnerMessage(NewVehicleOfOwnerMessage message)
        {
            var csv =
                $"{message.Email},old:{message.OldVehicle},new:{message.NewVehicle},{message.CreatedAt}";
            Console.WriteLine(csv);
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
}