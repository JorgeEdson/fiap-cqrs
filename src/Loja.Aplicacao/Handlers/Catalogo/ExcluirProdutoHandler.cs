using Loja.Aplicacao.Handlers.Catalogo.Interfaces;
using MediatR;

namespace Loja.Aplicacao.Handlers.Catalogo
{
    public sealed class ExcluirProdutoHandler(IProdutoStoreEscrita store) : IRequestHandler<ExcluirProdutoCommand, bool>
    {
        public Task<bool> Handle(ExcluirProdutoCommand request, CancellationToken ct)
            => store.RemoverAsync(request.Id, ct);
    }

    public record ExcluirProdutoCommand(Guid Id) : IRequest<bool>;
}
