using ClosedXML.Excel;
using InventorySales.Models;
using InventorySales.Repositories;
using InventorySales.Services;
using InventorySales.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySales.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private const int NumberOfRows = 5;   // product rows on the Create Order screen

    private readonly IOrderRepository _orders;
    private readonly IOrderService _orderService;
    private readonly IRepository<Customer> _customers;
    private readonly IProductRepository _products;

    public OrdersController(
        IOrderRepository orders,
        IOrderService orderService,
        IRepository<Customer> customers,
        IProductRepository products)
    {
        _orders = orders;
        _orderService = orderService;
        _customers = customers;
        _products = products;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _orders.GetAllWithCustomerAsync();
        return View(list);
    }

    // Invoice page
    public async Task<IActionResult> Details(int id)
    {
        var order = await _orders.GetWithDetailsAsync(id);
        if (order == null) return NotFound();

        return View(order);
    }

    public async Task<IActionResult> Create()
    {
        var model = new OrderCreateViewModel();
        for (int i = 0; i < NumberOfRows; i++)
        {
            model.Lines.Add(new OrderLineViewModel());
        }

        await FillDropdownsAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderCreateViewModel model)
    {
        var result = await _orderService.CreateOrderAsync(model.CustomerId, model.Lines);

        if (result.Success)
        {
            TempData["Success"] = "Order created and stock updated.";
            return RedirectToAction(nameof(Details), new { id = result.OrderId });
        }

        // Something is wrong: show the message and the same screen again
        ModelState.AddModelError("", result.Error);
        await FillDropdownsAsync();
        return View(model);
    }

    // Download all orders as an Excel file
    public async Task<IActionResult> ExportExcel()
    {
        var orders = await _orders.GetAllWithCustomerAsync();

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Orders");

        sheet.Cell(1, 1).Value = "Order No";
        sheet.Cell(1, 2).Value = "Date";
        sheet.Cell(1, 3).Value = "Customer";
        sheet.Cell(1, 4).Value = "Total (Rs)";
        sheet.Row(1).Style.Font.Bold = true;

        int row = 2;
        foreach (var order in orders)
        {
            sheet.Cell(row, 1).Value = order.Id;
            sheet.Cell(row, 2).Value = order.OrderDate.ToString("dd MMM yyyy HH:mm");
            sheet.Cell(row, 3).Value = order.Customer.Name;
            sheet.Cell(row, 4).Value = order.TotalAmount;
            row++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "orders.xlsx");
    }

    // Fills the customer and product dropdowns
    private async Task FillDropdownsAsync()
    {
        var customers = (await _customers.GetAllAsync()).OrderBy(c => c.Name).ToList();
        ViewBag.Customers = new SelectList(customers, "Id", "Name");

        var products = (await _products.GetAllAsync()).OrderBy(p => p.Name).ToList();
        ViewBag.Products = products
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} (Stock: {p.StockQuantity}) - Rs {p.Price:N0}"
            })
            .ToList();
    }
}
