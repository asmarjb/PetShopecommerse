using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PetShop.DataContext;
using PetShop.Services;

namespace PetShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddScoped<IWishlistService, WishlistService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Dual Cookie Authentication schemes:
            // 1) "AdminAuth" (PetShop_Admin_Auth) for Admin area
            // 2) "UserAuth" (PetShop_User_Auth) for Storefront customers
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "UserAuth";
                options.DefaultAuthenticateScheme = "UserAuth";
                options.DefaultChallengeScheme = "UserAuth";
            })
            .AddCookie("AdminAuth", options =>
            {
                options.LoginPath = "/Admin/Account/Login";
                options.LogoutPath = "/Admin/Account/Logout";
                options.AccessDeniedPath = "/Admin/Account/AccessDenied";
                options.Cookie.Name = "PetShop_Admin_Auth";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            })
            .AddCookie("UserAuth", options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "PetShop_User_Auth";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.SlidingExpiration = true;
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.AuthenticationSchemes.Add("AdminAuth");
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("UserOnly", policy =>
                {
                    policy.AuthenticationSchemes.Add("UserAuth");
                    policy.RequireRole("User");
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();   // ← must come before UseAuthorization
            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                // Force update all users named "Admin Hasan" to "Aytac"
                var users = dbContext.AdminUsers.ToList();
                foreach (var u in users)
                {
                    if (u.FullName == "Admin Hasan" || string.IsNullOrEmpty(u.FullName))
                    {
                        u.FullName = "Aytac";
                    }
                }
                dbContext.SaveChanges();
            }

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
