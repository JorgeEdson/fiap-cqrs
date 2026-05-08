using Loja.Dominio.Catalogo;

namespace Loja.Aplicacao.Handlers.Catalogo.Interfaces;

public interface IProdutoStoreEscrita
{
    Task<Produto?> ObterAsync(Guid id, CancellationToken ct);
    Task AdicionarAsync(Produto produto, CancellationToken ct);
    Task<bool> SalvarAlteracoesAsync(CancellationToken ct);
    Task<bool> RemoverAsync(Guid id, CancellationToken ct);
}
