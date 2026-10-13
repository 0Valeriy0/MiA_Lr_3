using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IValidator<Car> _carValidator;

        public CarsController(
            ICarService carService,
            IValidator<Car> carValidator)
        {
            _carService = carService;
            _carValidator = carValidator;
        }

        // GET: api/cars
        [HttpGet]
        public ActionResult<List<Car>> GetAll()
        {
            return Ok(_carService.GetAll());
        }

        // GET: api/cars/1
        [HttpGet("{id:int}")]
        public ActionResult<Car> GetById(int id)
        {
            var car = _carService.GetById(id);

            if (car == null)
                return NotFound();

            return Ok(car);
        }

        // POST: api/cars
        [HttpPost]
        public ActionResult<Car> Create(Car car)
        {
            var result = _carValidator.Validate(car);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(error => new
                {
                    error.PropertyName,
                    error.ErrorMessage
                }));
            }

            var createdCar = _carService.Create(car);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdCar.Id },
                createdCar);
        }

        // PUT: api/cars/1
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, Car car)
        {
            var result = _carValidator.Validate(car);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(error => new
                {
                    error.PropertyName,
                    error.ErrorMessage
                }));
            }

            if (!_carService.Update(id, car))
                return NotFound();

            return NoContent();
        }

        // DELETE: api/cars/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (!_carService.Delete(id))
                return NotFound();

            return NoContent();
        }
    }
}

