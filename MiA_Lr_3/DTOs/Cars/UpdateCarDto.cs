
namespace MiA_Lr_3.DTOs.Cars
{
    public class UpdateCarDto
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public int CurrentMileage { get; set; }
        public bool IsAvailable { get; set; }
    }
}
