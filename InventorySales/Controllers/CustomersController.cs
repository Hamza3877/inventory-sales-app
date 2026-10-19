using InventorySales.Models;
using InventorySales.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySales.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly IRepository<Customer> _customers;

    public CustomersController(IRepository<Customer> customers)
    {
        _customers = customers;
    }

    public async Task<IActionResult> Index(string search)
    {
        var list = await _customers.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var text = search.Trim().ToLower();
            list = list.Where(c => c.Name.ToLower().Contains(text)
                                || (c.Phone ?? "").ToLower().Contains(text)).ToList();
        }

        ViewBag.Search = search;
        return View(list.OrderBy(c => c.Name).ToList());
    }

    public IActionResult Create()
    {
        return View(new Customer());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Phone,Email")] Customer customer)
    {
        if (!ModelState.IsValid) return View(customer);

        await _customers.AddAsync(customer);
        TempData["Success"] = "Customer added.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customers.GetByIdAsync(id);
        if (customer == null) return NotFound();

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Email")] Customer customer)
    {
        if (id != customer.Id) return NotFound();
        if (!ModelState.IsValid) return View(customer);

        await _customers.UpdateAsync(customer);
        TempData["Success"] = "Customer updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _customers.GetByIdAsync(id);
        if (customer == null) return NotFound();

        try
        {
            await _customers.DeleteAsync(customer);
            TempData["Success"] = "Customer deleted.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "This customer has orders, so it cannot be deleted.";
        }

        return RedirectToAction(nameof(Index));
    }
}
