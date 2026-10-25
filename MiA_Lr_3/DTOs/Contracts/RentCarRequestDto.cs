using System.ComponentModel.DataAnnotations;
using MiA_Lr_3.Models.Enum;

namespace MiA_Lr_3.DTOs.Contracts
{
/*    Для бізнес-задачі 1*/
    public class RentCarRequestDto
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;
        [Required]
        public string CarId { get; set; } = string.Empty;
        [Required]
        public DateTime PlannedEndDate { get; set; }
        public InsuranceType Insurance { get; set; }
    }
}
