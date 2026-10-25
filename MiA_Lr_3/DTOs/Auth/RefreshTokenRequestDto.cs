using System.ComponentModel.DataAnnotations;

namespace MiA_Lr_3.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}