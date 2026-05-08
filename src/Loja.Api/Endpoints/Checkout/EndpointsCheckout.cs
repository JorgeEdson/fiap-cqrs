using Loja.Aplicacao.Handlers.Pedidos;
using Loja.Aplicacao.Sagas;
using Loja.Dominio.Pedidos.Snapshots;
using Loja.Dominio.Sagas.ProcessamentoPedido;
using Marten;
using MediatR;
using Wolverine;

namespace Loja.Api.Endpoints.Checkout;

/// <summary>
/// AULA 6 — Endpoint de "checkout" que dispara a Saga / Process Manager.
///
/// Demonstra o fluxo:
///   ConfirmarPedido (Marten append)
///     → Saga.Start (Wolverine)
///       → ReservarEstoque → CobrarPagamento → AgendarEmbalagem → Concluído
///
/// Endpoints adicionais permitem inspecionar o estado da saga no Marten.
/// </summary>
public static class EndpointsCheckout
{
    public static IEndpointRouteBuilder MapearCheckout(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/checkout").WithTags("Aula 6 · Saga / Process Manager");

        // 1) confirma o pedido (gera PedidoConfirmado) e dispara a saga.
        g.MapPost("/{pedidoId:guid}", async (Guid pedidoId, IMediator mediator,
            IMessageBus bus, IQuerySession query) =>
        {
            // Confirma o pedido — emite PedidoConfirmado na write-side.
            await mediator.Send(new ConfirmarPedidoCommand(pedidoId));

            // Carrega o snapshot para pegar itens e total.
            var snapshot = await query.LoadAsync<SnapshotPedido>(pedidoId)
                ?? throw new InvalidOperationException("Snapshot indisponível para iniciar saga.");

            var gatilho = new IniciarProcessamentoPedido(
                PedidoId: snapshot.Id,
                ClienteId: snapshot.ClienteId,
                CorrelacaoId: Guid.NewGuid(),
                Valor: snapshot.Total,
                Itens: snapshot.Itens
                    .Select(i => new LinhaEstoque(i.ProdutoId, i.Quantidade))
                    .ToList());

            await bus.PublishAsync(gatilho);
            return Results.Accepted($"/api/checkout/sagas/{gatilho.CorrelacaoId}", new { gatilho.CorrelacaoId });
        });

        // 2) inspeciona o estado da saga.
        g.MapGet("/sagas/{correlacaoId:guid}", async (Guid correlacaoId, IQuerySession query) =>
            await query.LoadAsync<EstadoProcessamentoPedido>(correlacaoId) is { } s
                ? Results.Ok(s) : Results.NotFound());

        return app;
    }
}
