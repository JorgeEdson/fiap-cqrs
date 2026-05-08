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
    public sealed class ListarProdutosHandler(IProdutoStoreLeitura store) : IRequestHandler<ListarProdutosQuery, IReadOnlyList<ProdutoModeloLeitura>>
    {
        public Task<IReadOnlyList<ProdutoModeloLeitura>> Handle(ListarProdutosQuery request, CancellationToken ct)
            => store.ListarAsync(request.IncluirDescontinuados, ct);
    }

    public record ListarProdutosQuery(bool IncluirDescontinuados = false) : IRequest<IReadOnlyList<ProdutoModeloLeitura>>;
}
