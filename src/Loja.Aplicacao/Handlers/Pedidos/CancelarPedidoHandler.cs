using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loja.Aplicacao.Handlers.Pedidos
{
    public sealed class CancelarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<CancelarPedidoCommand, Unit>
    {
        public async Task<Unit> Handle(CancelarPedidoCommand request, CancellationToken ct)
        {
            var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
            pedido.Cancelar(request.Motivo);
            await repositorio.AnexarAsync(pedido, ct);
            return Unit.Value;
        }
    }

    public record CancelarPedidoCommand(Guid PedidoId, string Motivo) : IRequest<Unit>;
}
