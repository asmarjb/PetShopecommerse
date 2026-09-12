using System;
using System.ComponentModel.DataAnnotations;

namespace PetShop.DataContext.Entites
{
    public class ProductReview
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? UserId { get; set; }
        public AppUser? User { get; set; }

        [Required]
        [MaxLength(150)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Comment { get; set; } = string.Empty;

        public bool IsVerifiedPurchase { get; set; } = true;

        public bool IsApproved { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
