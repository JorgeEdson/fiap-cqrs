using Loja.Aplicacao.Handlers.Catalogo.Interfaces;
using MediatR;

namespace Loja.Aplicacao.Handlers.Catalogo
{

    public sealed class DescontinuarProdutoHandler(IProdutoStoreEscrita store) : IRequestHandler<DescontinuarProdutoCommand, bool>
    {
        public async Task<bool> Handle(DescontinuarProdutoCommand request, CancellationToken ct)
        {
            var existente = await store.ObterAsync(request.Id, ct);
            if (existente is null) return false;
            existente.Descontinuado = true;
            return await store.SalvarAlteracoesAsync(ct);
        }
    }

    public record DescontinuarProdutoCommand(Guid Id) : IRequest<bool>;
}
