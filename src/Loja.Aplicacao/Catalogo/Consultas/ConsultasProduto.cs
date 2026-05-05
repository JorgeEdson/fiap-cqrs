using MediatR;

namespace Loja.Aplicacao.Catalogo.Consultas;

/// <summary>
/// Consultas (Queries) — perguntas ao sistema, sem efeito colateral. Discutido na AULA 1.
/// CQRS simples: Consultas lêem direto do banco, retornando DTOs prontos para a UI
/// (sem precisar carregar entidades de domínio "obesas").
/// </summary>
public record ObterProdutoPorIdQuery(Guid Id) : IRequest<ProdutoModeloLeitura?>;

public record ListarProdutosQuery(bool IncluirDescontinuados = false) : IRequest<IReadOnlyList<ProdutoModeloLeitura>>;

public record BuscarProdutosQuery(string? Termo, decimal? PrecoMinimo, decimal? PrecoMaximo) : IRequest<IReadOnlyList<ProdutoModeloLeitura>>;

public record ProdutoModeloLeitura(
    Guid Id,
    string Nome,
    decimal Preco,
    int Estoque,
    bool Descontinuado);
