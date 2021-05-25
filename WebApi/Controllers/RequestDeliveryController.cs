using DataAccessContracts.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Repositories;
using Contracts.Models;

namespace WebApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RequestDeliveryController : ControllerBase {
        private readonly IRequestDeliveryRepository repository;

        public RequestDeliveryController(IRequestDeliveryRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<RequestDeliveryDTO> GetAllRequestDeliverys() {
            return repository.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<RequestDeliveryDTO> GetRequestDelivery(int id) {
            RequestDeliveryDTO item = repository.Get(id);
            if (item == null) {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public int PostRequestDelivery(RequestDeliveryInputDTO item) {
            return repository.Add(item);
        }

        [HttpPut("{id}")]
        public IActionResult PutRequestDelivery(int id, RequestDeliveryInputDTO requestDelivery) {
            if (!repository.Update(id, requestDelivery)) {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRequestDelivery(int id) {
            if (repository.Remove(id))
                return Ok();
            else
                return BadRequest();
        }
    }
}
