using System.ComponentModel.DataAnnotations;

namespace PetShop.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Zəhmət olmasa ad və soyadınızı qeyd edin.")]
        [StringLength(100, ErrorMessage = "Ad 100 simvoldan çox ola bilməz.")]
        [Display(Name = "Ad və Soyad")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zəhmət olmasa e-poçt ünvanınızı daxil edin.")]
        [EmailAddress(ErrorMessage = "Düzgün e-poçt ünvanı daxil edin.")]
        [Display(Name = "E-poçt")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zəhmət olmasa əlaqə nömrənizi daxil edin.")]
        [Phone(ErrorMessage = "Düzgün telefon nömrəsi daxil edin.")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zəhmət olmasa mövzunu qeyd edin.")]
        [StringLength(150, ErrorMessage = "Mövzu 150 simvoldan çox ola bilməz.")]
        [Display(Name = "Mövzu")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zəhmət olmasa mesajınızı daxil edin.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Mesaj ən azı 10 simvol olmalıdır.")]
        [Display(Name = "Mesaj")]
        public string Message { get; set; } = string.Empty;

        // Static store details (can be loaded dynamically or configured)
        public string StoreAddress { get; set; } = "Bakı şəhəri, Nizami küçəsi 45, AZ1000";
        public string StorePhone1 { get; set; } = "+994 (12) 490 00 00";
        public string StorePhone2 { get; set; } = "+994 (50) 123 45 67";
        public string StoreEmail1 { get; set; } = "info@petshop.az";
        public string StoreEmail2 { get; set; } = "destek@petshop.az";
        public string WorkingHours { get; set; } = "Bazar ertəsi - Bazar: 09:00 - 21:00";
    }
}
