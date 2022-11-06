// See https://aka.ms/new-console-template for more information

using Auto.OwnersEngine;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:7261");
var grpcClient = new OwnerService.OwnerServiceClient(channel);
Console.WriteLine("Готово! Введите регистрационный номер!");
while (true)
{
    var regNumber = Console.ReadLine();
    var request = new OwnerByRegNumberRequest()
    {
        RegisterNumber = regNumber
    };
    try
    {
        var reply = grpcClient.GetOwnerByRegNumber(request);
        Console.WriteLine(
            $"Ответ получен! Владелец {reply.Fullname}, email {reply.Email}, регистрационный номер {reply.RegCodeVehicle}\n");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Ошибка при получении ответа. {e.Message}");
    }
   
    Console.WriteLine("Введите регистрационный номер");
}