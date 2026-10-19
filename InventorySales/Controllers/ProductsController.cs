using InventorySales.Models;
using InventorySales.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySales.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private const int PageSize = 8;

    private readonly IProductRepository _products;
    private readonly IRepository<Category> _categories;

    public ProductsController(IProductRepository products, IRepository<Category> categories)
    {
        _products = products;
        _categories = categories;
    }

    // List with search and pages
    public async Task<IActionResult> Index(string search, int page = 1)
    {
        if (page < 1) page = 1;

        var result = await _products.SearchAsync(search, page, PageSize);

        ViewBag.Search = search;
        return View(result);
    }

    public async Task<IActionResult> Create()
    {
        await FillCategoriesAsync(null);
        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Price,StockQuantity,CategoryId")] Product product)
    {
        if (!ModelState.IsValid)
        {
            await FillCategoriesAsync(product.CategoryId);
            return View(product);
        }

        await _products.AddAsync(product);
        TempData["Success"] = "Product added.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null) return NotFound();

        await FillCategoriesAsync(product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,StockQuantity,CategoryId")] Product product)
    {
        if (id != product.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await FillCategoriesAsync(product.CategoryId);
            return View(product);
        }

        await _products.UpdateAsync(product);
        TempData["Success"] = "Product updated.";
        return RedirectToAction(nameof(Index));
    }

    // Only Admin can delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null) return NotFound();

        try
        {
            await _products.DeleteAsync(product);
            TempData["Success"] = "Product deleted.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "This product is used in orders, so it cannot be deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    // Fills the category dropdown
    private async Task FillCategoriesAsync(int? selectedId)
    {
        var categories = (await _categories.GetAllAsync()).OrderBy(c => c.Name).ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
    }
}
