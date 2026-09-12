namespace PetShop.DataContext.Entites
{
    public class Order
    {
        public int Id { get; set; }
        
        public string SessionId { get; set; } = string.Empty; // For guest checkout or identify user
        public int? UserId { get; set; } // If we add registered users later
        
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? OrderNotes { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string? CouponCode { get; set; }

        public string OrderStatus { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed
        public string PaymentMethod { get; set; } = "CashOnDelivery"; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
