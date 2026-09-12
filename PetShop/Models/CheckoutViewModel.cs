using PetShopEcommerse.Models;

namespace PetShop.Models
{
    public class CheckoutViewModel
    {
        public BasketViewModel Basket { get; set; } = new();

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? Country { get; set; } = "Azərbaycan";
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }
        public string PaymentMethod { get; set; } = "cash";
    }
}
