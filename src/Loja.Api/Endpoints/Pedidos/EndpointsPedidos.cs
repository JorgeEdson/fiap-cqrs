using Loja.Aplicacao.Handlers.Pedidos;
using Loja.Aplicacao.Handlers.Pedidos.Consultas.ModelosLeitura;
using Marten;
using MediatR;

namespace Loja.Api.Endpoints.Pedidos;

/// <summary>
/// Endpoints do agregado Pedido. Cobrem AULAS 2, 3, 4, 5:
///
///  - POST   /api/pedidos                   → AULA 3: criar (StartStream)
///  - POST   /api/pedidos/{id}/itens        → AULA 3: adicionar item (Append)
///  - DELETE /api/pedidos/{id}/itens/...    → AULA 3: remover item
///  - POST   /api/pedidos/{id}/confirmar    → AULA 3
///  - POST   /api/pedidos/{id}/pagar        → AULA 3
///  - POST   /api/pedidos/{id}/enviar       → AULA 3
///  - POST   /api/pedidos/{id}/cancelar     → AULA 3
///  - GET    /api/pedidos/{id}              → AULA 4: read model inline (ResumoPedido)
///  - GET    /api/pedidos/{id}/historico    → AULA 2/4: stream cru (auditoria)
///  - GET    /api/pedidos/{id}/snapshot     → AULA 5: snapshot persistido
///  - GET    /api/pedidos-dashboard         → AULA 4: read model assíncrono
///  - GET    /api/clientes/{id}/pedidos     → AULA 4: outro read model
/// </summary>
public static class EndpointsPedidos
{
    public static IEndpointRouteBuilder MapearPedidos(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/pedidos").WithTags("Aulas 2-5 · Pedidos (ES + CQRS)");

        // -------- COMANDOS (AULA 3) --------
        g.MapPost("/", async (CriarPedidoCommand cmd, IMediator m) =>
        {
            var id = await m.Send(cmd);
            return Results.Created($"/api/pedidos/{id}", new { id });
        });

        g.MapPost("/{id:guid}/itens", async (Guid id, CorpoAdicionarItem corpo, IMediator m) =>
        {
            await m.Send(new AdicionarItemPedidoCommand(id, corpo.ProdutoId, corpo.NomeProduto, corpo.PrecoUnitario, corpo.Quantidade));
            return Results.NoContent();
        });

        g.MapDelete("/{id:guid}/itens/{produtoId:guid}", async (Guid id, Guid produtoId, int quantidade, IMediator m) =>
        {
            await m.Send(new RemoverItemPedidoCommand(id, produtoId, quantidade));
            return Results.NoContent();
        });

        g.MapPost("/{id:guid}/confirmar", async (Guid id, IMediator m) =>
        {
            await m.Send(new ConfirmarPedidoCommand(id));
            return Results.NoContent();
        });

        g.MapPost("/{id:guid}/pagar", async (Guid id, CorpoPagar corpo, IMediator m) =>
        {
            await m.Send(new PagarPedidoCommand(id, corpo.MeioPagamento, corpo.TransacaoId));
            return Results.NoContent();
        });

        g.MapPost("/{id:guid}/enviar", async (Guid id, CorpoEnviar corpo, IMediator m) =>
        {
            await m.Send(new EnviarPedidoCommand(id, corpo.CodigoRastreio, corpo.Transportadora));
            return Results.NoContent();
        });

        g.MapPost("/{id:guid}/cancelar", async (Guid id, CorpoCancelar corpo, IMediator m) =>
        {
            await m.Send(new CancelarPedidoCommand(id, corpo.Motivo));
            return Results.NoContent();
        });

        // -------- CONSULTAS (AULA 4) --------
        g.MapGet("/{id:guid}", async (Guid id, IQuerySession query) =>
            await query.LoadAsync<ResumoPedido>(id) is { } s ? Results.Ok(s) : Results.NotFound());

        // AULA 2/4: histórico cru de eventos — auditoria nativa, time travel.
        g.MapGet("/{id:guid}/historico", async (Guid id, IQuerySession query) =>
        {
            var eventos = await query.Events.FetchStreamAsync(id);
            return Results.Ok(eventos.Select(e => new
            {
                versao = e.Version,
                tipoEvento = e.EventTypeName,
                ocorridoEm = e.Timestamp,
                dados = e.Data
            }));
        });

        // AULA 5: lê o snapshot persistido do agregado.
        g.MapGet("/{id:guid}/snapshot", async (Guid id, IQuerySession query) =>
            await query.LoadAsync<Loja.Dominio.Pedidos.Snapshots.SnapshotPedido>(id) is { } s
                ? Results.Ok(s) : Results.NotFound());

        // AULA 4: read model assíncrono (consistência eventual).
        app.MapGet("/api/pedidos-dashboard", async (IQuerySession query) =>
            await query.LoadAsync<DashboardPedidos>(DashboardPedidos.SingletonId) is { } d
                ? Results.Ok(d) : Results.Ok(new DashboardPedidos()))
            .WithTags("Aula 4 · Read Models");

        // AULA 4: histórico de pedidos por cliente.
        app.MapGet("/api/clientes/{id:guid}/pedidos", async (Guid id, IQuerySession query) =>
            await query.LoadAsync<PedidosCliente>(id) is { } c ? Results.Ok(c) : Results.NotFound())
            .WithTags("Aula 4 · Read Models");

        return app;
    }

    public sealed record CorpoAdicionarItem(Guid ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade);
    public sealed record CorpoPagar(string MeioPagamento, string TransacaoId);
    public sealed record CorpoEnviar(string CodigoRastreio, string Transportadora);
    public sealed record CorpoCancelar(string Motivo);
}
