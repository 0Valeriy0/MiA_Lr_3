using MiA_Lr_3.Models;
namespace MiA_Lr_3.Repositories.Interface
{
    public interface IClientRepository
    {
        Task<IReadOnlyList<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(string id);
        Task<Client> CreateAsync(Client client);
        Task<bool> ReplaceAsync(string id, Client client);
        Task<bool> DeleteAsync(string id);

    }
}
