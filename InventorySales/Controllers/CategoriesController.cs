using InventorySales.Models;
using InventorySales.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySales.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly IRepository<Category> _categories;

    public CategoriesController(IRepository<Category> categories)
    {
        _categories = categories;
    }

    public async Task<IActionResult> Index()
    {
        var list = (await _categories.GetAllAsync()).OrderBy(c => c.Name).ToList();
        return View(list);
    }

    public IActionResult Create()
    {
        return View(new Category());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Category category)
    {
        if (await NameExistsAsync(category.Name, 0))
        {
            ModelState.AddModelError("Name", "This category already exists.");
        }

        if (!ModelState.IsValid) return View(category);

        await _categories.AddAsync(category);
        TempData["Success"] = "Category added.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categories.GetByIdAsync(id);
        if (category == null) return NotFound();

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
    {
        if (id != category.Id) return NotFound();

        if (await NameExistsAsync(category.Name, category.Id))
        {
            ModelState.AddModelError("Name", "This category already exists.");
        }

        if (!ModelState.IsValid) return View(category);

        //await _categories.UpdateAsync(category);
        var existingCategory = await _categories.GetByIdAsync(id);
        if (existingCategory == null) return NotFound();

        existingCategory.Name = category.Name;
        await _categories.UpdateAsync(existingCategory);
        TempData["Success"] = "Category updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categories.GetByIdAsync(id);
        if (category == null) return NotFound();

        try
        {
            await _categories.DeleteAsync(category);
            TempData["Success"] = "Category deleted.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "This category has products, so it cannot be deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    // True if another category already uses this name
    private async Task<bool> NameExistsAsync(string name, int currentId)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        var all = await _categories.GetAllAsync();
        return all.Any(c => c.Id != currentId && c.Name.ToLower() == name.Trim().ToLower());
    }
}
