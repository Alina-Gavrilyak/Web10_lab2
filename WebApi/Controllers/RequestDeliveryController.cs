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
    public class RequestDeliveryController : ControllerBase
    {
        private readonly RequestDeliveryService service;

        public RequestDeliveryController(RequestDeliveryService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IEnumerable<RequestDelivery> GetAllRequestDeliverys()
        {
            return service.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<RequestDelivery> GetRequestDelivery(int id)
        {
            RequestDelivery item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpPost]
        public RequestDelivery PostRequestDelivery(RequestDelivery item)
        {
            item = service.Add(item);
            return item;
        }
        [HttpPut("{id}")]
        public IActionResult PutRequestDelivery(int id, RequestDelivery requestDelivery)
        {
            //return BadRequest();

            requestDelivery.Id = id;
            if (!service.Update(requestDelivery))
            {
                return NotFound();
            }

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteRequestDelivery(int id)
        {
            RequestDelivery item = service.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            service.Remove(id);

            return Ok();
        }
    }
}
