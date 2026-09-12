using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Areas.Admin.Models;
using PetShop.DataContext;

namespace PetShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [Route("Admin")]
        [Route("Admin/Dashboard")]
        [Route("Admin/Dashboard/Index")]
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.ProductCategories.CountAsync(),
                TotalBrands = await _context.Brands.CountAsync(),
                TotalBlogPosts = await _context.BlogPosts.CountAsync(),
                
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.Where(o => o.PaymentStatus == "Paid").SumAsync(o => o.TotalAmount),
                
                RecentOrders = await _context.Orders
                                        .OrderByDescending(o => o.CreatedAt)
                                        .Take(5)
                                        .ToListAsync(),

                RecentUsers = await _context.AdminUsers
                                        .OrderByDescending(u => u.CreatedAt)
                                        .Take(5)
                                        .ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        [Route("Admin/Dashboard/ExportProducts")]
        public async Task<IActionResult> ExportProducts(string? search = null, int? categoryId = null)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(s) || (p.Sku != null && p.Sku.ToLower().Contains(s)));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.ProductCategoryId == categoryId.Value);
            }

            var products = await query.OrderBy(p => p.Id).ToListAsync();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Name,Category,Brand,Price,DiscountPrice,StockQuantity,IsActive,CreatedAt");

            foreach (var p in products)
            {
                var name = EscapeCsv(p.Name);
                var category = EscapeCsv(p.ProductCategory?.Name ?? "Uncategorized");
                var brand = EscapeCsv(p.Brand?.Name ?? "-");
                var price = p.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                var discountPrice = p.DiscountPrice.HasValue ? p.DiscountPrice.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "";
                var stock = p.StockQuantity.ToString();
                var isActive = p.IsActive ? "Yes" : "No";
                var createdAt = p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");

                sb.AppendLine($"{name},{category},{brand},{price},{discountPrice},{stock},{isActive},{createdAt}");
            }

            var preamble = System.Text.Encoding.UTF8.GetPreamble();
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            var fullBytes = new byte[preamble.Length + contentBytes.Length];
            Buffer.BlockCopy(preamble, 0, fullBytes, 0, preamble.Length);
            Buffer.BlockCopy(contentBytes, 0, fullBytes, preamble.Length, contentBytes.Length);

            var fileName = $"products-export-{DateTime.UtcNow:yyyyMMdd}.csv";
            return File(fullBytes, "text/csv; charset=utf-8", fileName);
        }

        [HttpGet]
        [Route("Admin/Dashboard/Report")]
        [Route("Admin/Dashboard/CreateReport")]
        public async Task<IActionResult> Report(DateTime? from, DateTime? to)
        {
            var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
            var toDate = to ?? DateTime.UtcNow.Date;

            var rangeStart = fromDate.Date;
            var rangeEnd = toDate.Date.AddDays(1).AddTicks(-1);

            var allProducts = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Include(p => p.Brand)
                .ToListAsync();

            var totalProducts = allProducts.Count;
            var outOfStockList = allProducts.Where(p => p.StockQuantity == 0).ToList();
            var lowStockList = allProducts.Where(p => p.StockQuantity > 0 && p.StockQuantity < 5).ToList();
            var totalInventoryValue = allProducts.Sum(p => (p.DiscountPrice ?? p.Price) * p.StockQuantity);

            var categorySummaries = allProducts
                .GroupBy(p => p.ProductCategory?.Name ?? "Uncategorized")
                .Select(g => new CategoryStockSummary
                {
                    CategoryName = g.Key,
                    ProductCount = g.Count(),
                    TotalStock = g.Sum(p => p.StockQuantity),
                    TotalStockValue = g.Sum(p => (p.DiscountPrice ?? p.Price) * p.StockQuantity)
                })
                .OrderByDescending(c => c.ProductCount)
                .ToList();

            var ordersInRange = await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Where(o => o.CreatedAt >= rangeStart && o.CreatedAt <= rangeEnd)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var totalOrders = ordersInRange.Count;
            var paidOrders = ordersInRange.Where(o => o.PaymentStatus == "Paid").ToList();
            var totalRevenue = paidOrders.Sum(o => o.TotalAmount);
            var avgOrderValue = paidOrders.Count > 0 ? (totalRevenue / paidOrders.Count) : (totalOrders > 0 ? ordersInRange.Sum(o => o.TotalAmount) / totalOrders : 0);

            var topSelling = ordersInRange
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.ProductName)
                .Select(g => new TopSellingItem
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToList();

            var viewModel = new SalesReportViewModel
            {
                FromDate = rangeStart,
                ToDate = toDate.Date,
                TotalProducts = totalProducts,
                OutOfStockCount = outOfStockList.Count,
                LowStockCount = lowStockList.Count,
                TotalInventoryValue = totalInventoryValue,
                OutOfStockProducts = outOfStockList.Select(p => new ProductAlertItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.ProductCategory?.Name ?? "Uncategorized",
                    BrandName = p.Brand?.Name ?? "-",
                    Price = p.DiscountPrice ?? p.Price,
                    StockQuantity = p.StockQuantity
                }).ToList(),
                LowStockProducts = lowStockList.Select(p => new ProductAlertItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.ProductCategory?.Name ?? "Uncategorized",
                    BrandName = p.Brand?.Name ?? "-",
                    Price = p.DiscountPrice ?? p.Price,
                    StockQuantity = p.StockQuantity
                }).ToList(),
                CategorySummaries = categorySummaries,
                TotalOrders = totalOrders,
                PaidOrdersCount = paidOrders.Count,
                PendingOrdersCount = ordersInRange.Count(o => o.OrderStatus == "Pending"),
                CancelledOrdersCount = ordersInRange.Count(o => o.OrderStatus == "Cancelled"),
                TotalRevenue = totalRevenue,
                AverageOrderValue = avgOrderValue,
                TopSellingProducts = topSelling,
                OrdersInRange = ordersInRange
            };

            return View(viewModel);
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}

