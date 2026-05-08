using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using MediatR;

namespace Loja.Aplicacao.Handlers.Pedidos
{
    public sealed class EnviarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<EnviarPedidoCommand, Unit>
    {
        public async Task<Unit> Handle(EnviarPedidoCommand request, CancellationToken ct)
        {
            var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
            pedido.Enviar(request.CodigoRastreio, request.Transportadora);
            await repositorio.AnexarAsync(pedido, ct);
            return Unit.Value;
        }
    }

    public record EnviarPedidoCommand(Guid PedidoId, string CodigoRastreio, string Transportadora) : IRequest<Unit>;
}
