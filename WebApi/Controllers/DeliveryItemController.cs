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
    public class DeliveryItemController : ControllerBase
    {
        private readonly DeliveryItemService service;

        public DeliveryItemController(DeliveryItemService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IEnumerable<DeliveryItem> GetAllDeliveryItems()
        {
            return service.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<DeliveryItem> GetDeliveryItem(int id)
        {
            DeliveryItem item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpPost]
        public DeliveryItem PostDeliveryItem(DeliveryItem item)
        {
            item = service.Add(item);
            return item;
        }
        [HttpPut("{id}")]
        public IActionResult PutDeliveryItem(int id, DeliveryItem deliveryItem)
        {
            //return BadRequest();

            deliveryItem.Id = id;
            if (!service.Update(deliveryItem))
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteDeliveryItem(int id)
        {
            DeliveryItem item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            service.Remove(id);

            return Ok();
        }
    }
}
