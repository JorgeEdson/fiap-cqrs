using Loja.Aplicacao.Pedidos.ModelosLeitura;
using Loja.Dominio.Pedidos;
using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;
using Marten;
using Marten.Events;
using Marten.Events.Projections;

namespace Loja.Aplicacao.Pedidos.Projecoes;

/// <summary>
/// AULA 4 — Projeção ASSÍNCRONA por cliente: cada documento <see cref="PedidosCliente"/>
/// agrupa o histórico de pedidos do cliente.
/// </summary>
public sealed class PedidosClienteProjection : EventProjection
{
    public async Task Project(IEvent<PedidoCriado> e, IDocumentOperations ops)
        => await UpsertEntrada(ops, e.Data.PedidoId, e.Data.ClienteId,
            entrada => entrada with { Status = StatusPedido.Rascunho, CriadoEm = e.Data.CriadoEm });

    public async Task Project(IEvent<PedidoCriadoV2> e, IDocumentOperations ops)
        => await UpsertEntrada(ops, e.Data.PedidoId, e.Data.ClienteId,
            entrada => entrada with { Status = StatusPedido.Rascunho, CriadoEm = e.Data.CriadoEm });

    public async Task Project(IEvent<PedidoConfirmado> e, IDocumentOperations ops)
        => await AtualizarEntrada(ops, e.Data.PedidoId,
            entrada => entrada with { Status = StatusPedido.Confirmado, Total = e.Data.Total });

    public async Task Project(IEvent<PedidoPago> e, IDocumentOperations ops)
        => await AtualizarEntrada(ops, e.Data.PedidoId, entrada => entrada with { Status = StatusPedido.Pago });

    public async Task Project(IEvent<PedidoEnviado> e, IDocumentOperations ops)
        => await AtualizarEntrada(ops, e.Data.PedidoId, entrada => entrada with { Status = StatusPedido.Enviado });

    public async Task Project(IEvent<PedidoCancelado> e, IDocumentOperations ops)
        => await AtualizarEntrada(ops, e.Data.PedidoId, entrada => entrada with { Status = StatusPedido.Cancelado });

    private static async Task UpsertEntrada(
        IDocumentOperations ops, Guid pedidoId, Guid clienteId,
        Func<EntradaPedidoCliente, EntradaPedidoCliente> mutar)
    {
        var doc = await ops.LoadAsync<PedidosCliente>(clienteId) ?? new PedidosCliente { Id = clienteId };
        var existente = doc.Pedidos.FirstOrDefault(o => o.PedidoId == pedidoId);
        var fresca = existente ?? new EntradaPedidoCliente(pedidoId, StatusPedido.Rascunho, DateTimeOffset.MinValue, 0m);
        var atualizada = mutar(fresca);
        if (existente is not null) doc.Pedidos.Remove(existente);
        doc.Pedidos.Add(atualizada);
        ops.Store(doc);
    }

    private static async Task AtualizarEntrada(
        IDocumentOperations ops, Guid pedidoId,
        Func<EntradaPedidoCliente, EntradaPedidoCliente> mutar)
    {
        // Em produção, manteríamos um índice secundário; aqui mantemos didático.
        var doc = await ops.Query<PedidosCliente>()
            .FirstOrDefaultAsync(c => c.Pedidos.Any(o => o.PedidoId == pedidoId));
        if (doc is null) return;

        var existente = doc.Pedidos.First(o => o.PedidoId == pedidoId);
        var atualizada = mutar(existente);
        doc.Pedidos.Remove(existente);
        doc.Pedidos.Add(atualizada);
        doc.ValorTotalCliente = doc.Pedidos
            .Where(o => o.Status is StatusPedido.Confirmado or StatusPedido.Pago or StatusPedido.Enviado)
            .Sum(o => o.Total);
        ops.Store(doc);
    }
}
