using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.Models;
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
    public async Task<IActionResult> Get(int index = 0, int count = 10)
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
            result = new {message = e.Message};
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Produces("application/hal+json")]
    [Route("{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        dynamic result;
        try
        {
            var item = GetResource(_context.FindOwnerByEmail(email), email);
            if (item == null)
            {
                throw new Exception("Владелец с такой почтой не найден");
            }

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
            result = new {message = e.Message};
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Produces("application/hal+json")]
    [Route("add")]
    public async Task<IActionResult> Add([FromBody] OwnerDto ownerDto)
    {
        dynamic result;
        try
        {
            Vehicle vehicle = null;
            if (!string.IsNullOrEmpty(ownerDto.RegCodeVehicle))
            {
                vehicle = _context.FindVehicle(ownerDto.RegCodeVehicle);
            }

            var ownerInContext = _context.FindOwnerByEmail(ownerDto.Email);
            if (ownerInContext == null)
            {
                Owner newOwner = CreateOwner(ownerDto, vehicle);
                result = new
                {
                    message = "Создан новый владелец",
                    owner = GetResource(newOwner)
                };
                return Ok(result);
            }

            result = new {message = "Владелец с такой почтой уже существует", owner = GetResource(ownerInContext)};
        }
        catch (Exception e)
        {
            result = new {message = e.Message};
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Produces("application/hal+json")]
    [Route("delete/{email}")]
    public async Task<IActionResult> Remove(string email)
    {
        var owner = _context.FindOwnerByEmail(email);
        _context.DeleteOwner(owner);
        return Ok(owner);
    }

    [HttpPost]
    [Produces("application/hal+json")]
    [Route("update/{email}")]
    public async Task<IActionResult> Update(string email, [FromBody] OwnerDto owner)
    {
        dynamic result;
        try
        {
            Vehicle vehicle = null;
            if (!string.IsNullOrEmpty(owner.RegCodeVehicle))
            {
                vehicle = _context.FindVehicle(owner.RegCodeVehicle);
            }

            var ownerInContext =
                _context.FindOwnerByEmail(email);
            if (ownerInContext == null)
            {
                result = new
                {
                    message = "Такого владельца нет. Воспользуйтесь методом add",
                };
                return BadRequest(result);
            }

            var oldEmail = ownerInContext.Email;

            ownerInContext.FirstName = owner.FirstName;
            ownerInContext.MiddleName = owner.MiddleName;
            ownerInContext.LastName = owner.LastName;
            ownerInContext.Email = owner.Email;
            ownerInContext.Vehicle = vehicle;

            _context.UpdateOwner(ownerInContext, oldEmail);

            return await GetByEmail(ownerInContext.Email);
        }
        catch (Exception e)
        {
            result = new {message = e.Message};
        }

        return BadRequest(result);
    }

    private Owner CreateOwner(OwnerDto owner, Vehicle vehicle)
    {
        Owner newOwner = new Owner
        {
            FirstName = owner.FirstName,
            MiddleName = owner.MiddleName,
            LastName = owner.LastName,
            Email = owner.Email,
        };

        newOwner.Vehicle = vehicle;

        _context.CreateOwner(newOwner);
        return newOwner;
    }

    private dynamic GetResource(Owner owner)
    {
        return GetResource(owner, null);
    }

    private dynamic GetResource(Owner owner, string email = null)
    {
        if (email != null && owner.Email != email)
        {
            return null;
        }

        var pathOwner = "/api/owners/";
        var pathVehicle = "/api/vehicles/";
        var ownerDynamic = owner.ToDynamic();

        dynamic links = new ExpandoObject();
        links.self = new
        {
            href = $"{pathOwner}{owner.Email}"
        };
        if (owner.Vehicle != null)
            links.vehicle = new
            {
                href = $"{pathVehicle}{owner.Vehicle.Registration}"
            };

        ownerDynamic._links = links;
        ownerDynamic.actions = new
        {
            update = new
            {
                href = $"/api/owners/update"
            },
            delete = new
            {
                href = $"/api/owners/delete/{owner.Email}"
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