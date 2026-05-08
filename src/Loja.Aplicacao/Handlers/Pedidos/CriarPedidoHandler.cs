using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using Loja.Dominio.Pedidos;
using MediatR;


namespace Loja.Aplicacao.Handlers.Pedidos
{
    public sealed class CriarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<CriarPedidoCommand, Guid>
    {
        public async Task<Guid> Handle(CriarPedidoCommand request, CancellationToken ct)
        {
            var pedidoId = Guid.NewGuid();
            var pedido = Pedido.Criar(pedidoId, request.ClienteId, request.EmailCliente);
            await repositorio.IniciarStreamAsync(pedido, ct);
            return pedidoId;
        }
    }

    public record CriarPedidoCommand(Guid ClienteId, string? EmailCliente) : IRequest<Guid>;
}
