namespace PetShopEcommerse.Models
{
    public class BasketItemViewModel
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class BasketItemDetailViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? VariantLabel { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public bool HasPriceChanged => UnitPrice != CurrentPrice;
        public int Quantity { get; set; }
        public string ProductImagePath { get; set; } = null!;
        public int AvailableStock { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;
    }

    public class BasketViewModel
    {
        public List<BasketItemDetailViewModel> Items { get; set; } = new();

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice); // Subtotal
        public int TotalItemCount => Items.Sum(i => i.Quantity);
        public bool IsEmpty => Items.Count == 0;

        // Coupon & Discounts
        public string? AppliedCouponCode { get; set; }
        public decimal DiscountAmount { get; set; } = 0m;
        public decimal? DiscountPercent { get; set; }

        // Shipping calculation (50+ is free, otherwise 4.99)
        public const decimal FreeShippingThreshold = 50.00m;
        public const decimal StandardShippingFee = 4.99m;

        public decimal ShippingFee => (IsEmpty || TotalPrice >= FreeShippingThreshold) ? 0.00m : StandardShippingFee;
        public bool IsFreeShipping => TotalPrice >= FreeShippingThreshold && !IsEmpty;
        public decimal AmountNeededForFreeShipping => (TotalPrice >= FreeShippingThreshold || IsEmpty) ? 0.00m : (FreeShippingThreshold - TotalPrice);
        public decimal FreeShippingProgress => TotalPrice >= FreeShippingThreshold ? 100m : (TotalPrice / FreeShippingThreshold * 100m);

        // Final total payable
        public decimal FinalTotal => Math.Max(0, TotalPrice - DiscountAmount + ShippingFee);
    }

    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateQuantityDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveFromCartDto
    {
        public int? CartItemId { get; set; }
        public int? ProductId { get; set; }
        public int? ProductVariantId { get; set; }
    }

    public class ApplyCouponDto
    {
        public string Code { get; set; } = string.Empty;
    }

    public class BasketResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public BasketViewModel? Data { get; set; }
    }
}
