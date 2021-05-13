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
    public class ProductShopController : ControllerBase
    {
        private readonly ProductShopService service;

        public ProductShopController(ProductShopService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IEnumerable<ProductShop> GetAllProductShops()
        {
            return service.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<ProductShop> GetProductShop(int id)
        {
            ProductShop item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpPost]
        public ProductShop PostProductShop(ProductShop item)
        {
            item = service.Add(item);
            return item;
        }
        [HttpPut("{id}")]
        public IActionResult PutProductShop(int id, ProductShop productShop)
        {
            //return BadRequest();

            productShop.Id = id;
            if (!service.Update(productShop))
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteProductShop(int id)
        {
            ProductShop item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            service.Remove(id);

            return Ok();
        }
    }
}
