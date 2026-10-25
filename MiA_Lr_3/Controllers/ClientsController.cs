
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
        public async Task<ActionResult<List<Client>>> GetAll()
        {
            return Ok(await _clientService.GetAllAsync());
        }

        // GET: api/clients/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(string id)
        {
            var client = await _clientService.GetByIdAsync(id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<Client>> Create(Client client)
        {
            var createdClient = await _clientService.CreateAsync(client);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdClient.Id },
                createdClient);
        }

        // PUT: api/clients/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Client client)
        {
            if (!await _clientService.UpdateAsync(id, client))
                return NotFound();

            return NoContent();
        }

        // DELETE: api/clients/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _clientService.DeleteAsync(id))
                return NotFound();

            return NoContent();
        }
    }
}

