using MiA_Lr_3.Models;

namespace MiA_Lr_3.Repositories.Interface;

public interface ICarContractRepository
{
    Task<IReadOnlyList<CarContract>> GetAllAsync();
    Task<CarContract?> GetByIdAsync(string id);
    Task<CarContract> CreateAsync(CarContract contract);
    Task<bool> ReplaceAsync(string id, CarContract contract);
    Task<bool> DeleteAsync(string id);
}
