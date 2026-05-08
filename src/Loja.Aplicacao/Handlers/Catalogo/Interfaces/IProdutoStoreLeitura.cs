using Loja.Aplicacao.Handlers.Catalogo.Consultas;

namespace Loja.Aplicacao.Handlers.Catalogo.Interfaces;

public interface IProdutoStoreLeitura
{
    Task<ProdutoModeloLeitura?> ObterAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<ProdutoModeloLeitura>> ListarAsync(bool incluirDescontinuados, CancellationToken ct);
    Task<IReadOnlyList<ProdutoModeloLeitura>> BuscarAsync(string? termo, decimal? precoMinimo, decimal? precoMaximo, CancellationToken ct);
}
