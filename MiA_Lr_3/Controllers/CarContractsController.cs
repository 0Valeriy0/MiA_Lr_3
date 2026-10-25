using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MiA_Lr_3.Dtos.Contracts;
using MiA_Lr_3.DTOs.Contracts;
using MiA_Lr_3.Models.Enum;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarContractsController : ControllerBase
    {
        private readonly ICarContractService _contractService;
        private readonly IMapper _mapper;

        public CarContractsController(ICarContractService contractService, IMapper mapper)
        {
            _contractService = contractService;
            _mapper = mapper;
        }

        // GET: api/carcontracts
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ContractResponseDto>>> GetAll()
        {
            var contracts = await _contractService.GetAllAsync();
            return Ok(_mapper.Map<IReadOnlyList<ContractResponseDto>>(contracts));
        }

        // GET: api/carcontracts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ContractResponseDto>> GetById(string id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null)
                return NotFound();

            return Ok(_mapper.Map<ContractResponseDto>(contract));
        }

        // DELETE: api/carcontracts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _contractService.DeleteAsync(id))
                return NotFound();

            return NoContent();
        }

        // POST: api/carcontracts/calculate-cost (Бізнес-задача 2: Розрахунок вартості)
        [HttpPost("calculate-cost")]
        public ActionResult<decimal> CalculateCost(decimal dailyRate, DateTime startDate, DateTime plannedEndDate, InsuranceType insurance)
        {
            try
            {
                var cost = _contractService.CalculateRentalCost(dailyRate, startDate, plannedEndDate, insurance);
                return Ok(new { TotalCost = cost });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/carcontracts/rent (Бізнес-задача 1: Створення договору та оренда)
        [HttpPost("rent")]
        public async Task<ActionResult<ContractResponseDto>> RentCar([FromBody] RentCarRequestDto dto)
        {
            try
            {
                var contract = await _contractService.CreateRentalAsync(
                    dto.ClientId,
                    dto.CarId,
                    dto.PlannedEndDate,
                    dto.Insurance);

                var response = _mapper.Map<ContractResponseDto>(contract);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = response.Id },
                    response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/carcontracts/{id}/return (Бізнес-задача 3: Повернення автомобіля)
        [HttpPost("{id}/return")]
        public async Task<ActionResult<ContractResponseDto>> ReturnCar(string id, [FromBody] ReturnCarRequestDto dto)
        {
            try
            {
                var contract = await _contractService.ReturnCarAsync(id, dto.EndMileage, dto.DamageCost);
                return Ok(_mapper.Map<ContractResponseDto>(contract));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}