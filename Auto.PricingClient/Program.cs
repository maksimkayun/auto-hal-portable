// See https://aka.ms/new-console-template for more information

using Auto.PricingEngine;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:7102");
var grpcClient = new Pricer.PricerClient(channel);
Console.WriteLine("Ready! Press any key to send a gRPC request (or Ctrl-C to quit):");
while (true) {
    Console.ReadKey(true);
    var request = new PriceRequest {
        Model = "audi-rs6",
        Color = "Green",
        Year = 1985
    };
    var reply = grpcClient.GetPrice(request);
    Console.WriteLine($"Price: {reply.Price}");
}