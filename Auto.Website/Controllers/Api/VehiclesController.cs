using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Messages;
using Auto.Website.Models;
using Castle.Core.Internal;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;


namespace Auto.Website.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IAutoDatabase db;
        private readonly IBus _bus;

        public VehiclesController(IAutoDatabase db, IBus bus)
        {
            this.db = db;
            _bus = bus;
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

        // GET: api/vehicles
        [HttpPost]
        [Produces("application/hal+json")]
        public IActionResult Get(int index = 0, int count = 10)
        {
            var items = db.ListVehicles().Skip(index).Take(count).Select(GetResource);
            var total = db.CountVehicles();
            var _links = Paginate("/api/vehicles", index, count, total);
            var _actions = new
            {
                create = new
                {
                    method = "POST",
                    type = "application/json",
                    name = "Create a new vehicle",
                    href = "/api/vehicles"
                },
                delete = new
                {
                    method = "DELETE",
                    name = "Delete a vehicle",
                    href = "/api/vehicles/{id}"
                }
            };
            var result = new
            {
                _links, _actions, index, count, total, items
            };
            return Ok(result);
        }

        // GET api/vehicles/ABC123
        [HttpPost("{id}")]
        public IActionResult Get(string id)
        {
            var vehicle = db.FindVehicle(id);
            if (vehicle == default) return NotFound();
            var json = GetResource(vehicle);
            return Ok(json);
        }

        // POST api/vehicles/add
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Post([FromBody] VehicleDto dto)
        {
            var vehicleModel = db.FindModel(dto.ModelCode);
            var vehicle = new Vehicle
            {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                VehicleModel = vehicleModel
            };
            db.CreateVehicle(vehicle);

            return Ok(dto);
        }

        // PUT api/vehicles/ABC123
        [HttpPost("update/{id}")]
        public IActionResult Put(string id, [FromBody] dynamic dto)
        {
            var vehicleModelHref = dto._links.vehicleModel.href;
            var vehicleModelId = ModelsController.ParseModelId(vehicleModelHref);
            var vehicleModel = db.FindModel(vehicleModelId);
            var vehicle = new Vehicle
            {
                Registration = id,
                Color = dto.color,
                Year = dto.year,
                ModelCode = vehicleModel.Code
            };
            db.UpdateVehicle(vehicle);
            return Get(id);
        }

        // DELETE api/vehicles/ABC123
        [HttpPost("delete/{id}")]
        public IActionResult Delete(string id)
        {
            var vehicle = db.FindVehicle(id);
            if (vehicle == default) return NotFound();
            db.DeleteVehicle(vehicle);
            return NoContent();
        }

        private dynamic
            GetResource(Vehicle vehicle)
        {
            var pathVehicle = "/api/vehicles/";
            var pathModel = "/api/modes/";
            var vehicleDynamic = vehicle.ToDynamic();

            dynamic links = new ExpandoObject();
            links.self = new
            {
                href = $"{pathVehicle}{vehicle.Registration}"
            };
            if (vehicle.VehicleModel != null)
                links.model = new
                {
                    href = $"{pathModel}{vehicle.VehicleModel.Code}"
                };

            vehicleDynamic._links = links;
            vehicleDynamic.actions = new
            {
                update = new
                {
                    href = $"/api/vehicles/update",
                    accept = "application/json"
                },
                delete = new
                {
                    href = $"/api/vehicles/delete/{vehicle.Registration}"
                }
            };
            return vehicleDynamic;
        }
    }
}