using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetShop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataWithItemImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "LogoUrl", "Name" },
                values: new object[,]
                {
                    { 4, null, "Butcher's" },
                    { 5, null, "Merrick" },
                    { 6, null, "Pulsitos" },
                    { 7, null, "Love Corn" },
                    { 8, null, "Champion Pet" }
                });

            migrationBuilder.InsertData(
                table: "ProductAnimalTypes",
                columns: new[] { "AnimalTypeId", "ProductId" },
                values: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "IconClass", "Name", "ParentCategoryId", "Slug" },
                values: new object[] { 8, "ph:house-line", "Aksessuar və Yuvalar", 2, "cloth-acc" });

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "item1.jpg");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "item2.jpg");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "item3.jpg");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "item4.jpg");

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Color", "Size", "StockQuantity", "Weight" },
                values: new object[] { "Boz", "S", 30, null });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "PriceModifier", "ProductId", "Size", "StockQuantity", "Weight" },
                values: new object[] { "Boz", 2.00m, 1, "M", 40, null });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Color", "PriceModifier", "ProductId", "Size", "StockQuantity" },
                values: new object[] { "Boz", 4.00m, 1, "L", 25 });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Color", "PriceModifier", "ProductId", "Size", "StockQuantity" },
                values: new object[] { "Mavi Cins", 0m, 2, "M", 80 });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Color", "PriceModifier", "ProductId", "Size", "StockQuantity", "Weight" },
                values: new object[,]
                {
                    { 5, "Mavi Cins", 3.00m, 2, "L", 65, null },
                    { 6, "Çəhrayı", 0m, 3, "S", 40, null },
                    { 7, "Çəhrayı", 2.50m, 3, "M", 37, null },
                    { 8, "Qara", 0m, 4, "M", 20, null },
                    { 9, "Qara", 3.00m, 4, "L", 18, null }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BrandId", "CreatedAt", "Description", "DiscountPrice", "Name", "Price", "ProductCategoryId", "Sku", "StockQuantity" },
                values: new object[] { 8, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yumşaq təbii pambıq parçadan hazırlanmış, qulaqlı və isti saxlayan dəbli it kapüşonlusu.", 24.00m, "Boz Qulaqlı İt Kapüşonlusu", 28.50m, 6, "CLT-DOG-001", 95 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BrandId", "CreatedAt", "Description", "DiscountPrice", "Name", "Price", "ProductCategoryId", "Sku", "StockQuantity" },
                values: new object[] { 8, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Zolaqlı köynək və dəbli cins kombinezon dəsti, kiçik və orta cins itlər üçün rahat gəzinti geyimi.", 29.99m, "Zolaqlı və Cins İt Kombinezonu", 34.00m, 6, "CLT-DOG-002", 145 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "DiscountPrice", "Name", "ProductCategoryId", "Sku", "StockQuantity" },
                values: new object[] { new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Payız və qış fəsilləri üçün elastik, dərini qıcıqlandırmayan və isti saxlayan toxunma sviter.", 19.50m, "Çəhrayı-Krem İt Sviteri", 6, "CLT-DOG-003", 77 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BrandId", "CreatedAt", "Description", "DiscountPrice", "Name", "Price", "Sku", "StockQuantity" },
                values: new object[] { 8, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ağ haşiyəli, zərif və rahat düyməli qara ev və ziyafət geyimi.", 32.50m, "Qara Zərif İt Kostyumu", 38.00m, "CLT-DOG-004", 38 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CreatedAt", "Description", "DiscountPrice", "IsActive", "Name", "Price", "ProductCategoryId", "Sku", "StockQuantity" },
                values: new object[,]
                {
                    { 13, 1, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "100% təbii dondurularaq qurudulmuş toyuq sinəsi tikələri və dəri-tük sağlamlığı üçün omega-3 yağı.", 21.00m, true, "All Natural Qaxac Toyuq Əti və Qızılbalıq Yağı", 24.50m, 4, "FOD-CAT-013", 65 },
                    { 16, 2, new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "İtlər və pişiklər üçün təlim zamanı istifadə olunan, asan bölünən ətirli mükafat qəlyanaltısı.", 5.90m, true, "Winiary Təlim və Mükafat Kubikləri", 7.20m, 4, "FOD-PET-016", 140 }
                });

            migrationBuilder.InsertData(
                table: "ProductAnimalTypes",
                columns: new[] { "AnimalTypeId", "ProductId" },
                values: new object[,]
                {
                    { 1, 13 },
                    { 2, 13 },
                    { 1, 16 },
                    { 2, 16 }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "ImageUrl", "IsMain", "ProductId" },
                values: new object[,]
                {
                    { 13, "item13.jpg", true, 13 },
                    { 16, "item16.jpg", true, 16 }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Color", "PriceModifier", "ProductId", "Size", "StockQuantity", "Weight" },
                values: new object[,]
                {
                    { 23, null, 0m, 13, null, 65, "100g" },
                    { 27, null, 0m, 16, null, 140, "6 ədəd" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CreatedAt", "Description", "DiscountPrice", "IsActive", "Name", "Price", "ProductCategoryId", "Sku", "StockQuantity" },
                values: new object[,]
                {
                    { 5, 1, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Təbii ağacdan hazırlanmış, içərisində yumşaq döşəyi olan rahat və estetik evcik.", 72.00m, true, "Təbii Taxta Pişik və İt Yuvası", 85.00m, 8, "ACC-PET-005", 25 },
                    { 6, 4, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Böyük və orta yaşlı itlər üçün super-premium dərəcəli zərif paştet, taxılsız tərkib.", 3.80m, true, "Butcher's Pro Series Mal və Quş Ətli İt Paşteti", 4.50m, 3, "FOD-DOG-006", 120 },
                    { 7, 8, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "İdman tərzində, 100% pambıq parçadan hazırlanmış parlaq sarı kapüşonlu svitşot.", null, true, "Sarı Champion İt Kapüşonlusu", 26.00m, 6, "CLT-DOG-007", 60 },
                    { 8, 8, new DateTime(2026, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ev şəraitində tük tökülməsinin qarşısını alan, yüngül və nəfəs alan əyləncəli pijama.", 16.50m, true, "Banan Naxışlı İt Pijaması", 19.90m, 6, "CLT-DOG-008", 70 },
                    { 9, 5, new DateTime(2026, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nanə tərkibli, nəfəsi təravətləndirən və diş daşlarını təmizləyən ikitərəfli diş fırçası sümükləri.", null, true, "Merrick Fresh Kisses Diş Təmizləyici Çubuqlar", 17.50m, 3, "FOD-DOG-009", 85 },
                    { 10, 4, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kiçik cins itlər üçün kalsium və fosforla zənginləşdirilmiş zərif quş ətli yaş yem.", 4.20m, true, "Butcher's Pro Diş və Sümük Qoruyucu İt Paşteti", 4.80m, 3, "FOD-DOG-010", 110 },
                    { 11, 6, new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Qovrulmuş noxud və tərəvəzlə zənginləşdirilmiş, yüksək zülallı sağlam qəlyanaltı dəsti.", 9.90m, true, "Pulsitos Təbii Qəlyanaltı Dəsti (3 Dad)", 12.00m, 3, "FOD-DOG-011", 90 },
                    { 12, 4, new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bala itlərin inkişafı üçün xüsusi hazırlanmış, asan həzm olunan vitaminli pate.", null, true, "Butcher's Junior Quzu və Toyuq Ətli Bala Yemi", 4.60m, 3, "FOD-DOG-012", 130 },
                    { 14, 7, new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ev quşları və gəmiricilər üçün təbii hisə verilmiş dadlı və xırtıldayan qarğıdalı dənələri.", null, true, "Love Corn Hissə Verilmiş BBQ Qəlyanaltı", 8.50m, 5, "FOD-BRD-014", 100 },
                    { 15, 4, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Həssas həzm sisteminə uyğun zərif mal ətli ləziz souslu paştet.", 4.10m, true, "Butcher's Gastro Mal Ətli Zərif Paştet", 4.90m, 3, "FOD-DOG-015", 115 }
                });

            migrationBuilder.InsertData(
                table: "ProductAnimalTypes",
                columns: new[] { "AnimalTypeId", "ProductId" },
                values: new object[,]
                {
                    { 1, 5 },
                    { 2, 5 },
                    { 1, 6 },
                    { 1, 7 },
                    { 1, 8 },
                    { 1, 9 },
                    { 1, 10 },
                    { 1, 11 },
                    { 2, 11 },
                    { 1, 12 },
                    { 3, 14 },
                    { 1, 15 }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "ImageUrl", "IsMain", "ProductId" },
                values: new object[,]
                {
                    { 5, "item5.jpg", true, 5 },
                    { 6, "item6.jpg", true, 6 },
                    { 7, "item7.jpg", true, 7 },
                    { 8, "item8.jpg", true, 8 },
                    { 9, "item9.jpg", true, 9 },
                    { 10, "item10.jpg", true, 10 },
                    { 11, "item11.jpg", true, 11 },
                    { 12, "item12.jpg", true, 12 },
                    { 14, "item14.jpg", true, 14 },
                    { 15, "item15.jpg", true, 15 }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Color", "PriceModifier", "ProductId", "Size", "StockQuantity", "Weight" },
                values: new object[,]
                {
                    { 10, "Təbii Taxta", 0m, 5, "Standart", 25, null },
                    { 11, null, 0m, 6, null, 70, "150g" },
                    { 12, null, 3.00m, 6, null, 50, "300g" },
                    { 13, "Sarı", 0m, 7, "S", 20, null },
                    { 14, "Sarı", 2.00m, 7, "M", 25, null },
                    { 15, "Sarı", 4.00m, 7, "L", 15, null },
                    { 16, "Mavi Banan", 0m, 8, "S", 35, null },
                    { 17, "Mavi Banan", 2.00m, 8, "M", 35, null },
                    { 18, null, 0m, 9, null, 85, "170g" },
                    { 19, null, 0m, 10, null, 110, "150g" },
                    { 20, null, 0m, 11, null, 90, "3x50g" },
                    { 21, null, 0m, 12, null, 80, "150g" },
                    { 22, null, 3.00m, 12, null, 50, "300g" },
                    { 24, null, 0m, 14, null, 100, "115g" },
                    { 25, null, 0m, 15, null, 65, "150g" },
                    { 26, null, 3.20m, 15, null, 50, "300g" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 7 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 8 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 9 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 10 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 11 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 2, 11 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 12 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 13 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 2, 13 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 3, 14 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 15 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 16 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 2, 16 });

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.InsertData(
                table: "ProductAnimalTypes",
                columns: new[] { "AnimalTypeId", "ProductId" },
                values: new object[] { 2, 2 });

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/products/royal-canin-dog.jpg");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/products/whiskas-cat.jpg");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "/images/products/pedigree-puppy.jpg");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "/images/products/dog-jacket.jpg");

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Color", "Size", "StockQuantity", "Weight" },
                values: new object[] { null, null, 100, "3kg" });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "PriceModifier", "ProductId", "Size", "StockQuantity", "Weight" },
                values: new object[] { null, 0m, 2, null, 150, "1.5kg" });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Color", "PriceModifier", "ProductId", "Size", "StockQuantity" },
                values: new object[] { "Blue", 0m, 4, "M", 40 });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Color", "PriceModifier", "ProductId", "Size", "StockQuantity" },
                values: new object[] { "Blue", 3.00m, 4, "L", 20 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BrandId", "CreatedAt", "Description", "DiscountPrice", "Name", "Price", "ProductCategoryId", "Sku", "StockQuantity" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Böyük yaşlı itlər üçün premium yem", null, "Royal Canin Dog Food 3kg", 45.99m, 3, "RC-DOG-3KG", 100 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BrandId", "CreatedAt", "Description", "DiscountPrice", "Name", "Price", "ProductCategoryId", "Sku", "StockQuantity" },
                values: new object[] { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yetişkin pişiklər üçün balıqlı yem", null, "Whiskas Cat Food 1.5kg", 18.50m, 4, "WHI-CAT-1.5KG", 150 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "DiscountPrice", "Name", "ProductCategoryId", "Sku", "StockQuantity" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kiçik cinslər üçün balaca it yemi", null, "Pedigree Puppy Food 2kg", 3, "PED-PUP-2KG", 80 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BrandId", "CreatedAt", "Description", "DiscountPrice", "Name", "Price", "Sku", "StockQuantity" },
                values: new object[] { null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Orta ölçülü itlər üçün qış ceketi", null, "Dog Winter Jacket M", 15.99m, "CLT-DOG-M", 40 });
        }
    }
}
