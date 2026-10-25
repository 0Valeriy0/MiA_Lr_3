using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MiA_Lr_3.Models.Enum;

namespace MiA_Lr_3.Models.Documents;

public class CarDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("brand")]
    public string Brand { get; set; } = string.Empty;

    [BsonElement("model")]
    public string Model { get; set; } = string.Empty;

    [BsonElement("registration_number")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [BsonElement("class")]
    [BsonRepresentation(BsonType.String)]
    public CarClass Class { get; set; }

    [BsonElement("daily_rate")]
    public decimal DailyRate { get; set; }

    [BsonElement("current_mileage")]
    public int CurrentMileage { get; set; }

    [BsonElement("is_available")]
    public bool IsAvailable { get; set; }
}
