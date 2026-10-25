using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiA_Lr_3.Models.Documents;

public class ClientDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("full_name")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("birth_date")]
    public DateTime? BirthDate { get; set; }

    [BsonElement("driving_experience_years")]
    public int DrivingExperienceYears { get; set; }

    [BsonElement("license_number")]
    public string LicenseNumber { get; set; } = string.Empty;

    [BsonElement("balance")]
    public decimal Balance { get; set; }
}
