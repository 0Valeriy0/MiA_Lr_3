using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Enum;

namespace MiA_Lr_3.Services.Interface
{
    public interface ICarContractService
    {
        Task<IReadOnlyList<CarContract>> GetAllAsync();
        Task<CarContract?> GetByIdAsync(string id);
        Task<CarContract> CreateAsync(CarContract contract);
        Task<bool> UpdateAsync(string id, CarContract contract);
        Task<bool> DeleteAsync(string id);

        // Бізнес-задача 1: Оренда
        Task<CarContract> CreateRentalAsync(
            string clientId,
            string carId,
            DateTime plannedEndDate,
            InsuranceType insurance);

        // Бізнес-задача 2: Розрахунок вартості
        decimal CalculateRentalCost(
            decimal dailyRate,
            DateTime startDate,
            DateTime plannedEndDate,
            InsuranceType insurance);

        // Бізнес-задача 3: Повернення автомобіля
        Task<CarContract> ReturnCarAsync(
            string contractId,
            int endMileage,
            decimal damageCost = 0);
    }
}