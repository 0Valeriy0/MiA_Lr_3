using MiA_Lr_3.Models.Enum;

namespace MiA_Lr_3.DTOs.Cars
{
    public class CarResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public CarClass Class { get; set; }
        public decimal DailyRate { get; set; }
        public int CurrentMileage { get; set; }
        public bool IsAvailable { get; set; }
    }
}
