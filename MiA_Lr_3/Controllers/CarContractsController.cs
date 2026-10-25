using FluentValidation;
using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Enum;
using MiA_Lr_3.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MiA_Lr_3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarContractsController : ControllerBase
    {
        private readonly ICarContractService _contractService;

        public CarContractsController(
            ICarContractService contractService)
        {
            _contractService = contractService;
        }

        // GET: api/carcontracts
        [HttpGet]
        public async Task<ActionResult<List<CarContract>>> GetAll()
        {
            return Ok(await _contractService.GetAllAsync());
        }

        // GET: api/carcontracts/
        [HttpGet("{id}")]
        public async Task<ActionResult<CarContract>> GetById(string id)
        {
            var contract = await _contractService.GetByIdAsync(id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        // POST: api/carcontracts
        [HttpPost]
        public async Task<ActionResult<CarContract>> Create([FromBody]CarContract contract)
        {   


            var createdContract = await _contractService.CreateAsync(contract);

            return CreatedAtAction( nameof(GetById),  new { id = createdContract.Id }, createdContract);
        }

        // PUT: api/carcontracts/
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, CarContract contract)
        { 

            if (!await _contractService.UpdateAsync(id, contract))
                return NotFound();

            return NoContent();
        }

        // DELETE: api/carcontracts/
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _contractService.DeleteAsync(id))
                return NotFound();

            return NoContent();
        }


        // POST: api/carcontracts/calculate-cost (Бізнес-задача 2)
        [HttpPost("calculate-cost")]
        public  ActionResult<decimal> CalculateCost(decimal dailyRate, DateTime startDate, DateTime plannedEndDate, InsuranceType insurance)
        {
            try
            {
                var cost =  _contractService.CalculateRentalCost(dailyRate, startDate, plannedEndDate, insurance);
                return Ok(new { TotalCost = cost });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        // POST: api/carcontracts/rent (Бізнес-задача 1)
        [HttpPost("rent")]
        public async Task<ActionResult<CarContract>> RentCar(string clientId, string carId, DateTime plannedEndDate, InsuranceType insurance)
        {
            try
            {
                var contract = await _contractService.CreateRentalAsync(clientId, carId, plannedEndDate, insurance);
                return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
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
        // POST: api/carcontracts/1/return (Бізнес-задача 3)
        [HttpPost("{id:int}/return")]
        public async Task<ActionResult<CarContract>> ReturnCar(string id, int endMileage, decimal damageCost = 0)
        {
            try
            {
                var contract = await _contractService.ReturnCarAsync(id, endMileage, damageCost);
                return Ok(contract);
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
