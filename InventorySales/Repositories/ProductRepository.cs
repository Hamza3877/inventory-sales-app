using InventorySales.Data;
using InventorySales.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySales.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<PagedResult<Product>> SearchAsync(string search, int page, int pageSize);
}

// Products need extra actions (search + pages), so they get their own repository
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Product>> SearchAsync(string search, int page, int pageSize)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Category.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
