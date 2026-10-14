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
        private readonly IValidator<CarContract> _contractValidator;

        public CarContractsController(
            ICarContractService contractService,
            IValidator<CarContract> contractValidator)
        {
            _contractService = contractService;
            _contractValidator = contractValidator;
        }

        // GET: api/carcontracts
        [HttpGet]
        public ActionResult<List<CarContract>> GetAll()
        {
            return Ok(_contractService.GetAll());
        }

        // GET: api/carcontracts/1
        [HttpGet("{id:int}")]
        public ActionResult<CarContract> GetById(int id)
        {
            var contract = _contractService.GetById(id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        // POST: api/carcontracts
        [HttpPost]
        public ActionResult<CarContract> Create(CarContract contract)
        {
            var validationResult = _contractValidator.Validate(contract);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(
                    error => new
                    {
                        error.PropertyName,
                        error.ErrorMessage
                    }));
            }

            var createdContract = _contractService.Create(contract);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdContract.Id },
                createdContract);
        }

        // PUT: api/carcontracts/1
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, CarContract contract)
        {
            var validationResult = _contractValidator.Validate(contract);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(
                    error => new
                    {
                        error.PropertyName,
                        error.ErrorMessage
                    }));
            }

            if (!_contractService.Update(id, contract))
                return NotFound();

            return NoContent();
        }

        // DELETE: api/carcontracts/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (!_contractService.Delete(id))
                return NotFound();

            return NoContent();
        }


        // POST: api/carcontracts/calculate-cost (Бізнес-задача 2)
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
        // POST: api/carcontracts/rent (Бізнес-задача 1)
        [HttpPost("rent")]
        public ActionResult<CarContract> RentCar(int clientId, int carId, DateTime plannedEndDate, InsuranceType insurance)
        {
            try
            {
                var contract = _contractService.CreateRental(clientId, carId, plannedEndDate, insurance);
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
        public ActionResult<CarContract> ReturnCar(int id, int endMileage, decimal damageCost = 0)
        {
            try
            {
                var contract = _contractService.ReturnCar(id, endMileage, damageCost);
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
