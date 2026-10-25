using MongoDB.Driver;
using MiA_Lr_3.Data;
using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Documents;
using MiA_Lr_3.Repositories.Interface;

namespace MiA_Lr_3.Repositories;

public class CarContractRepository : ICarContractRepository
{
    private readonly IMongoCollection<CarContractDocument> _collection =
        MongoDbClient.Instance.GetCollection<CarContractDocument>("contracts");

    public async Task<IReadOnlyList<CarContract>> GetAllAsync()
    {
        var documents = await _collection.Find(_ => true).ToListAsync();
        return documents.Select(ToModel).ToList();
    }

    public async Task<CarContract?> GetByIdAsync(string id)
    {
        var document = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return document is null ? null : ToModel(document);
    }

    public async Task<CarContract> CreateAsync(CarContract contract)
    {
        var document = new CarContractDocument
        {
            ClientId = contract.ClientId,
            CarId = contract.CarId,
            StartDate = contract.StartDate,
            PlannedEndDate = contract.PlannedEndDate,
            ActualEndDate = contract.ActualEndDate,
            Insurance = contract.Insurance,
            Status = contract.Status,
            DepositAmount = contract.DepositAmount,
            TotalCost = contract.TotalCost,
            StartMileage = contract.StartMileage,
            EndMileage = contract.EndMileage,
            DamageCost = contract.DamageCost,
            ExtraMileageCharge = contract.ExtraMileageCharge,
            DepositRefund = contract.DepositRefund
        };

        await _collection.InsertOneAsync(document);
        contract.Id = document.Id;
        return contract;
    }

    public async Task<bool> ReplaceAsync(string id, CarContract contract)
    {
        var document = new CarContractDocument
        {
            Id = id,
            ClientId = contract.ClientId,
            CarId = contract.CarId,
            StartDate = contract.StartDate,
            PlannedEndDate = contract.PlannedEndDate,
            ActualEndDate = contract.ActualEndDate,
            Insurance = contract.Insurance,
            Status = contract.Status,
            DepositAmount = contract.DepositAmount,
            TotalCost = contract.TotalCost,
            StartMileage = contract.StartMileage,
            EndMileage = contract.EndMileage,
            DamageCost = contract.DamageCost,
            ExtraMileageCharge = contract.ExtraMileageCharge,
            DepositRefund = contract.DepositRefund
        };

        var result = await _collection.ReplaceOneAsync(x => x.Id == id, document);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    private static CarContract ToModel(CarContractDocument document) => new()
    {
        Id = document.Id,
        ClientId = document.ClientId,
        CarId = document.CarId,
        StartDate = document.StartDate,
        PlannedEndDate = document.PlannedEndDate,
        ActualEndDate = document.ActualEndDate,
        Insurance = document.Insurance,
        Status = document.Status,
        DepositAmount = document.DepositAmount,
        TotalCost = document.TotalCost,
        StartMileage = document.StartMileage,
        EndMileage = document.EndMileage,
        DamageCost = document.DamageCost,
        ExtraMileageCharge = document.ExtraMileageCharge,
        DepositRefund = document.DepositRefund
    };
}