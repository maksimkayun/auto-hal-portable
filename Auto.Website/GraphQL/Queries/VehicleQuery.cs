using Auto.Data;
using Auto.Data.Entities;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class VehicleQuery : ObjectGraphType
{
    private readonly IAutoDatabase _context;

    public VehicleQuery(IAutoDatabase context)
    {
        _context = context;
    }
    private Vehicle GetVehicle(IResolveFieldContext<object> context)
    {
        var registration = context.GetArgument<string>("registration");
        return _context.FindVehicle(registration);
    }
} 