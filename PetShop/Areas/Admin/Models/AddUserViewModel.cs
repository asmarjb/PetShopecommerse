using System.ComponentModel.DataAnnotations;

namespace PetShop.Areas.Admin.Models
{
    public class AddUserViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; } = true;
    }
}
