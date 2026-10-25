using MiA_Lr_3.Models;

namespace MiA_Lr_3.Services.Interface
{
    public interface ICarService
    {
        Task<IReadOnlyList<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(string id);
        Task<Car> CreateAsync(Car car);
        Task<bool> UpdateAsync(string id, Car car);
        Task<bool> DeleteAsync(string id);
    }
}
