using MiA_Lr_3.Models;
using MiA_Lr_3.Repositories.Interface;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<IReadOnlyList<Car>> GetAllAsync() =>
            await _carRepository.GetAllAsync();

        public async Task<Car?> GetByIdAsync(string id) =>
            await _carRepository.GetByIdAsync(id);

        public async Task<Car> CreateAsync(Car car) =>
            await _carRepository.CreateAsync(car);

        public async Task<bool> UpdateAsync(string id, Car car) =>
            await _carRepository.ReplaceAsync(id, car);

        public async Task<bool> DeleteAsync(string id) =>
            await _carRepository.DeleteAsync(id);
    }
}