using System;
using System.IO;
using System.Linq;
using System.Threading;
using Auto.Data.Entities;
using Auto.Messages;
using Auto.Messages.UpdateContextMessages.Owner;
using Auto.Messages.UpdateContextMessages.Vehicle;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace Auto.Data;

public class ProcessHelper
{
    private readonly IConfigurationRoot config;
    private readonly string SUBSCRIBER_ID = $"Auto.Data_{DateTime.Now}";
    private readonly IBus bus;
    private readonly AutoCsvFileDatabase _context;

    public ProcessHelper(AutoCsvFileDatabase context)
    {
        config = ReadConfiguration();
        bus = RabbitHutch.CreateBus(config.GetConnectionString("AutoRabbitMQ") ?? Environment.GetEnvironmentVariable("CONNECTION_STRING_RABBITMQ", EnvironmentVariableTarget.Machine));
        _context = context;
        Thread threadSyncData = new Thread(SyncAllData);
        threadSyncData.Start();
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

    private void SyncAllData()
    {
        bus.PubSub.Subscribe<NewOwnerMessage>(SUBSCRIBER_ID, ProcessingNewOwnerMessage);
        bus.PubSub.Subscribe<UpdateOwnerMessage>(SUBSCRIBER_ID, ProcessingUpdateOwnerMessage);
        bus.PubSub.Subscribe<DeleteOwnerMessage>(SUBSCRIBER_ID, ProcessingDeleteOwnerMessage);

        bus.PubSub.Subscribe<NewVehicleMessage>(SUBSCRIBER_ID, ProcessingNewVehicleMessage);
        bus.PubSub.Subscribe<UpdateVehicleMessage>(SUBSCRIBER_ID, ProcessingUpdateVehicleMessage);
        bus.PubSub.Subscribe<DeleteVehicleMessage>(SUBSCRIBER_ID, ProcessingDeleteVehicleMessage);
    }

    private void ProcessingDeleteVehicleMessage(DeleteVehicleMessage obj)
    {
        var owners = _context.owners
            .Where(o => o.Value.Vehicle != null && o.Value.Vehicle.Registration == obj.RegistrationCode)
            .ToList();
        foreach (var owner in owners)
        {
            _context.owners[owner.Key].Vehicle = null;
        }
        
        _context.vehicles.Remove(obj.RegistrationCode);
    }

    private void ProcessingUpdateVehicleMessage(UpdateVehicleMessage obj)
    {
        _context.vehicles.Add(obj.Registration, new Vehicle
        {
            Registration = obj.Registration,
            ModelCode = obj.ModelCode,
            Color = obj.Color,
            Year = obj.Year,
            VehicleModel = _context.models[obj.ModelCode]
        }); 
        if (obj.OldRegNumber != obj.Registration)
        {
            var owners = _context.owners
                .Where(o => o.Value.Vehicle != null && o.Value.Vehicle.Registration == obj.OldRegNumber)
                .ToList();
            foreach (var owner in owners)
            {
                _context.owners[owner.Key].Vehicle = _context.vehicles[obj.Registration];
            }
            _context.vehicles.Remove(obj.OldRegNumber);
        }
        
    }

    private void ProcessingNewVehicleMessage(NewVehicleMessage obj)
    {
        _context.vehicles.Add(obj.Registration, new Vehicle
        {
            Registration = obj.Registration,
            ModelCode = obj.ModelCode,
            Color = obj.Color,
            Year = obj.Year,
            VehicleModel = _context.models[obj.ModelCode]
        });
    }

    private void ProcessingDeleteOwnerMessage(DeleteOwnerMessage deleteOwnerMessage)
    {
        _context.owners.Remove(deleteOwnerMessage.Email);
    }

    private void ProcessingNewOwnerMessage(NewOwnerMessage newOwnerMessage)
    {
        if (!_context.owners.ContainsKey(newOwnerMessage.Email))
        {
            _context.owners.Add(key: newOwnerMessage.Email, new Owner
            {
                FirstName = newOwnerMessage.FirstName,
                MiddleName = newOwnerMessage.MiddleName,
                LastName = newOwnerMessage.LastName,
                Email = newOwnerMessage.Email,
                Vehicle = _context.vehicles.FirstOrDefault(e => e.Key == newOwnerMessage.RegCode).Value
            });
        }
    }

    private void ProcessingUpdateOwnerMessage(UpdateOwnerMessage updateOwnerMessage)
    {
        if (_context.owners.ContainsKey(updateOwnerMessage.OldEmail))
        {
            _context.owners[updateOwnerMessage.Email] = new Owner
            {
                FirstName = updateOwnerMessage.FirstName,
                MiddleName = updateOwnerMessage.MiddleName,
                LastName = updateOwnerMessage.LastName,
                Email = updateOwnerMessage.Email,
                Vehicle = _context.vehicles.FirstOrDefault(e => e.Key == updateOwnerMessage.RegCode).Value
            };
            if (updateOwnerMessage.Email != updateOwnerMessage.OldEmail)
            {
                _context.owners.Remove(updateOwnerMessage.OldEmail);
            }
        }
    }
}