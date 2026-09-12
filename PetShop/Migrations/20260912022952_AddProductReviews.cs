using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetShop.Migrations
{
    /// <inheritdoc />
    public partial class AddProductReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 2, 11 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 13 });

            migrationBuilder.DeleteData(
                table: "ProductAnimalTypes",
                keyColumns: new[] { "AnimalTypeId", "ProductId" },
                keyValues: new object[] { 1, 16 });

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsVerifiedPurchase = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReviews_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId",
                table: "ProductReviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId_UserId",
                table: "ProductReviews",
                columns: new[] { "ProductId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.InsertData(
                table: "ProductAnimalTypes",
                columns: new[] { "AnimalTypeId", "ProductId" },
                values: new object[,]
                {
                    { 2, 11 },
                    { 1, 13 },
                    { 1, 16 }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Color", "PriceModifier", "ProductId", "Size", "StockQuantity", "Weight" },
                values: new object[] { 26, null, 3.20m, 15, null, 50, "300g" });
        }
    }
}
