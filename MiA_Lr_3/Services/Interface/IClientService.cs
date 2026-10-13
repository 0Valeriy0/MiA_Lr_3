
using MiA_Lr_3.Models;

namespace MiA_Lr_3.Services.Interface
{
    public interface IClientService
    {
        List<Client> GetAll();
        Client? GetById(int id);
        Client Create(Client client);
        bool Update(int id, Client client);
        bool Delete(int id);
    }
}

