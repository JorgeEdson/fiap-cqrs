using Loja.Dominio.Pedidos;

namespace Loja.Aplicacao.Handlers.Pedidos.Interfaces;

public interface IPedidoRepository
{

    Task<Pedido?> CarregarAsync(Guid pedidoId, CancellationToken ct);


    Task IniciarStreamAsync(Pedido pedido, CancellationToken ct);


    Task AnexarAsync(Pedido pedido, CancellationToken ct);
}

internal static class PedidoRepositoryExtensoes
{
    public static async Task<Pedido> CarregarOuFalharAsync(this IPedidoRepository repo, Guid id, CancellationToken ct)
        => await repo.CarregarAsync(id, ct) ?? throw new InvalidOperationException($"Pedido {id} não encontrado.");
}
