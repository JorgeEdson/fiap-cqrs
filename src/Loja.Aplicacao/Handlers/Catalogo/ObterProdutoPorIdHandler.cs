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
    public sealed class ObterProdutoPorIdHandler(IProdutoStoreLeitura store)
    : IRequestHandler<ObterProdutoPorIdQuery, ProdutoModeloLeitura?>
    {
        public Task<ProdutoModeloLeitura?> Handle(ObterProdutoPorIdQuery request, CancellationToken ct)
            => store.ObterAsync(request.Id, ct);
    }

    public record ObterProdutoPorIdQuery(Guid Id) : IRequest<ProdutoModeloLeitura?>;
}
