using Loja.Dominio.Catalogo;

namespace Loja.Aplicacao.Catalogo;

/// <summary>
/// AULA 1 — Lado da ESCRITA do Catálogo (CQRS simples, banco único).
/// Implementado em Loja.Infraestrutura com EF Core.
/// </summary>
public interface IProdutoStoreEscrita
{
    Task<Produto?> ObterAsync(Guid id, CancellationToken ct);
    Task AdicionarAsync(Produto produto, CancellationToken ct);
    Task<bool> SalvarAlteracoesAsync(CancellationToken ct);
    Task<bool> RemoverAsync(Guid id, CancellationToken ct);
}
