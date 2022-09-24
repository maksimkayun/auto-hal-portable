using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using Auto.Data;
using Auto.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Website.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class OwnersController : ControllerBase
{
    private readonly IAutoDatabase _context;

    public OwnersController(IAutoDatabase context)
    {
        _context = context;
    }

    [HttpPost]
    [Produces("application/hal+json")]
    public IActionResult Get(int index = 0, int count = 10)
    {
        var items = _context.ListOwners().Skip(index).Take(count).Select(GetResource);
        var total = _context.CountOwners();
        var _links = Paginate("/api/owners", index, count, total);
        var _actions = new
        {
            create = new
            {
                method = "POST",
                type = "application/json",
                name = "Create a new owner",
                href = "/api/addowner"
            },
            delete = new
            {
                method = "POST",
                name = "Delete a owner",
                href = "/api/owners/{name}"
            }
        };
        var result = new
        {
            _links, _actions, index, count, total, items
        };
        return Ok(result);
    }

    private dynamic GetResource(Owner owner)
    {
        var pathOwner = "/api/owners/";
        var pathVehicle = "/api/vehicles/";
        var ownerDynamic = owner.ToDynamic();
        ownerDynamic._links = new
        {
            self = new
            {
                href = $"{pathOwner}{owner.GetFullName}"
            }
        };
        if (owner.Vehicles?.Count > 0)
        {
            ownerDynamic._links = new
            {
                self = new
                {
                    href = $"{pathOwner}{owner.GetFullName}"
                },
                vehicles = new
                {
                    // /api/vehicles/group/AA07AMM&AAC792H&AAY452D&AB01MFL
                    href = $"{pathVehicle}group/{owner.Vehicles?.Select(e => e.Registration).ToQueryString()}"
                }
            };
        }
        else
        {
            ownerDynamic._links = new
            {
                self = new
                {
                    href = $"{pathOwner}{owner.GetFullName}"
                }
            };
        }

        return ownerDynamic;
    }

    private dynamic Paginate(string url, int index, int count, int total)
    {
        dynamic links = new ExpandoObject();
        links.self = new {href = url};
        links.final = new {href = $"{url}?index={total - (total % count)}&count={count}"};
        links.first = new {href = $"{url}?index=0&count={count}"};
        if (index > 0) links.previous = new {href = $"{url}?index={index - count}&count={count}"};
        if (index + count < total) links.next = new {href = $"{url}?index={index + count}&count={count}"};
        return links;
    }
}