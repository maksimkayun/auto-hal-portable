using Auto.Data;
using Auto.Website.GraphQL.Queries;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Schemas;

public class AutoSchema: Schema {
    public AutoSchema(IAutoDatabase db)
    {
        Query = new VehicleQuery(db);
        Mutation = new VehicleMutation(db);
    }
}