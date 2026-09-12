namespace PetShop.DataContext.Entites
{
    public class WishlistItem
    {
        public int Id { get; set; }

        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
