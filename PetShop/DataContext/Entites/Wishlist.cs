namespace PetShop.DataContext.Entites
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string SessionId { get; set; } = null!;
        public string? UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
