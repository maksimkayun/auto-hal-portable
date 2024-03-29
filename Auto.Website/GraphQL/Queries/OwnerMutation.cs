﻿using System;
using System.Collections.Generic;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class OwnerMutation : ObjectGraphType
{
    private readonly IAutoDatabase _context;

    public OwnerMutation(IAutoDatabase context)
    {
        _context = context;

        Field<OwnerGraphType>("createOwner",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "firstName"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "middleName"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "lastName"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "email"},
                new QueryArgument<StringGraphType> {Name = "vehicle"}
            ),
            resolve: tContext =>
            {
                try
                {
                    var f_name = tContext.GetArgument<string>("firstName");
                    var m_name = tContext.GetArgument<string>("middleName");
                    var l_name = tContext.GetArgument<string>("lastName");
                    var email = tContext.GetArgument<string>("email");
                    var vehicle = tContext.GetArgument<string>("vehicle");

                    var newOwner = new Owner(f_name, m_name, l_name, email);

                    if (!string.IsNullOrEmpty(vehicle))
                    {
                        newOwner.Vehicle = _context.FindVehicle(vehicle);
                    }

                    _context.CreateOwner(newOwner);
                    return newOwner;
                }
                catch (Exception e)
                {
                    tContext.Errors.Add(new ExecutionError(e.Message));
                    throw;
                }
            }
        );
        
        Field<OwnerGraphType>("updateOwner",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "oldEmail"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "firstName"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "middleName"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "lastName"},
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "email"},
                new QueryArgument<StringGraphType> {Name = "vehicle"}
            ),
            resolve: tContext =>
            {
                try
                {
                    var oldEmail = tContext.GetArgument<string>("oldEmail");
                    var f_name = tContext.GetArgument<string>("firstName");
                    var m_name = tContext.GetArgument<string>("middleName");
                    var l_name = tContext.GetArgument<string>("lastName");
                    var email = tContext.GetArgument<string>("email");
                    var vehicle = tContext.GetArgument<string>("vehicle");

                
                    var newOwner = new Owner(f_name, m_name, l_name, email);

                    if (vehicle != null)
                    {
                        if (vehicle == "")
                        {
                            newOwner.Vehicle = null;
                        }
                        else
                        {
                            newOwner.Vehicle = _context.FindVehicle(vehicle);;
                        }
                    }

                    var ownerInContext = _context.FindOwnerByEmail(oldEmail);
                    if (ownerInContext == null)
                    {
                        throw new KeyNotFoundException("Владелец не найден");
                    }

                    _context.UpdateOwner(newOwner, oldEmail);
                    return newOwner;
                }
                catch (Exception e)
                {
                    tContext.Errors.Add(new ExecutionError(e.Message));
                    throw;
                }
            }
        );
        
        Field<OwnerGraphType>("deleteOwner",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "email"}
            ),
            resolve: tContext =>
            {
                var email = tContext.GetArgument<string>("email");
                var owner = _context.FindOwnerByEmail(email);
                _context.DeleteOwner(owner);
                return owner;
            }
        );
    }
}



