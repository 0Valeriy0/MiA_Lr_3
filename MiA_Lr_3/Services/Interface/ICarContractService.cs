using MiA_Lr_3.Models;

namespace MiA_Lr_3.Services.Interface
{
    public interface ICarContractService
    {
        List<CarContract> GetAll();
        CarContract? GetById(int id);
        CarContract Create(CarContract contract);
        bool Update(int id, CarContract contract);
        bool Delete(int id);
    }
}
