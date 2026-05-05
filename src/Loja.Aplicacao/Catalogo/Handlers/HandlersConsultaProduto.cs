using Loja.Aplicacao.Catalogo.Consultas;
using MediatR;

namespace Loja.Aplicacao.Catalogo.Handlers;

/// <summary>AULA 1 — Handlers das Consultas: leitura otimizada, sem mexer em domínio.</summary>
public sealed class ObterProdutoPorIdHandler(IProdutoStoreLeitura store)
    : IRequestHandler<ObterProdutoPorIdQuery, ProdutoModeloLeitura?>
{
    public Task<ProdutoModeloLeitura?> Handle(ObterProdutoPorIdQuery request, CancellationToken ct)
        => store.ObterAsync(request.Id, ct);
}

public sealed class ListarProdutosHandler(IProdutoStoreLeitura store)
    : IRequestHandler<ListarProdutosQuery, IReadOnlyList<ProdutoModeloLeitura>>
{
    public Task<IReadOnlyList<ProdutoModeloLeitura>> Handle(ListarProdutosQuery request, CancellationToken ct)
        => store.ListarAsync(request.IncluirDescontinuados, ct);
}

public sealed class BuscarProdutosHandler(IProdutoStoreLeitura store)
    : IRequestHandler<BuscarProdutosQuery, IReadOnlyList<ProdutoModeloLeitura>>
{
    public Task<IReadOnlyList<ProdutoModeloLeitura>> Handle(BuscarProdutosQuery request, CancellationToken ct)
        => store.BuscarAsync(request.Termo, request.PrecoMinimo, request.PrecoMaximo, ct);
}
