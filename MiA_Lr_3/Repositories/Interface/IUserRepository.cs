using MiA_Lr_3.Models;

namespace MiA_Lr_3.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<bool> UpdateAsync(string id, User user);
    }
}
