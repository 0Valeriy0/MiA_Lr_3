
using MiA_Lr_3.Models;

namespace MiA_Lr_3.Services.Interface
{
    public interface IClientService
    {
        Task<IReadOnlyList<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(string id);
        Task<Client> CreateAsync(Client client);
        Task<bool> UpdateAsync(string id, Client client);
        Task<bool> DeleteAsync(string id);
    }
}

