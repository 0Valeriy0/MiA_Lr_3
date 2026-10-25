using MiA_Lr_3.Models;

namespace MiA_Lr_3.Repositories.Interface
{
    public interface ICarRepository
    {
        Task<IReadOnlyList<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(string id);
        Task<Car> CreateAsync(Car car);
        Task<bool> ReplaceAsync(string id, Car car);
        Task<bool> DeleteAsync(string id);
    }
}
