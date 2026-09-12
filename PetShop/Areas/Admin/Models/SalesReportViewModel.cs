using System;
using System.Collections.Generic;
using PetShop.DataContext.Entites;

namespace PetShop.Areas.Admin.Models
{
    public class SalesReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        // Inventory Metrics
        public int TotalProducts { get; set; }
        public int OutOfStockCount { get; set; }
        public int LowStockCount { get; set; }
        public decimal TotalInventoryValue { get; set; }

        public List<ProductAlertItem> OutOfStockProducts { get; set; } = new();
        public List<ProductAlertItem> LowStockProducts { get; set; } = new();
        public List<CategoryStockSummary> CategorySummaries { get; set; } = new();

        // Sales Metrics
        public int TotalOrders { get; set; }
        public int PaidOrdersCount { get; set; }
        public int PendingOrdersCount { get; set; }
        public int CancelledOrdersCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }

        public List<TopSellingItem> TopSellingProducts { get; set; } = new();
        public List<Order> OrdersInRange { get; set; } = new();
    }

    public class ProductAlertItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class CategoryStockSummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int TotalStock { get; set; }
        public decimal TotalStockValue { get; set; }
    }

    public class TopSellingItem
    {
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
