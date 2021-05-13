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
    public class ShopController : ControllerBase
    {
        private readonly ShopService service;

        public ShopController(ShopService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IEnumerable<Shop> GetAllShops()
        {
            return service.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<Shop> GetShop(int id)
        {
            Shop item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpPost]
        public Shop PostShop(Shop item)
        {
            item = service.Add(item);
            return item;
        }
        [HttpPut("{id}")]
        public IActionResult PutShop(int id, Shop shop)
        {
            //return BadRequest();

            shop.Id = id;
            if (!service.Update(shop))
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteShop(int id)
        {
            Shop item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            service.Remove(id);
            return Ok();
        }
    }
}
