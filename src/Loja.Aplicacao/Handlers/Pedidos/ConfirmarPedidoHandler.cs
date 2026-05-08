using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using MediatR;

namespace Loja.Aplicacao.Handlers.Pedidos
{
    public sealed class ConfirmarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<ConfirmarPedidoCommand, Unit>
    {
        public async Task<Unit> Handle(ConfirmarPedidoCommand request, CancellationToken ct)
        {
            var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
            pedido.Confirmar();
            await repositorio.AnexarAsync(pedido, ct);
            return Unit.Value;
        }
    }

    public record ConfirmarPedidoCommand(Guid PedidoId) : IRequest<Unit>;
}
