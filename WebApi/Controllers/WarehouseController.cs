using DataAccessContracts.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataAccessServices.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService service;

        public WarehouseController(WarehouseService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IEnumerable<Warehouse> GetAllWarehouses()
        {
            return service.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<Warehouse> GetWarehouse(int id)
        {
            Warehouse item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpPost]
        public Warehouse PostWarehouse(Warehouse item)
        {
            item = service.Add(item);
            return item;
        }
        [HttpPut("{id}")]
        public IActionResult PutWarehouse(int id, Warehouse warehouse)
        {
            //return BadRequest();

            warehouse.Id = id;
            if (!service.Update(warehouse))
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteWarehouse(int id)
        {
            Warehouse item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            service.Remove(id);
            return Ok();
        }
    }
}
