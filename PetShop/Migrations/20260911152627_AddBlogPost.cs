using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetShop.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "Author", "Content", "ImageUrl", "IsPublished", "PublishedAt", "Slug", "Summary", "Title" },
                values: new object[,]
                {
                    { 1, "Waggy Team", "At the core of our practice is the idea that cities are the incubators of our greatest achievements, and the best hope for a sustainable future. Animals play a crucial role in maintaining the balance of our ecosystem. By being kind to animals, we not only protect biodiversity but also ensure a healthier planet for future generations.", "blog1.jpg", true, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "10-reasons-helpful-towards-animals", "At the core of our practice is the idea that cities are the incubators of our greatest achievements, and the best hope for a sustainable future.", "10 Reasons to be helpful towards any animals" },
                    { 2, "Waggy Team", "Understanding your pet's hunger cues is essential for their health and wellbeing. Pets communicate their needs in various ways — from pawing at their food bowl to vocalizing. Learn the key signs that indicate your pet needs to be fed and how to establish a healthy feeding routine.", "blog2.jpg", true, new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "how-to-know-your-pet-is-hungry", "At the core of our practice is the idea that cities are the incubators of our greatest achievements, and the best hope for a sustainable future.", "How to know your pet is hungry" },
                    { 3, "Waggy Team", "Creating the perfect home environment for your pets is one of the most important things you can do as a pet owner. From choosing the right bedding to setting up safe play areas, a well-designed home keeps your pets happy, healthy, and stress-free.", "blog3.jpg", true, new DateTime(2026, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "best-home-for-your-pets", "At the core of our practice is the idea that cities are the incubators of our greatest achievements, and the best hope for a sustainable future.", "Best home for your pets" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_Slug",
                table: "BlogPosts",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPosts");
        }
    }
}
