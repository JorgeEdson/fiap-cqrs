using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using MediatR;


namespace Loja.Aplicacao.Handlers.Pedidos
{
    public sealed class AdicionarItemPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<AdicionarItemPedidoCommand, Unit>
    {
        public async Task<Unit> Handle(AdicionarItemPedidoCommand request, CancellationToken ct)
        {
            var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
            pedido.AdicionarItem(request.ProdutoId, request.NomeProduto, request.PrecoUnitario, request.Quantidade);
            await repositorio.AnexarAsync(pedido, ct);
            return Unit.Value;
        }
    }

    public record AdicionarItemPedidoCommand(Guid PedidoId, Guid ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade) : IRequest<Unit>;
}
