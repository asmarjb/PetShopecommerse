using PetShop.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
var configuration = builder.Build();

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));

using (var db = new AppDbContext(optionsBuilder.Options))
{
    foreach (var user in db.AdminUsers)
    {
        Console.WriteLine($"ID: {user.Id}, Email: {user.Email}, Name: {user.FullName}, Image: {user.ProfileImageUrl}");
    }
}
