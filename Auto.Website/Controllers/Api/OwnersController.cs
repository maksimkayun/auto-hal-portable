using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
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
        dynamic result;
        try
        {
            var items = _context.ListOwners().Skip(index).Take(count).Select(GetResource);
            var total = _context.CountOwners();
            var links = Paginate("/api/owners", index, count, total);

            result = new
            {
                links, index, count, total, items
            };
            return Ok(result);
        }
        catch (Exception e)
        {
            result = new { message = e.Message };
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Produces("application/hal+json")]
    [Route("{name}")]
    public IActionResult GetByName(string name)
    {
        dynamic result;
        try
        {
            var item = GetResource(_context.FindOwnerByName(name));
            var total = _context.CountOwners();
            result = new
            {
                total,
                item
            };
            return Ok(result);
        }
        catch (Exception e)
        {
            result = new { message = e.Message };
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Produces("application/hal+json")]
    [Route("delete/{name}")]
    public IActionResult Remove(string name)
    {
        var owner = _context.FindOwnerByName(name);
        _context.DeleteOwner(owner);
        return Ok(owner);
    }
    
    [HttpPost]
    [Produces("application/hal+json")]
    [Route("update/{name}")]
    public IActionResult Update(string name, [FromBody] dynamic owner)
    {
        dynamic vehicles = ParseVehicles(owner._links.vehicles?.href);

        var ownerInContext =
            _context.FindOwnerByName(name);
        
        ownerInContext.FirstName = owner.FirstName;
        ownerInContext.MiddleName = owner.MiddleName;
        ownerInContext.LastName = owner.LastName;
        ownerInContext.Email = owner.Email;

        if (vehicles != null)
        {
            ownerInContext.Vehicles = new List<Vehicle>();
            foreach (var e in vehicles.Result.Value)
            {
                Vehicle vehicle = _context.FindVehicle((string) e.Registration);
                ownerInContext.Vehicles.Add(vehicle);
            }
        }
        
        _context.UpdateOwner(ownerInContext);

        return GetByName(ownerInContext.GetFullName);
    }

    private Task<IActionResult> ParseVehicles(dynamic href)
    {
        var token = ((string) href).Split("/").LastOrDefault();
        return new VehiclesController(_context).GetGroup(token);
    }

    private dynamic GetResource(Owner owner)
    {
        var pathOwner = "/api/owners/";
        var pathVehicle = "/api/vehicles/";
        var ownerDynamic = owner.ToDynamic();
        
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

        ownerDynamic.actions = new
        {
            update = new
            {
                href = $"/api/owners/update",
                accept = "application/json"
            },
            delete = new
            {
                href = $"/api/owners/delete/{owner.GetFullName}"
            }
        };
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