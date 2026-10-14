using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Enum;

namespace MiA_Lr_3.Services.Interface
{
    public interface ICarContractService
    {
        List<CarContract> GetAll();
        CarContract? GetById(int id);
        CarContract Create(CarContract contract);
        bool Update(int id, CarContract contract);
        bool Delete(int id);

        // Бізнес логіка
        // Укладання договору
        CarContract CreateRental(
            int clientId,
            int carId,
            DateTime plannedEndDate,
            InsuranceType insurance);
        // Розрахунок вартості
        decimal CalculaateRentalCost(
            decimal daikyRate,
            DateTime startDate,
            DateTime plannedEdDate,
            InsuranceType insurence);
        // Повернення автомобіля
        CarContract ReturnCar(
            int contractId,
            int endMileage,
            decimal damageCost);
            
            
    
    }
}
