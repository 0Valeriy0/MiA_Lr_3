using MiA_Lr_3.Models.Documents;
using MiA_Lr_3.Models.Enum;
using MongoDB.Driver;

namespace CarRental.IntegrationTests.Infrastructure
{

    /// Наповнює тестову БД фіксованими даними перед кожним тестом,
    /// щоб тести не впливали один на одного.

    public class DatabaseSeeder
    {
        // Фіксовані Id, щоб тести знали, до яких записів звертатися.
        // Це валідні ObjectId — рівно 24 шістнадцяткові символи.
        public const string Car1Id = "000000000000000000000001";
        public const string Car2Id = "000000000000000000000002";
        public const string Car3Id = "000000000000000000000003";

        private readonly IMongoDatabase _db;

        public DatabaseSeeder(IMongoDatabase db)
        {
            _db = db;
        }

        public async Task SeedCarsAsync()
        {
            var collection = _db.GetCollection<CarDocument>("cars");

            // 1. Очищення колекції
            await collection.DeleteManyAsync(FilterDefinition<CarDocument>.Empty);

            // 2. Початкові дані
            var cars = new List<CarDocument>
            {
                new() { Id = Car1Id, Brand = "Toyota", Model = "Corolla", RegistrationNumber = "AA1111AA",
                        Class = CarClass.Economy, DailyRate = 1000m, CurrentMileage = 10000, IsAvailable = true },
                new() { Id = Car2Id, Brand = "BMW", Model = "X5", RegistrationNumber = "BB2222BB",
                        Class = CarClass.SUV, DailyRate = 3000m, CurrentMileage = 5000, IsAvailable = true },
                new() { Id = Car3Id, Brand = "Mercedes", Model = "S-Class", RegistrationNumber = "CC3333CC",
                        Class = CarClass.Premium, DailyRate = 6000m, CurrentMileage = 2000, IsAvailable = false }
            };

            await collection.InsertManyAsync(cars);
        }
    }
}
