using Auto.OwnersEngine.Interfaces;
using Auto.OwnersEngine.MappingExtensions;
using Grpc.Core;

namespace Auto.OwnersEngine.Services;

public class OwnerService : OwnersEngine.OwnerService.OwnerServiceBase
{
    private readonly IOwnersRepositoryService _service;

    public OwnerService(IOwnersRepositoryService service)
    {
        _service = service;
    }

    public override Task<OwnerByRegNumberResult?> GetOwnerByRegNumber(OwnerByRegNumberRequest request,
        ServerCallContext context)
    {
        return Task.FromResult(_service.GetOwnerByRegNumber(request.RegisterNumber).ToOwnerByRegNumberResult());
    }

    public override Task<VehicleByOwnerEmailResult?> GetVehicleByOwnerEmail(VehicleByOwnerEmailRequest request,
        ServerCallContext context)
    {
        return Task.FromResult(_service.GetVehicleByOwnerEmail(request.Email).ToVehicleByOwnerEmailResult());
    }
}