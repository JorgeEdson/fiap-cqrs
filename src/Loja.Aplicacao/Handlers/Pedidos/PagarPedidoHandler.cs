using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using MediatR;


namespace Loja.Aplicacao.Handlers.Pedidos
{
    public sealed class PagarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<PagarPedidoCommand, Unit>
    {
        public async Task<Unit> Handle(PagarPedidoCommand request, CancellationToken ct)
        {
            var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
            pedido.Pagar(request.MeioPagamento, request.TransacaoId);
            await repositorio.AnexarAsync(pedido, ct);
            return Unit.Value;
        }
    }

    public record PagarPedidoCommand(Guid PedidoId, string MeioPagamento, string TransacaoId) : IRequest<Unit>;
}
