using InventorySales.Data;
using InventorySales.Models;
using InventorySales.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventorySales.Services;

public class OrderResult
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public int OrderId { get; set; }
}

public interface IOrderService
{
    Task<OrderResult> CreateOrderAsync(int customerId, List<OrderLineViewModel> lines);
}

// The business rules for creating an order live here (not in the controller)
public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderResult> CreateOrderAsync(int customerId, List<OrderLineViewModel> lines)
    {
        // Step 1: keep only the filled rows, and merge the same product if it appears twice
        var wanted = (lines ?? new List<OrderLineViewModel>())
            .Where(l => l.ProductId.HasValue && l.Quantity > 0)
            .GroupBy(l => l.ProductId.Value)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(l => l.Quantity.Value) })
            .ToList();

        if (wanted.Count == 0)
        {
            return Fail("Please add at least one product.");
        }

        // Step 2: check the customer
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
        {
            return Fail("Please select a customer.");
        }

        // Step 3: load the products and check there is enough stock for every row
        var productIds = wanted.Select(w => w.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        foreach (var row in wanted)
        {
            var product = products.FirstOrDefault(p => p.Id == row.ProductId);
            if (product == null)
            {
                return Fail("One of the selected products was not found.");
            }
            if (product.StockQuantity < row.Quantity)
            {
                return Fail($"Not enough stock for {product.Name}. Available: {product.StockQuantity}.");
            }
        }

        // Step 4: everything is fine, so build the order and reduce the stock
        var order = new Order
        {
            CustomerId = customerId,
            OrderDate = DateTime.Now
        };

        foreach (var row in wanted)
        {
            var product = products.First(p => p.Id == row.ProductId);

            product.StockQuantity -= row.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = row.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.LineTotal);

        // One SaveChanges saves the order AND the new stock together.
        // If something fails, nothing is saved.
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return new OrderResult { Success = true, OrderId = order.Id };
    }

    private static OrderResult Fail(string message)
    {
        return new OrderResult { Success = false, Error = message };
    }
}
