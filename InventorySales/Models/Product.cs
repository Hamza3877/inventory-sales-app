using System.ComponentModel.DataAnnotations;

namespace InventorySales.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    [Range(0.01, 10000000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Range(0, 100000, ErrorMessage = "Stock cannot be negative")]
    [Display(Name = "Stock")]
    public int StockQuantity { get; set; }

    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public Category Category { get; set; }
}
