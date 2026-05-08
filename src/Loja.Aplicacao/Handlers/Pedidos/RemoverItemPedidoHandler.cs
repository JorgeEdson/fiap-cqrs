using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using MediatR;


namespace Loja.Aplicacao.Handlers.Pedidos
{
    public sealed class RemoverItemPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<RemoverItemPedidoCommand, Unit>
    {
        public async Task<Unit> Handle(RemoverItemPedidoCommand request, CancellationToken ct)
        {
            var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
            pedido.RemoverItem(request.ProdutoId, request.Quantidade);
            await repositorio.AnexarAsync(pedido, ct);
            return Unit.Value;
        }
    }

    public record RemoverItemPedidoCommand(Guid PedidoId, Guid ProdutoId, int Quantidade) : IRequest<Unit>;
}
