using MongoDB.Driver;
using MiA_Lr_3.Data;
using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Documents;
using MiA_Lr_3.Repositories.Interface;
using AutoMapper;

namespace MiA_Lr_3.Repositories;

public class CarRepository(
    IMapper _mapper
) : ICarRepository
{
    private readonly IMongoCollection<CarDocument> _collection =
        MongoDbClient.Instance.GetCollection<CarDocument>("cars");

    public async Task<IReadOnlyList<Car>> GetAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();

        return _mapper.Map<IReadOnlyList<Car>>(documents);
    }

    public async Task<Car?> GetByIdAsync(string id)
    {
        var document = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return document is null ? null : _mapper.Map<Car>(document);
    }

    public async Task<Car> CreateAsync(Car car)
    {
        var document = _mapper.Map<CarDocument>(car);
        await _collection.InsertOneAsync(document);
        car.Id = document.Id;
        return car;
    }

    public async Task<bool> ReplaceAsync(string id, Car car)
    {
        var document = _mapper.Map<CarDocument>(car);
        document.Id = id;

        var result = await _collection.ReplaceOneAsync(x => x.Id == id, document);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
}
