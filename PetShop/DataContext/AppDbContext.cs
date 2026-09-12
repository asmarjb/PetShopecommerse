using Microsoft.EntityFrameworkCore;
using PetShop.DataContext.Entites;

namespace PetShop.DataContext
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAnimalType> ProductAnimalTypes { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<WishlistItem> WishlistItems { get; set; } = null!;
        public DbSet<AdminUser> AdminUsers { get; set; } = null!;
        public DbSet<BlogPost> BlogPosts { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<AppUser> AppUsers { get; set; } = null!;
        public DbSet<ProductReview> ProductReviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProductReview configuration
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.HasOne(r => r.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(r => r.ProductId);
                entity.HasIndex(r => new { r.ProductId, r.UserId });
            });

            // AppUser - unique email
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // AdminUser - unique email
            modelBuilder.Entity<AdminUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // BlogPost - unique slug
            modelBuilder.Entity<BlogPost>()
                .HasIndex(b => b.Slug)
                .IsUnique();

            // BlogPost seed data
            modelBuilder.Entity<BlogPost>().HasData(
                new BlogPost
                {
                    Id = 1,
                    Title = "10 Reasons to be helpful towards any animals",
                    Slug = "10-reasons-helpful-towards-animals",
                    Summary = "At the core of our practice is the idea that cities are the incubators of our greatest achievements, and the best hope for a sustainable future.",
                    Content = "At the core of our practice is the idea that cities are the incubators of our greatest achievements, and the best hope for a sustainable future. Animals play a crucial role in maintaining the balance of our ecosystem. By being kind to animals, we not only protect biodiversity but also ensure a healthier planet for future generations.",
                    ImageUrl = "blog1.jpg",
                    Author = "Waggy Team",
                    PublishedAt = new DateTime(2026, 2, 20),
                    IsPublished = true
                },
                new BlogPost
                {
                    Id = 2,
                    Title = "How to know your pet is hungry",
                    Slug = "how-to-know-your-pet-is-hungry",
                    Summary = "At the core of our practice is the idea that cities are the incubators of our greatest achievements, and the best hope for a sustainable future.",
                    Content = "Understanding your pet's hunger cues is essential for their health and wellbeing. Pets communicate their needs in various ways — from pawing at their food bowl to vocalizing. Learn the key signs that indicate your pet needs to be fed and how to establish a healthy feeding routine.",
                    ImageUrl = "blog2.jpg",
                    Author = "Waggy Team",
                    PublishedAt = new DateTime(2026, 2, 21),
                    IsPublished = true
                },
                new BlogPost
                {
                    Id = 3,
                    Title = "Best home for your pets",
                    Slug = "best-home-for-your-pets",
                    Summary = "At the core of our practice is the idea that cities are the incubators of our greatest achievements, and the best hope for a sustainable future.",
                    Content = "Creating the perfect home environment for your pets is one of the most important things you can do as a pet owner. From choosing the right bedding to setting up safe play areas, a well-designed home keeps your pets happy, healthy, and stress-free.",
                    ImageUrl = "blog3.jpg",
                    Author = "Waggy Team",
                    PublishedAt = new DateTime(2026, 2, 22),
                    IsPublished = true
                }
            );

            // ProductAnimalType - composite key (Many-to-Many join entity)
            modelBuilder.Entity<ProductAnimalType>()
                .HasKey(pa => new { pa.ProductId, pa.AnimalTypeId });

            modelBuilder.Entity<ProductAnimalType>()
                .HasOne(pa => pa.Product)
                .WithMany(p => p.ProductAnimalTypes)
                .HasForeignKey(pa => pa.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductAnimalType>()
                .HasOne(pa => pa.AnimalType)
                .WithMany(a => a.ProductAnimalTypes)
                .HasForeignKey(pa => pa.AnimalTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductCategory - özünə istinad (Parent/Sub)
            modelBuilder.Entity<ProductCategory>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCategory>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            // Product - ProductCategory (One-to-Many)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductCategory)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Brand (One-to-Many, optional)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - ProductVariant (One-to-Many)
            modelBuilder.Entity<ProductVariant>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product - ProductImage (One-to-Many)
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.PriceModifier)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Cart>()
    .HasIndex(c => c.SessionId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.UnitPriceSnapshot)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Wishlist>()
                .HasIndex(w => w.SessionId);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Wishlist)
                .WithMany(w => w.WishlistItems)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Product)
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.ProductVariant)
                .WithMany()
                .HasForeignKey(wi => wi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order & OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductVariant)
                .WithMany()
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Coupon>()
                .Property(c => c.DiscountValue)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<Coupon>()
                .Property(c => c.MinimumOrderAmount)
                .HasColumnType("decimal(18,2)");

            // ================= SEED DATA =================

            // 1) AnimalType
            modelBuilder.Entity<AnimalType>().HasData(
                new AnimalType { Id = 1, Name = "İt" },
                new AnimalType { Id = 2, Name = "Pişik" },
                new AnimalType { Id = 3, Name = "Quş" },
                new AnimalType { Id = 4, Name = "Balıq" }
            );

            // 2) ProductCategory (Parent + Sub-category)
            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { Id = 1, Name = "Yem", Slug = "food", ParentCategoryId = null, IconClass = "ph:bowl-food" },
                new ProductCategory { Id = 2, Name = "Geyim", Slug = "cloth", ParentCategoryId = null, IconClass = "ph:t-shirt" },
                new ProductCategory { Id = 3, Name = "İt Yemi", Slug = "food-dog", ParentCategoryId = 1, IconClass = "ph:dog" },
                new ProductCategory { Id = 4, Name = "Pişik Yemi", Slug = "food-cat", ParentCategoryId = 1, IconClass = "ph:cat" },
                new ProductCategory { Id = 5, Name = "Quş Yemi", Slug = "food-bird", ParentCategoryId = 1, IconClass = "ph:bird" },
                new ProductCategory { Id = 6, Name = "İt Geyimi", Slug = "cloth-dog", ParentCategoryId = 2, IconClass = "ph:dog" },
                new ProductCategory { Id = 7, Name = "Pişik Geyimi", Slug = "cloth-cat", ParentCategoryId = 2, IconClass = "ph:cat" },
                new ProductCategory { Id = 8, Name = "Aksessuar və Yuvalar", Slug = "cloth-acc", ParentCategoryId = 2, IconClass = "ph:house-line" }
            );

            // 3) Brand
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Royal Canin" },
                new Brand { Id = 2, Name = "Whiskas" },
                new Brand { Id = 3, Name = "Pedigree" },
                new Brand { Id = 4, Name = "Butcher's" },
                new Brand { Id = 5, Name = "Merrick" },
                new Brand { Id = 6, Name = "Pulsitos" },
                new Brand { Id = 7, Name = "Love Corn" },
                new Brand { Id = 8, Name = "Champion Pet" }
            );

            // 4) Product (1-16)
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Boz Qulaqlı İt Kapüşonlusu",
                    Description = "Yumşaq təbii pambıq parçadan hazırlanmış, qulaqlı və isti saxlayan dəbli it kapüşonlusu.",
                    Price = 28.50m,
                    DiscountPrice = 24.00m,
                    StockQuantity = 95,
                    Sku = "CLT-DOG-001",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 1),
                    ProductCategoryId = 6,
                    BrandId = 8
                },
                new Product
                {
                    Id = 2,
                    Name = "Zolaqlı və Cins İt Kombinezonu",
                    Description = "Zolaqlı köynək və dəbli cins kombinezon dəsti, kiçik və orta cins itlər üçün rahat gəzinti geyimi.",
                    Price = 34.00m,
                    DiscountPrice = 29.99m,
                    StockQuantity = 145,
                    Sku = "CLT-DOG-002",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 5),
                    ProductCategoryId = 6,
                    BrandId = 8
                },
                new Product
                {
                    Id = 3,
                    Name = "Çəhrayı-Krem İt Sviteri",
                    Description = "Payız və qış fəsilləri üçün elastik, dərini qıcıqlandırmayan və isti saxlayan toxunma sviter.",
                    Price = 22.00m,
                    DiscountPrice = 19.50m,
                    StockQuantity = 77,
                    Sku = "CLT-DOG-003",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 10),
                    ProductCategoryId = 6,
                    BrandId = 3
                },
                new Product
                {
                    Id = 4,
                    Name = "Qara Zərif İt Kostyumu",
                    Description = "Ağ haşiyəli, zərif və rahat düyməli qara ev və ziyafət geyimi.",
                    Price = 38.00m,
                    DiscountPrice = 32.50m,
                    StockQuantity = 38,
                    Sku = "CLT-DOG-004",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 15),
                    ProductCategoryId = 6,
                    BrandId = 8
                },
                new Product
                {
                    Id = 5,
                    Name = "Təbii Taxta Pişik və İt Yuvası",
                    Description = "Təbii ağacdan hazırlanmış, içərisində yumşaq döşəyi olan rahat və estetik evcik.",
                    Price = 85.00m,
                    DiscountPrice = 72.00m,
                    StockQuantity = 25,
                    Sku = "ACC-PET-005",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 1),
                    ProductCategoryId = 8,
                    BrandId = 1
                },
                new Product
                {
                    Id = 6,
                    Name = "Butcher's Pro Series Mal və Quş Ətli İt Paşteti",
                    Description = "Böyük və orta yaşlı itlər üçün super-premium dərəcəli zərif paştet, taxılsız tərkib.",
                    Price = 4.50m,
                    DiscountPrice = 3.80m,
                    StockQuantity = 120,
                    Sku = "FOD-DOG-006",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 2),
                    ProductCategoryId = 3,
                    BrandId = 4
                },
                new Product
                {
                    Id = 7,
                    Name = "Sarı Champion İt Kapüşonlusu",
                    Description = "İdman tərzində, 100% pambıq parçadan hazırlanmış parlaq sarı kapüşonlu svitşot.",
                    Price = 26.00m,
                    DiscountPrice = null,
                    StockQuantity = 60,
                    Sku = "CLT-DOG-007",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 20),
                    ProductCategoryId = 6,
                    BrandId = 8
                },
                new Product
                {
                    Id = 8,
                    Name = "Banan Naxışlı İt Pijaması",
                    Description = "Ev şəraitində tük tökülməsinin qarşısını alan, yüngül və nəfəs alan əyləncəli pijama.",
                    Price = 19.90m,
                    DiscountPrice = 16.50m,
                    StockQuantity = 70,
                    Sku = "CLT-DOG-008",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 22),
                    ProductCategoryId = 6,
                    BrandId = 8
                },
                new Product
                {
                    Id = 9,
                    Name = "Merrick Fresh Kisses Diş Təmizləyici Çubuqlar",
                    Description = "Nanə tərkibli, nəfəsi təravətləndirən və diş daşlarını təmizləyən ikitərəfli diş fırçası sümükləri.",
                    Price = 17.50m,
                    DiscountPrice = null,
                    StockQuantity = 85,
                    Sku = "FOD-DOG-009",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 3),
                    ProductCategoryId = 3,
                    BrandId = 5
                },
                new Product
                {
                    Id = 10,
                    Name = "Butcher's Pro Diş və Sümük Qoruyucu İt Paşteti",
                    Description = "Kiçik cins itlər üçün kalsium və fosforla zənginləşdirilmiş zərif quş ətli yaş yem.",
                    Price = 4.80m,
                    DiscountPrice = 4.20m,
                    StockQuantity = 110,
                    Sku = "FOD-DOG-010",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 25),
                    ProductCategoryId = 3,
                    BrandId = 4
                },
                new Product
                {
                    Id = 11,
                    Name = "Pulsitos Təbii Qəlyanaltı Dəsti (3 Dad)",
                    Description = "Qovrulmuş noxud və tərəvəzlə zənginləşdirilmiş, yüksək zülallı sağlam qəlyanaltı dəsti.",
                    Price = 12.00m,
                    DiscountPrice = 9.90m,
                    StockQuantity = 90,
                    Sku = "FOD-DOG-011",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 4),
                    ProductCategoryId = 3,
                    BrandId = 6
                },
                new Product
                {
                    Id = 12,
                    Name = "Butcher's Junior Quzu və Toyuq Ətli Bala Yemi",
                    Description = "Bala itlərin inkişafı üçün xüsusi hazırlanmış, asan həzm olunan vitaminli pate.",
                    Price = 4.60m,
                    DiscountPrice = null,
                    StockQuantity = 130,
                    Sku = "FOD-DOG-012",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 18),
                    ProductCategoryId = 3,
                    BrandId = 4
                },
                new Product
                {
                    Id = 13,
                    Name = "All Natural Qaxac Toyuq Əti və Qızılbalıq Yağı",
                    Description = "100% təbii dondurularaq qurudulmuş toyuq sinəsi tikələri və dəri-tük sağlamlığı üçün omega-3 yağı.",
                    Price = 24.50m,
                    DiscountPrice = 21.00m,
                    StockQuantity = 65,
                    Sku = "FOD-CAT-013",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 5),
                    ProductCategoryId = 4,
                    BrandId = 1
                },
                new Product
                {
                    Id = 14,
                    Name = "Love Corn Hissə Verilmiş BBQ Qəlyanaltı",
                    Description = "Ev quşları və gəmiricilər üçün təbii hisə verilmiş dadlı və xırtıldayan qarğıdalı dənələri.",
                    Price = 8.50m,
                    DiscountPrice = null,
                    StockQuantity = 100,
                    Sku = "FOD-BRD-014",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 28),
                    ProductCategoryId = 5,
                    BrandId = 7
                },
                new Product
                {
                    Id = 15,
                    Name = "Butcher's Gastro Mal Ətli Zərif Paştet",
                    Description = "Həssas həzm sisteminə uyğun zərif mal ətli ləziz souslu paştet.",
                    Price = 4.90m,
                    DiscountPrice = 4.10m,
                    StockQuantity = 115,
                    Sku = "FOD-DOG-015",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 3, 6),
                    ProductCategoryId = 3,
                    BrandId = 4
                },
                new Product
                {
                    Id = 16,
                    Name = "Winiary Təlim və Mükafat Kubikləri",
                    Description = "İtlər və pişiklər üçün təlim zamanı istifadə olunan, asan bölünən ətirli mükafat qəlyanaltısı.",
                    Price = 7.20m,
                    DiscountPrice = 5.90m,
                    StockQuantity = 140,
                    Sku = "FOD-PET-016",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 2, 14),
                    ProductCategoryId = 4,
                    BrandId = 2
                }
            );

            // 5) ProductAnimalType (composite key join)
            modelBuilder.Entity<ProductAnimalType>().HasData(
                new ProductAnimalType { ProductId = 1, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 2, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 3, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 4, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 5, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 5, AnimalTypeId = 2 },
                new ProductAnimalType { ProductId = 6, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 7, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 8, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 9, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 10, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 11, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 12, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 13, AnimalTypeId = 2 },
                new ProductAnimalType { ProductId = 14, AnimalTypeId = 3 },
                new ProductAnimalType { ProductId = 15, AnimalTypeId = 1 },
                new ProductAnimalType { ProductId = 16, AnimalTypeId = 2 }
            );

            // 6) ProductVariant
            modelBuilder.Entity<ProductVariant>().HasData(
                new ProductVariant { Id = 1, ProductId = 1, Size = "S", Color = "Boz", PriceModifier = 0m, StockQuantity = 30 },
                new ProductVariant { Id = 2, ProductId = 1, Size = "M", Color = "Boz", PriceModifier = 2.00m, StockQuantity = 40 },
                new ProductVariant { Id = 3, ProductId = 1, Size = "L", Color = "Boz", PriceModifier = 4.00m, StockQuantity = 25 },
                new ProductVariant { Id = 4, ProductId = 2, Size = "M", Color = "Mavi Cins", PriceModifier = 0m, StockQuantity = 80 },
                new ProductVariant { Id = 5, ProductId = 2, Size = "L", Color = "Mavi Cins", PriceModifier = 3.00m, StockQuantity = 65 },
                new ProductVariant { Id = 6, ProductId = 3, Size = "S", Color = "Çəhrayı", PriceModifier = 0m, StockQuantity = 40 },
                new ProductVariant { Id = 7, ProductId = 3, Size = "M", Color = "Çəhrayı", PriceModifier = 2.50m, StockQuantity = 37 },
                new ProductVariant { Id = 8, ProductId = 4, Size = "M", Color = "Qara", PriceModifier = 0m, StockQuantity = 20 },
                new ProductVariant { Id = 9, ProductId = 4, Size = "L", Color = "Qara", PriceModifier = 3.00m, StockQuantity = 18 },
                new ProductVariant { Id = 10, ProductId = 5, Size = "Standart", Color = "Təbii Taxta", PriceModifier = 0m, StockQuantity = 25 },
                new ProductVariant { Id = 11, ProductId = 6, Weight = "150g", PriceModifier = 0m, StockQuantity = 70 },
                new ProductVariant { Id = 12, ProductId = 6, Weight = "300g", PriceModifier = 3.00m, StockQuantity = 50 },
                new ProductVariant { Id = 13, ProductId = 7, Size = "S", Color = "Sarı", PriceModifier = 0m, StockQuantity = 20 },
                new ProductVariant { Id = 14, ProductId = 7, Size = "M", Color = "Sarı", PriceModifier = 2.00m, StockQuantity = 25 },
                new ProductVariant { Id = 15, ProductId = 7, Size = "L", Color = "Sarı", PriceModifier = 4.00m, StockQuantity = 15 },
                new ProductVariant { Id = 16, ProductId = 8, Size = "S", Color = "Mavi Banan", PriceModifier = 0m, StockQuantity = 35 },
                new ProductVariant { Id = 17, ProductId = 8, Size = "M", Color = "Mavi Banan", PriceModifier = 2.00m, StockQuantity = 35 },
                new ProductVariant { Id = 18, ProductId = 9, Weight = "170g", PriceModifier = 0m, StockQuantity = 85 },
                new ProductVariant { Id = 19, ProductId = 10, Weight = "150g", PriceModifier = 0m, StockQuantity = 110 },
                new ProductVariant { Id = 20, ProductId = 11, Weight = "3x50g", PriceModifier = 0m, StockQuantity = 90 },
                new ProductVariant { Id = 21, ProductId = 12, Weight = "150g", PriceModifier = 0m, StockQuantity = 80 },
                new ProductVariant { Id = 22, ProductId = 12, Weight = "300g", PriceModifier = 3.00m, StockQuantity = 50 },
                new ProductVariant { Id = 23, ProductId = 13, Weight = "100g", PriceModifier = 0m, StockQuantity = 65 },
                new ProductVariant { Id = 24, ProductId = 14, Weight = "115g", PriceModifier = 0m, StockQuantity = 100 },
                new ProductVariant { Id = 25, ProductId = 15, Weight = "150g", PriceModifier = 0m, StockQuantity = 65 },
                new ProductVariant { Id = 27, ProductId = 16, Weight = "6 ədəd", PriceModifier = 0m, StockQuantity = 140 },
                new ProductVariant { Id = 28, ProductId = 2, Size = "S", Color = "Mavi Cins", PriceModifier = 0m, StockQuantity = 50 }
            );

            // 7) ProductImage (item1.jpg - item16.jpg)
            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage { Id = 1, ProductId = 1, ImageUrl = "item1.jpg", IsMain = true },
                new ProductImage { Id = 2, ProductId = 2, ImageUrl = "item2.jpg", IsMain = true },
                new ProductImage { Id = 3, ProductId = 3, ImageUrl = "item3.jpg", IsMain = true },
                new ProductImage { Id = 4, ProductId = 4, ImageUrl = "item4.jpg", IsMain = true },
                new ProductImage { Id = 5, ProductId = 5, ImageUrl = "item5.jpg", IsMain = true },
                new ProductImage { Id = 6, ProductId = 6, ImageUrl = "item6.jpg", IsMain = true },
                new ProductImage { Id = 7, ProductId = 7, ImageUrl = "item7.jpg", IsMain = true },
                new ProductImage { Id = 8, ProductId = 8, ImageUrl = "item8.jpg", IsMain = true },
                new ProductImage { Id = 9, ProductId = 9, ImageUrl = "item9.jpg", IsMain = true },
                new ProductImage { Id = 10, ProductId = 10, ImageUrl = "item10.jpg", IsMain = true },
                new ProductImage { Id = 11, ProductId = 11, ImageUrl = "item11.jpg", IsMain = true },
                new ProductImage { Id = 12, ProductId = 12, ImageUrl = "item12.jpg", IsMain = true },
                new ProductImage { Id = 13, ProductId = 13, ImageUrl = "item13.jpg", IsMain = true },
                new ProductImage { Id = 14, ProductId = 14, ImageUrl = "item14.jpg", IsMain = true },
                new ProductImage { Id = 15, ProductId = 15, ImageUrl = "item15.jpg", IsMain = true },
                new ProductImage { Id = 16, ProductId = 16, ImageUrl = "item16.jpg", IsMain = true }
            );

        }
    }
}
