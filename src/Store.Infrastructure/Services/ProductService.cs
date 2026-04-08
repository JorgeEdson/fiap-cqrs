using Microsoft.EntityFrameworkCore;
using Store.Domain;
using Store.Domain.Services;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Services;

public class ProductService(AppDbContext dbContext) : IProductService
{
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await dbContext.Products.AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Name required", nameof(product.Name));

        if (product.Price <= 0)
            throw new ArgumentOutOfRangeException("Price must be greater then zero", nameof(product.Price));

        if (product.Stock < 0)
            throw new ArgumentOutOfRangeException("Initial Stock must be non-negative", nameof(product.Stock));

        product.Discontinued = false;

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = await dbContext.Products.FindAsync(product.Id);
        if (existing is null)
            return false;

        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Name required", nameof(product.Name));

        if (product.Price <= 0)
            throw new ArgumentOutOfRangeException("Price must be greater then zero", nameof(product.Price));

        if (product.Stock < 0)
            throw new ArgumentOutOfRangeException("Initial Stock must be non-negative", nameof(product.Stock));


        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.Discontinued = product.Discontinued;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product is null)
        {
            return false;
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
