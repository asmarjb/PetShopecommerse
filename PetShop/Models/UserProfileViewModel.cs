using System.ComponentModel.DataAnnotations;
using PetShop.DataContext.Entites;

namespace PetShop.Models
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad və soyad boş buraxıla bilməz.")]
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Əlaqə nömrəsi boş buraxıla bilməz.")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Order> Orders { get; set; } = new();
    }
}
