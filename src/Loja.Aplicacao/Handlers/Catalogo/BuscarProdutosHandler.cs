using Loja.Aplicacao.Handlers.Catalogo.Consultas;
using Loja.Aplicacao.Handlers.Catalogo.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loja.Aplicacao.Handlers.Catalogo
{
    public sealed class BuscarProdutosHandler(IProdutoStoreLeitura store) : IRequestHandler<BuscarProdutosQuery, IReadOnlyList<ProdutoModeloLeitura>>
    {
        public Task<IReadOnlyList<ProdutoModeloLeitura>> Handle(BuscarProdutosQuery request, CancellationToken ct)
            => store.BuscarAsync(request.Termo, request.PrecoMinimo, request.PrecoMaximo, ct);
    }

    public record BuscarProdutosQuery(string? Termo, decimal? PrecoMinimo, decimal? PrecoMaximo) : IRequest<IReadOnlyList<ProdutoModeloLeitura>>;
}
