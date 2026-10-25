using MongoDB.Driver;
using MiA_Lr_3.Data;
using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Documents;
using MiA_Lr_3.Repositories.Interface;

namespace MiA_Lr_3.Repositories;

public class CarRepository : ICarRepository
{
    private readonly IMongoCollection<CarDocument> _collection =
        MongoDbClient.Instance.GetCollection<CarDocument>("cars");

    public async Task<IReadOnlyList<Car>> GetAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();
        return documents.Select(ToModel).ToList();
    }

    public async Task<Car?> GetByIdAsync(string id)
    {
        var document = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return document is null ? null : ToModel(document);
    }

    public async Task<Car> CreateAsync(Car car)
    {
        var document = new CarDocument
        {
            Brand = car.Brand,
            Model = car.Model,
            RegistrationNumber = car.RegistrationNumber,
            Class = car.Class,
            DailyRate = car.DailyRate,
            CurrentMileage = car.CurrentMileage,
            IsAvailable = car.IsAvailable
        };

        await _collection.InsertOneAsync(document);
        car.Id = document.Id;
        return car;
    }

    public async Task<bool> ReplaceAsync(string id, Car car)
    {
        var document = new CarDocument
        {
            Id = id,
            Brand = car.Brand,
            Model = car.Model,
            RegistrationNumber = car.RegistrationNumber,
            Class = car.Class,
            DailyRate = car.DailyRate,
            CurrentMileage = car.CurrentMileage,
            IsAvailable = car.IsAvailable
        };

        var result = await _collection.ReplaceOneAsync(x => x.Id == id, document);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    private static Car ToModel(CarDocument document) => new()
    {
        Id = document.Id,
        Brand = document.Brand,
        Model = document.Model,
        RegistrationNumber = document.RegistrationNumber,
        Class = document.Class,
        DailyRate = document.DailyRate,
        CurrentMileage = document.CurrentMileage,
        IsAvailable = document.IsAvailable
    };
}
