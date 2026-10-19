using InventorySales.Data;
using InventorySales.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySales.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetAllWithCustomerAsync();
    Task<Order> GetWithDetailsAsync(int id);
}

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    // All orders with the customer name, newest first
    public async Task<List<Order>> GetAllWithCustomerAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    // One order with its customer and all its items (used for the invoice)
    public async Task<Order> GetWithDetailsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
