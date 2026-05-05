using Loja.Aplicacao.Pedidos.ModelosLeitura;
using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;
using Marten;
using Marten.Events;
using Marten.Events.Projections;

namespace Loja.Aplicacao.Pedidos.Projecoes;

/// <summary>
/// AULA 4 — Projeção ASSÍNCRONA que agrega TODOS os streams de pedidos
/// em UM ÚNICO documento singleton (<see cref="DashboardPedidos"/>) para um dashboard global.
///
/// Roda no Async Daemon do Marten — atualizada eventualmente, sem onerar o write-side.
///
/// OBS: o método Project DEVE permanecer em inglês — convenção do Marten.
/// </summary>
public sealed class DashboardPedidosProjection : EventProjection
{
    public async Task Project(IEvent<PedidoCriado> e, IDocumentOperations ops)
        => await Atualizar(ops, d => d.Rascunhos++, e.Timestamp);

    public async Task Project(IEvent<PedidoCriadoV2> e, IDocumentOperations ops)
        => await Atualizar(ops, d => d.Rascunhos++, e.Timestamp);

    public async Task Project(IEvent<PedidoConfirmado> e, IDocumentOperations ops)
        => await Atualizar(ops, d =>
        {
            d.Rascunhos = Math.Max(0, d.Rascunhos - 1);
            d.Confirmados++;
            d.TotalVendido += e.Data.Total;
        }, e.Timestamp);

    public async Task Project(IEvent<PedidoPago> e, IDocumentOperations ops)
        => await Atualizar(ops, d =>
        {
            d.Confirmados = Math.Max(0, d.Confirmados - 1);
            d.Pagos++;
        }, e.Timestamp);

    public async Task Project(IEvent<PedidoEnviado> e, IDocumentOperations ops)
        => await Atualizar(ops, d =>
        {
            d.Pagos = Math.Max(0, d.Pagos - 1);
            d.Enviados++;
        }, e.Timestamp);

    public async Task Project(IEvent<PedidoCancelado> e, IDocumentOperations ops)
        => await Atualizar(ops, d => d.Cancelados++, e.Timestamp);

    private static async Task Atualizar(IDocumentOperations ops, Action<DashboardPedidos> mutar, DateTimeOffset ts)
    {
        var dashboard =
            await ops.LoadAsync<DashboardPedidos>(DashboardPedidos.SingletonId)
            ?? new DashboardPedidos { Id = DashboardPedidos.SingletonId };

        mutar(dashboard);
        dashboard.AtualizadoEm = ts;
        ops.Store(dashboard);
    }
}
