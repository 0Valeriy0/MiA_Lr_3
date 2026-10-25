
using MiA_Lr_3.Models.Enum;

namespace MiA_Lr_3.Models
{
    public class CarContract
    {
        public string Id { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string CarId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public InsuranceType Insurance { get; set; }
        public ContractStatus Status { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal TotalCost { get; set; }
        public int StartMileage { get; set; }
        public int? EndMileage { get; set; }
        public decimal DamageCost { get; set; }
        public decimal ExtraMileageCharge { get; set; }

        public decimal DepositRefund { get; set; }
    }
}