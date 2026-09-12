using System.ComponentModel.DataAnnotations;

namespace PetShop.Models
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Zəhmət olmasa ad və soyadınızı daxil edin.")]
        [StringLength(100, ErrorMessage = "Ad və soyad 100 simvoldan artıq ola bilməz.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zəhmət olmasa e-poçt ünvanınızı daxil edin.")]
        [EmailAddress(ErrorMessage = "Düzgün e-poçt formatı daxil edin.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Düzgün telefon nömrəsi daxil edin.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Zəhmət olmasa şifrə təyin edin.")]
        [MinLength(6, ErrorMessage = "Şifrə ən azı 6 simvoldan ibarət olmalıdır.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zəhmət olmasa şifrəni təkrar daxil edin.")]
        [Compare("Password", ErrorMessage = "Daxil edilən şifrələr uyğun gəlmir.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
