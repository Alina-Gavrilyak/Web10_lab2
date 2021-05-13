using DataAccessContracts.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessServices.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductWarehouseController : ControllerBase
    {
        private readonly ProductWarehouseService service;

        public ProductWarehouseController(ProductWarehouseService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IEnumerable<ProductWarehouse> GetAllProductWarehouses()
        {
            return service.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<ProductWarehouse> GetProductWarehouse(int id)
        {
            ProductWarehouse item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpPost]
        public ProductWarehouse PostProductWarehouse(ProductWarehouse item)
        {
            item = service.Add(item);
            return item;
        }
        [HttpPut("{id}")]
        public IActionResult PutProductWarehouse(int id, ProductWarehouse productWarehouse)
        {
            //return BadRequest();

            productWarehouse.Id = id;
            if (!service.Update(productWarehouse))
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteProductWarehouse(int id)
        {
            ProductWarehouse item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            service.Remove(id);

            return Ok();
        }
    }
}
