using InventorySales.Data;
using InventorySales.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventorySales.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync();
}

public class DashboardService : IDashboardService
{
    private const int LowStockLevel = 10;

    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var today = DateTime.Today;
        var firstDay = today.AddDays(-6);   // today + the 6 days before = 7 days

        var recentOrders = await _context.Orders
            .Where(o => o.OrderDate >= firstDay)
            .ToListAsync();

        var model = new DashboardViewModel
        {
            TodaySales = recentOrders.Where(o => o.OrderDate.Date == today).Sum(o => o.TotalAmount),
            TotalOrders = await _context.Orders.CountAsync(),
            TotalProducts = await _context.Products.CountAsync(),
            TotalCustomers = await _context.Customers.CountAsync(),
            LowStockProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity < LowStockLevel)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync()
        };

        // One bar per day for the chart
        for (int i = 0; i < 7; i++)
        {
            var day = firstDay.AddDays(i);
            var total = recentOrders.Where(o => o.OrderDate.Date == day).Sum(o => o.TotalAmount);

            model.SalesLabels.Add(day.ToString("dd MMM"));
            model.SalesValues.Add(total);
        }

        return model;
    }
}
