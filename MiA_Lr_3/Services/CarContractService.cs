using System.Text.Json;
using MiA_Lr_3.Models;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Services
{
    public class CarContractService : ICarContractService
    {
        private readonly string filePath =
            Path.Combine("Storage", "contracts.json");

        public CarContractService()
        {
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory("Storage");
                File.WriteAllText(filePath, "[]");
            }
        }

        private List<CarContract> ReadContracts()
        {
            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<List<CarContract>>(json)
                   ?? new List<CarContract>();
        }

        private void WriteContracts(List<CarContract> contracts)
        {
            string json = JsonSerializer.Serialize(
                contracts,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(filePath, json);
        }

        // CRUD operations
        public List<CarContract> GetAll()
        {
            return ReadContracts();
        }

        public CarContract? GetById(int id)
        {
            return ReadContracts()
                .FirstOrDefault(c => c.Id == id);
        }

        public CarContract Create(CarContract contract)
        {
            var contracts = ReadContracts();

            contract.Id = contracts.Count == 0
                ? 1
                : contracts.Max(c => c.Id) + 1;

            contracts.Add(contract);
            WriteContracts(contracts);

            return contract;
        }

    
        public bool Update(int id, CarContract contract)
        {
            var contracts = ReadContracts();

            int index = contracts.FindIndex(c => c.Id == id);

            if (index == -1)
                return false;

            contract.Id = id;
            contracts[index] = contract;

            WriteContracts(contracts);

            return true;
        }

 
        public bool Delete(int id)
        {
            var contracts = ReadContracts();

            var contract = contracts.FirstOrDefault(c => c.Id == id);

            if (contract == null)
                return false;

            contracts.Remove(contract);
            WriteContracts(contracts);

            return true;
        }
    }
}