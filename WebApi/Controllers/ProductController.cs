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
    public class ProductController : ControllerBase
    {
        private readonly ProductService service;

        public ProductController(ProductService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            return service.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            Product item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpPost]
        public Product PostProduct(Product item)
        {
            item = service.Add(item);
            return item;
        }
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            //return BadRequest();

            product.Id = id;
            if (!service.Update(product))
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            Product item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            service.Remove(id);

            return Ok();
        }
    }
}
