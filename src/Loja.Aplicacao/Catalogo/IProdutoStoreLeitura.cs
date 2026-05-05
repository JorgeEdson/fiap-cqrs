using Loja.Aplicacao.Catalogo.Consultas;

namespace Loja.Aplicacao.Catalogo;

/// <summary>
/// AULA 1 — Lado da LEITURA do Catálogo.
/// Mesmo banco físico (CQRS simples), porém com SQL otimizado para queries.
/// Em sistemas mais maduros, viraria um Read Model dedicado (visto na AULA 4).
/// </summary>
public interface IProdutoStoreLeitura
{
    Task<ProdutoModeloLeitura?> ObterAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<ProdutoModeloLeitura>> ListarAsync(bool incluirDescontinuados, CancellationToken ct);
    Task<IReadOnlyList<ProdutoModeloLeitura>> BuscarAsync(string? termo, decimal? precoMinimo, decimal? precoMaximo, CancellationToken ct);
}
