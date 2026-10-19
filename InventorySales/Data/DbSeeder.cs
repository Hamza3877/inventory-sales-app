using InventorySales.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventorySales.Data;

// Creates the database and fills it with demo data the first time the app runs
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Creates the database and tables if they do not exist yet
        await context.Database.EnsureCreatedAsync();

        await SeedUsersAsync(services);
        await SeedBusinessDataAsync(context);
    }

    private static async Task SeedUsersAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var role in new[] { "Admin", "Staff" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await CreateUserAsync(userManager, "admin@demo.com", "Admin@123", "Admin");
        await CreateUserAsync(userManager, "staff@demo.com", "Staff@123", "Staff");
    }

    private static async Task CreateUserAsync(UserManager<IdentityUser> userManager, string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) != null)
        {
            return;
        }

        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }

    private static async Task SeedBusinessDataAsync(AppDbContext context)
    {
        if (await context.Categories.AnyAsync())
        {
            return;   // data already exists
        }

        var jerseys = new Category { Name = "Jerseys" };
        var shorts = new Category { Name = "Shorts" };
        var tracksuits = new Category { Name = "Tracksuits" };
        var accessories = new Category { Name = "Accessories" };

        var products = new List<Product>
        {
            new Product { Name = "Football Jersey - Home", Price = 2500, StockQuantity = 40, Category = jerseys },
            new Product { Name = "Football Jersey - Away", Price = 2500, StockQuantity = 6, Category = jerseys },
            new Product { Name = "Cricket Jersey", Price = 2200, StockQuantity = 35, Category = jerseys },
            new Product { Name = "Training Shorts", Price = 1200, StockQuantity = 60, Category = shorts },
            new Product { Name = "Match Shorts", Price = 1400, StockQuantity = 8, Category = shorts },
            new Product { Name = "Winter Tracksuit", Price = 5500, StockQuantity = 20, Category = tracksuits },
            new Product { Name = "Summer Tracksuit", Price = 4200, StockQuantity = 25, Category = tracksuits },
            new Product { Name = "Sports Socks (pair)", Price = 350, StockQuantity = 150, Category = accessories },
            new Product { Name = "Team Cap", Price = 700, StockQuantity = 4, Category = accessories },
            new Product { Name = "Gym Bag", Price = 3000, StockQuantity = 18, Category = accessories }
        };

        var customers = new List<Customer>
        {
            new Customer { Name = "Ali Sports Club", Phone = "0300-1111111", Email = "ali@example.com" },
            new Customer { Name = "City Football Academy", Phone = "0301-2222222", Email = "city@example.com" },
            new Customer { Name = "Green Valley School", Phone = "0302-3333333", Email = "school@example.com" },
            new Customer { Name = "Hamid Traders", Phone = "0303-4444444", Email = "hamid@example.com" }
        };

        context.Categories.AddRange(jerseys, shorts, tracksuits, accessories);
        context.Products.AddRange(products);
        context.Customers.AddRange(customers);

        // A few past orders so the dashboard chart is not empty
        AddOrder(context, customers[0], 0, (products[0], 3), (products[3], 5));
        AddOrder(context, customers[1], 1, (products[2], 4), (products[7], 20));
        AddOrder(context, customers[2], 2, (products[5], 2));
        AddOrder(context, customers[3], 3, (products[6], 3), (products[9], 2));
        AddOrder(context, customers[0], 4, (products[1], 2), (products[4], 2));
        AddOrder(context, customers[1], 5, (products[0], 5));
        AddOrder(context, customers[2], 6, (products[3], 10), (products[8], 3));

        await context.SaveChangesAsync();
    }

    private static void AddOrder(AppDbContext context, Customer customer, int daysAgo, params (Product product, int quantity)[] lines)
    {
        var order = new Order
        {
            Customer = customer,
            OrderDate = DateTime.Now.AddDays(-daysAgo)
        };

        foreach (var line in lines)
        {
            line.product.StockQuantity -= line.quantity;

            order.Items.Add(new OrderItem
            {
                Product = line.product,
                Quantity = line.quantity,
                UnitPrice = line.product.Price
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.LineTotal);
        context.Orders.Add(order);
    }
}
