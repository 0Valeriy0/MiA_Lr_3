using MongoDB.Driver;
using MiA_Lr_3.Data;
using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Documents;
using MiA_Lr_3.Repositories.Interface;

namespace MiA_Lr_3.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IMongoCollection<ClientDocument> _collection =
            MongoDbClient.Instance.GetCollection<ClientDocument>("clients");
        public async Task<IReadOnlyList<Client>> GetAllAsync()
        {
            var documents = await _collection.Find(_ => true).ToListAsync();
            return documents.Select(ToModel).ToList();
        }
        public async Task<Client?> GetByIdAsync(string id)
        {
            var document = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return document is null ? null : ToModel(document);
        }
        public async Task<Client> CreateAsync(Client client)
        {
            var document = new ClientDocument
            {
                FullName = client.FullName,
                BirthDate = client.BirthDate,
                DrivingExperienceYears = client.DrivingExperienceYears,
                LicenseNumber = client.LicenseNumber,
                Balance = client.Balance
            };
            await _collection.InsertOneAsync(document);
            client.Id = document.Id;
            return client;
        }
        public async Task<bool> ReplaceAsync(string id, Client client)
        {
            var document = new ClientDocument
            {
                Id = id,
                FullName = client.FullName,
                BirthDate = client.BirthDate,
                DrivingExperienceYears = client.DrivingExperienceYears,
                LicenseNumber = client.LicenseNumber,
                Balance = client.Balance
            };
            var result = await _collection.ReplaceOneAsync(x => x.Id == id, document);
            return result.MatchedCount > 0;
        }
        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }
        private static Client ToModel(ClientDocument document) => new()
        {
            Id = document.Id,
            FullName = document.FullName,
            BirthDate = document.BirthDate,
            DrivingExperienceYears = document.DrivingExperienceYears,
            LicenseNumber = document.LicenseNumber,
            Balance = document.Balance
        };
    }
}
