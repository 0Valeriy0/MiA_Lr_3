using System.ComponentModel.DataAnnotations;
namespace MiA_Lr_3.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [MinLength(6, ErrorMessage = "Пароль повинен містити щонайменше 6 символів")]
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // "User" або "Admin"
    }
}
