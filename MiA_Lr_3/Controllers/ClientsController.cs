
using Microsoft.AspNetCore.Mvc;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // GET: api/clients
        [HttpGet]
        public ActionResult<List<Client>> GetAll()
        {
            return Ok(_clientService.GetAll());
        }

        // GET: api/clients/1
        [HttpGet("{id:int}")]
        public ActionResult<Client> GetById(int id)
        {
            var client = _clientService.GetById(id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        // POST: api/clients
        [HttpPost]
        public ActionResult<Client> Create(Client client)
        {
            var createdClient = _clientService.Create(client);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdClient.Id },
                createdClient);
        }

        // PUT: api/clients/1
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, Client client)
        {
            if (!_clientService.Update(id, client))
                return NotFound();

            return NoContent();
        }

        // DELETE: api/clients/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (!_clientService.Delete(id))
                return NotFound();

            return NoContent();
        }
    }
}

