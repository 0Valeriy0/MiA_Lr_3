using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(
            ICarService carService)
        {
            _carService = carService;
        }

        // GET: api/cars
        [HttpGet]
        public async  Task<ActionResult<List<Car>>> GetAll() => Ok(await _carService.GetAllAsync());

        // GET: api/cars/
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetById(string id)
        {
            var car = await _carService.GetByIdAsync(id);

            if (car == null)
                return NotFound();

            return Ok(car);
        }

        // POST: api/cars
        [HttpPost]
        public async Task<ActionResult<Car>> Create(Car car)
        {
            var createdCar = await _carService.CreateAsync(car);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdCar.Id },
                createdCar);
        }

        // PUT: api/cars/
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Car car)
        {

            if (!await _carService.UpdateAsync(id, car))
                return NotFound();

            return NoContent();
        }

        // DELETE: api/cars/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _carService.DeleteAsync(id))
                return NotFound();

            return NoContent();
        }
    }
}

