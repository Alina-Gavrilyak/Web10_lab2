using DataAccessContracts.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contracts.Repositories;
using Contracts.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository repository;

        public ProductController(IProductRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<ProductDTO> GetAllProducts()
        {
            return repository.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<ProductDTO> GetProduct(int id) {
            ProductDTO item = repository.Get(id);
            if (item == null) {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public int PostProduct(ProductInputDTO item) {
            return repository.Add(item);
        }

        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, ProductInputDTO product) {
            if (!repository.Update(id, product)) {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id) {
            if (repository.Remove(id))
                return Ok();
            else
                return BadRequest();
        }
    }
}
