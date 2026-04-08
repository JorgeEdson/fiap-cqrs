using Microsoft.EntityFrameworkCore;
using Store.Domain;

namespace Store.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>(e =>
        {
            e.ToTable("products");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired();
            e.Property(p => p.Price).HasColumnType("numeric(18,2)");
            e.Property(p => p.Stock);
            e.Property(p => p.Discontinued);
        });
    }
}
