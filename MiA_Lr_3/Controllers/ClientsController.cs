using AutoMapper;
using MiA_Lr_3.DTOs.Clients;
using MiA_Lr_3.Dtos.Clients;
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
        private readonly IMapper _mapper;

        public ClientsController(IClientService clientService, IMapper mapper)
        {
            _clientService = clientService;
            _mapper = mapper;
        }

        // GET: api/clients
        [HttpGet]
        public async Task<ActionResult<List<ClientResponseDto>>> GetAll()
        {
            var clients = await _clientService.GetAllAsync();
            return Ok(_mapper.Map<IReadOnlyList<ClientResponseDto>>(clients));
        }

        // GET: api/clients/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponseDto>> GetById(string id)
        {
            var client = await _clientService.GetByIdAsync(id);

            if (client == null)
                return NotFound();

            return Ok(_mapper.Map<ClientResponseDto>(client));
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<ClientResponseDto>> Create([FromBody] CreateClientDto dto)
        {
            var client = _mapper.Map<Client>(dto);
            var createdClient = await _clientService.CreateAsync(client);
            var response = _mapper.Map<ClientResponseDto>(createdClient);

            return CreatedAtAction(
                      nameof(GetById),
                      new { id = response.Id },
                      response);
        }

        // PUT: api/clients/
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateClientDto dto)
        {
            var existingClient = await _clientService.GetByIdAsync(id);
            if (existingClient == null)
                return NotFound();

            _mapper.Map(dto, existingClient);
            await _clientService.UpdateAsync(id, existingClient);
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

