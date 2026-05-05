using Loja.Dominio.Catalogo;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infraestrutura.Persistencia;

/// <summary>
/// AULA 1 — DbContext do Catálogo (CRUD/CQRS simples).
/// Mantemos o nome AppDbContext porque é convenção do EF Core.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public DbSet<Produto> Produtos => Set<Produto>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Produto>(e =>
        {
            e.ToTable("produtos");
            e.HasKey(p => p.Id);
            e.Property(p => p.Nome).IsRequired().HasMaxLength(200);
            e.Property(p => p.Preco).HasColumnType("numeric(18,2)");
            e.Property(p => p.Estoque);
            e.Property(p => p.Descontinuado);
        });
    }
}
