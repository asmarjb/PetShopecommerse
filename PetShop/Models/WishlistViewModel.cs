namespace PetShopEcommerse.Models
{
    public class WishlistItemDetailViewModel
    {
        public int WishlistItemId { get; set; }
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public string? VariantLabel { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal FinalPrice => DiscountPrice ?? Price;
        public string ProductImagePath { get; set; } = null!;
        public int StockQuantity { get; set; }
        public bool IsOutOfStock => StockQuantity <= 0;
        public DateTime AddedAt { get; set; }
    }

    public class WishlistViewModel
    {
        public List<WishlistItemDetailViewModel> Items { get; set; } = new();

        public int TotalItemCount => Items.Count;
        public bool IsEmpty => Items.Count == 0;
    }

    public class AddToWishlistDto
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
    }

    public class RemoveFromWishlistDto
    {
        public int? WishlistItemId { get; set; }
        public int? ProductId { get; set; }
    }

    public class WishlistResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public WishlistViewModel? Data { get; set; }
    }
}
