using System.ComponentModel.DataAnnotations;

namespace MiA_Lr_3.Dtos.Clients
{
    public class UpdateClientDto
    {
        [Range(0, 80, ErrorMessage = "Водійський стаж має бути від 0 до 80 років")]
        [Required(ErrorMessage = "Номер водійського посвідчення є обов'язковим")]
       
        [RegularExpression(@"^[A-Z]{3}[0-9]{6}$", ErrorMessage = "Формат посвідчення: ABC123456")]
        public string LicenseNumber { get; set; } = string.Empty;

        public decimal Balance { get; set; }
    }
}