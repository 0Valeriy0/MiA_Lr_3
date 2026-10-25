using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MiA_Lr_3.Models.Documents
{
    public class UserDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
        [BsonElement("role")]
        public string Role { get; set; } = "User";
        [BsonElement("refreshToken")]
        public string? RefreshToken { get; set; }
        [BsonElement("refreshTokenExpiryTime")]
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}