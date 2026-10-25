namespace MiA_Lr_3.DTOs.Clients
{
    public class ClientResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public int DrivingExperienceYears { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
