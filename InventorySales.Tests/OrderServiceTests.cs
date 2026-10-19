using InventorySales.Data;
using InventorySales.Models;
using InventorySales.Services;
using InventorySales.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventorySales.Tests;

public class OrderServiceTests
{
    // Makes a fresh in-memory database with 1 customer and 1 product (stock 10, price 100)
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var category = new Category { Name = "Test Category" };
        context.Customers.Add(new Customer { Id = 1, Name = "Test Customer" });
        context.Products.Add(new Product { Id = 1, Name = "Test Product", Price = 100, StockQuantity = 10, Category = category });
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task CreateOrder_ReducesStock_AndCalculatesTotal()
    {
        using var context = CreateContext();
        var service = new OrderService(context);
        var lines = new List<OrderLineViewModel>
        {
            new OrderLineViewModel { ProductId = 1, Quantity = 3 }
        };

        var result = await service.CreateOrderAsync(1, lines);

        Assert.True(result.Success);
        Assert.Equal(7, context.Products.Find(1).StockQuantity);   // 10 - 3
        Assert.Equal(300, context.Orders.Find(result.OrderId).TotalAmount);   // 3 x 100
    }

    [Fact]
    public async Task CreateOrder_NotEnoughStock_FailsAndKeepsStock()
    {
        using var context = CreateContext();
        var service = new OrderService(context);
        var lines = new List<OrderLineViewModel>
        {
            new OrderLineViewModel { ProductId = 1, Quantity = 11 }
        };

        var result = await service.CreateOrderAsync(1, lines);

        Assert.False(result.Success);
        Assert.Equal(10, context.Products.Find(1).StockQuantity);   // unchanged
        Assert.Empty(context.Orders);
    }

    [Fact]
    public async Task CreateOrder_WithNoProducts_Fails()
    {
        using var context = CreateContext();
        var service = new OrderService(context);
        var lines = new List<OrderLineViewModel>
        {
            new OrderLineViewModel()   // empty row
        };

        var result = await service.CreateOrderAsync(1, lines);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task CreateOrder_SameProductTwice_IsMergedIntoOneRow()
    {
        using var context = CreateContext();
        var service = new OrderService(context);
        var lines = new List<OrderLineViewModel>
        {
            new OrderLineViewModel { ProductId = 1, Quantity = 2 },
            new OrderLineViewModel { ProductId = 1, Quantity = 3 }
        };

        var result = await service.CreateOrderAsync(1, lines);

        Assert.True(result.Success);
        Assert.Equal(5, context.Orders.Include(o => o.Items).First().Items.Single().Quantity);
        Assert.Equal(5, context.Products.Find(1).StockQuantity);   // 10 - 5
    }
}
