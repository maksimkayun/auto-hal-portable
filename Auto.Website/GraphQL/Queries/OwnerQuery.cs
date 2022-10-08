using System;
using System.Linq;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class OwnerQuery : ObjectGraphType
{
    private readonly IAutoDatabase _context;

    public OwnerQuery(IAutoDatabase context)
    {
        _context = context;
        Field<OwnerGraphType>("owner", "Запрос для получения данных о владельце",
            new QueryArguments(MakeNonNullStringArgument("email", "Полное имя искомого владельца")),
            resolve: GetOwner);
    }

    private Owner GetOwner(IResolveFieldContext<object> context)
    {
        var email = context.GetArgument<string>("email");
        try
        {
            return _context.FindOwnerByEmail(email);
        }
        catch (Exception e)
        {
            context.Errors.Add(new ExecutionError(e.Message));
            throw;
        }
    }

    private QueryArgument MakeNonNullStringArgument(string name, string description)
    {
        return new QueryArgument<NonNullGraphType<StringGraphType>>
        {
            Name = name, Description = description
        };
    }
}


