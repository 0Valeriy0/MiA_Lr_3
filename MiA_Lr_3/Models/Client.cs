
using System.ComponentModel.DataAnnotations;

namespace MiA_Lr_3.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Вкажіть ПІБ клієнта")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "ПІБ має містити від 3 до 100 символів")]
        [RegularExpression(@"^[\p{L}\s'-]+$", ErrorMessage = "ПІБ може містити лише літери, пробіли, апострофи та дефіси")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть дату народження")]
        public DateTime? BirthDate { get; set; }

        [Range(0, 80,
            ErrorMessage = "Водійський стаж має бути від 0 до 80 років")]
        public int DrivingExperienceYears { get; set; }

        [Required(ErrorMessage = "Вкажіть номер водійського посвідчення")]
        [RegularExpression(@"^[A-Z]{3}[0-9]{6}$",
            ErrorMessage = "Номер посвідчення має відповідати формату ABC123456")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "100000000", ErrorMessage = "Баланс має бути від 0 до 100000000")]
        public decimal Balance { get; set; }
    }
}
