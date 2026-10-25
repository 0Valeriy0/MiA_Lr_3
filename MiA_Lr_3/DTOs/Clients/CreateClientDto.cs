using System.ComponentModel.DataAnnotations;
namespace MiA_Lr_3.Dtos.Clients
{
    public class CreateClientDto
    {
        [Required(ErrorMessage = "Поле імені є обов'язковим")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Ім'я має містити від 3 до 100 символів")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Дата народження є обов'язковою")]
        public DateTime? BirthDate { get; set; }
        [Range(0, 80, ErrorMessage = "Водійський стаж має бути від 0 до 80 років")]
        public int DrivingExperienceYears { get; set; }
        [Required(ErrorMessage = "Номер водійського посвідчення є обов'язковим")]
        [RegularExpression(@"^[A-Z]{3}[0-9]{6}$", ErrorMessage = "Формат посвідчення: ABC123456")]
        public string LicenseNumber { get; set; } = string.Empty;
        [Range(0, 100000000, ErrorMessage = "Баланс не може бути від'ємним")]
        public decimal Balance { get; set; }
    }
}
