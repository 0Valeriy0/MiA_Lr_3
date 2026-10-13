using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;

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
    }
}
