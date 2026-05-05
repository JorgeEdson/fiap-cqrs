using Loja.Aplicacao.Catalogo;
using Loja.Dominio.Catalogo;
using Loja.Infraestrutura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infraestrutura.Catalogo;

public sealed class EfProdutoStoreEscrita(AppDbContext db) : IProdutoStoreEscrita
{
    public Task<Produto?> ObterAsync(Guid id, CancellationToken ct)
        => db.Produtos.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AdicionarAsync(Produto produto, CancellationToken ct)
        => await db.Produtos.AddAsync(produto, ct);

    public async Task<bool> SalvarAlteracoesAsync(CancellationToken ct)
        => await db.SaveChangesAsync(ct) > 0;

    public async Task<bool> RemoverAsync(Guid id, CancellationToken ct)
    {
        var existente = await db.Produtos.FindAsync([id], ct);
        if (existente is null) return false;
        db.Produtos.Remove(existente);
        return await db.SaveChangesAsync(ct) > 0;
    }
}
