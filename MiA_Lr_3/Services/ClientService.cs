using MiA_Lr_3.Models;
using MiA_Lr_3.Repositories.Interface;
using MiA_Lr_3.Services.Interface;

namespace MiA_Lr_3.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IReadOnlyList<Client>> GetAllAsync() =>
            await _clientRepository.GetAllAsync();

        public async Task<Client?> GetByIdAsync(string id) =>
            await _clientRepository.GetByIdAsync(id);

        public async Task<Client> CreateAsync(Client client) =>
            await _clientRepository.CreateAsync(client);

        public async Task<bool> UpdateAsync(string id, Client client) =>
            await _clientRepository.ReplaceAsync(id, client);

        public async Task<bool> DeleteAsync(string id) =>
            await _clientRepository.DeleteAsync(id);
    }
}