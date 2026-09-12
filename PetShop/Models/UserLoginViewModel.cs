using System.ComponentModel.DataAnnotations;

namespace PetShop.Models
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Zəhmət olmasa e-poçt ünvanınızı daxil edin.")]
        [EmailAddress(ErrorMessage = "Düzgün e-poçt formatı daxil edin.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zəhmət olmasa şifrənizi daxil edin.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
