using MiA_Lr_3.Models.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiA_Lr_3.Models.Documents
{

    public class CarContractDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("client_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClientId { get; set; } = string.Empty;
        [BsonElement("car_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CarId { get; set; } = string.Empty;
        [BsonElement("start_date")]
        public DateTime StartDate { get; set; }
        [BsonElement("planned_end_date")]
        public DateTime PlannedEndDate { get; set; }
        [BsonElement("actual_end_date")]
        public DateTime? ActualEndDate { get; set; }
        [BsonElement("insurance")]
        [BsonRepresentation(BsonType.String)]
        public InsuranceType Insurance { get; set; }
        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public ContractStatus Status { get; set; }
        [BsonElement("deposit_amount")]
        public decimal DepositAmount { get; set; }
        [BsonElement("total_cost")]
        public decimal TotalCost { get; set; }
        [BsonElement("start_mileage")]
        public int StartMileage { get; set; }
        [BsonElement("end_mileage")]
        public int? EndMileage { get; set; }
        [BsonElement("damage_cost")]
        public decimal DamageCost { get; set; }
        [BsonElement("extra_mileage_charge")]
        public decimal ExtraMileageCharge { get; set; }
        [BsonElement("deposit_refund")]
        public decimal DepositRefund { get; set; }
    }
}

