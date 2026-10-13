using System.Text.Json;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Services
{
    public class CarService : ICarService
    {
        private readonly string filePath =
            Path.Combine("Storage", "cars.json");

        public CarService()
        {
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory("Storage");
                File.WriteAllText(filePath, "[]");
            }
        }

        private List<Car> ReadCars()
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<Car>>(json)
                   ?? new List<Car>();
        }

        private void WriteCars(List<Car> cars)
        {
            string json = JsonSerializer.Serialize(
                cars,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(filePath, json);
        }

        // CRUD operations
        public List<Car> GetAll()
        {
            return ReadCars();
        }

        public Car? GetById(int id)
        {
            return ReadCars().FirstOrDefault(c => c.Id == id);
        }

        public Car Create(Car car)
        {
            var cars = ReadCars();

            car.Id = cars.Count == 0
                ? 1
                : cars.Max(c => c.Id) + 1;

            cars.Add(car);
            WriteCars(cars);

            return car;
        }

        public bool Update(int id, Car car)
        {
            var cars = ReadCars();

            int index = cars.FindIndex(c => c.Id == id);

            if (index == -1)
                return false;

            car.Id = id;
            cars[index] = car;

            WriteCars(cars);

            return true;
        }

        public bool Delete(int id)
        {
            var cars = ReadCars();

            var car = cars.FirstOrDefault(c => c.Id == id);

            if (car == null)
                return false;

            cars.Remove(car);
            WriteCars(cars);

            return true;
        }
    }
}

