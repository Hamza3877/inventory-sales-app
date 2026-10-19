using InventorySales.Models;

namespace InventorySales.ViewModels;

public class DashboardViewModel
{
    public decimal TodaySales { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }

    // Products with stock below the warning level
    public List<Product> LowStockProducts { get; set; } = new();

    // Data for the "last 7 days" chart
    public List<string> SalesLabels { get; set; } = new();
    public List<decimal> SalesValues { get; set; } = new();
}
