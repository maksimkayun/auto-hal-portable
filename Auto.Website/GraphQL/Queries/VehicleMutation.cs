using System;
using Auto.Data;
using Auto.Data.Entities;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class VehicleMutation: ObjectGraphType
{
    private readonly IAutoDatabase _context;

    public VehicleMutation(IAutoDatabase context)
    {
        _context = context;
    }
    
    private Vehicle UpdateVehicle(IResolveFieldContext<object> context)
    {
        throw new NotImplementedException();
    }
} 