using AutoMapper;
using MiA_Lr_3.DTOs.Cars;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiA_Lr_3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IMapper _mapper;

        public CarController( ICarService carService, IMapper mapper)
        {
            _carService = carService;
            _mapper = mapper;
        }

        // GET: api/car
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CarResponseDto>>> GetAll()
        {
            var cars = await _carService.GetAllAsync();
            return Ok(_mapper.Map<IReadOnlyList<CarResponseDto>>(cars));
        }

        // GET: api/cars/
        [HttpGet("{id}")]
        public async Task<ActionResult<CarResponseDto>> GetById(string id)
        {
            var car = await _carService.GetByIdAsync(id);

            if (car == null)
                return NotFound();

            return Ok(_mapper.Map<CarResponseDto>(car));
        }

        // POST: api/car (тільки адмін додає нові машини в автопарк)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CarResponseDto>> Create([FromBody] CreateCarDto dto)
        {
            var car = _mapper.Map<Car>(dto);
            var createdCar = await _carService.CreateAsync(car);
              var response = _mapper.Map<CarResponseDto>(createdCar);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdCar.Id },
                _mapper.Map<CarResponseDto>(createdCar));

        }

        // PUT: api/car/{id} (тільки адмін оновлює тариф чи статус)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCarDto dto)
        {
            var existingCar = await _carService.GetByIdAsync(id);
            if (existingCar == null)
                return NotFound();
            // Оновлюємо тільки змінені поля з DTO
            _mapper.Map(dto, existingCar);
            await _carService.UpdateAsync(id, existingCar);
            return NoContent();
        }

        // DELETE: api/car/{id} (тільки адмін списує авто)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _carService.DeleteAsync(id))
                return NotFound();

            return NoContent();
        }
    }
}

