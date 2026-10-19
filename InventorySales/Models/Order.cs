namespace InventorySales.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public decimal TotalAmount { get; set; }

    // One order has many items (one row per product)
    public List<OrderItem> Items { get; set; } = new();
}
