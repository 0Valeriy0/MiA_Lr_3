using MiA_Lr_3.Data;
using MiA_Lr_3.Models;
using MiA_Lr_3.Models.Documents;
using MiA_Lr_3.Repositories.Interface;
using MongoDB.Driver;

namespace MiA_Lr_3.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserDocument> _users;

        public UserRepository()
        {
            _users = MongoDbClient.Instance.GetCollection<UserDocument>("users");
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var doc = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            return doc != null ? ToModel(doc) : null;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var doc = await _users.Find(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
            return doc != null ? ToModel(doc) : null;
        }

        public async Task<User> CreateAsync(User user)
        {
            var doc = ToDocument(user);
            await _users.InsertOneAsync(doc);
            user.Id = doc.Id!;
            return user;
        }

        public async Task<bool> UpdateAsync(string id, User user)
        {
            var doc = ToDocument(user);
            doc.Id = id;
            var result = await _users.ReplaceOneAsync(u => u.Id == id, doc);
            return result.ModifiedCount > 0;
        }

        private static User ToModel(UserDocument doc) => new()
        {
            Id = doc.Id ?? string.Empty,
            Email = doc.Email,
            PasswordHash = doc.PasswordHash,
            Role = doc.Role,
            RefreshToken = doc.RefreshToken,
            RefreshTokenExpiryTime = doc.RefreshTokenExpiryTime
        };

        private static UserDocument ToDocument(User model) => new()
        {
            Id = string.IsNullOrEmpty(model.Id) ? null : model.Id,
            Email = model.Email,
            PasswordHash = model.PasswordHash,
            Role = model.Role,
            RefreshToken = model.RefreshToken,
            RefreshTokenExpiryTime = model.RefreshTokenExpiryTime
        };
    }
}