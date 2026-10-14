using MiA_Lr_3.Models;

namespace MiA_Lr_3.Services.Interface
{
    public interface ICarService
    {
        List<Car> GetAll();
        Car? GetById(int id);
        Car Create(Car car);
        bool Update(int id, Car car);
        bool Delete(int id);
    }
}
