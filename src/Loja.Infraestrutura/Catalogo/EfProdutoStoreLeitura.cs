using Loja.Aplicacao.Catalogo;
using Loja.Aplicacao.Catalogo.Consultas;
using Loja.Infraestrutura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infraestrutura.Catalogo;

public sealed class EfProdutoStoreLeitura(AppDbContext db) : IProdutoStoreLeitura
{
    public Task<ProdutoModeloLeitura?> ObterAsync(Guid id, CancellationToken ct)
        => db.Produtos
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProdutoModeloLeitura(p.Id, p.Nome, p.Preco, p.Estoque, p.Descontinuado))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ProdutoModeloLeitura>> ListarAsync(bool incluirDescontinuados, CancellationToken ct)
        => await db.Produtos
            .AsNoTracking()
            .Where(p => incluirDescontinuados || !p.Descontinuado)
            .OrderBy(p => p.Nome)
            .Select(p => new ProdutoModeloLeitura(p.Id, p.Nome, p.Preco, p.Estoque, p.Descontinuado))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ProdutoModeloLeitura>> BuscarAsync(
        string? termo, decimal? precoMinimo, decimal? precoMaximo, CancellationToken ct)
    {
        var q = db.Produtos.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(termo))
            q = q.Where(p => EF.Functions.ILike(p.Nome, $"%{termo}%"));
        if (precoMinimo.HasValue) q = q.Where(p => p.Preco >= precoMinimo.Value);
        if (precoMaximo.HasValue) q = q.Where(p => p.Preco <= precoMaximo.Value);
        return await q
            .OrderBy(p => p.Nome)
            .Select(p => new ProdutoModeloLeitura(p.Id, p.Nome, p.Preco, p.Estoque, p.Descontinuado))
            .ToListAsync(ct);
    }
}
