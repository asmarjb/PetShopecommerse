using System.ComponentModel.DataAnnotations;

namespace PetShop.Areas.Admin.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Display(Name = "New Password (leave blank to keep current)")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmNewPassword { get; set; }

        public string? ProfileImageUrl { get; set; }
        public IFormFile? ProfileImageFile { get; set; }
        public bool RemoveProfileImage { get; set; }
    }
}
