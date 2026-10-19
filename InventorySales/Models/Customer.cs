using System.ComponentModel.DataAnnotations;

namespace InventorySales.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    [StringLength(20)]
    public string Phone { get; set; }

    [EmailAddress, StringLength(100)]
    public string Email { get; set; }
}
