namespace InventorySales.ViewModels;

// One row on the "Create Order" screen: which product and how many
public class OrderLineViewModel
{
    public int? ProductId { get; set; }
    public int? Quantity { get; set; }
}

// The whole "Create Order" screen: a customer and a few product rows
public class OrderCreateViewModel
{
    public int CustomerId { get; set; }
    public List<OrderLineViewModel> Lines { get; set; } = new();
}
