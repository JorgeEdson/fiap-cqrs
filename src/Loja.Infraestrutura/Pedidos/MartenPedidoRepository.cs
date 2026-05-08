using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using Loja.Dominio.Compartilhado;
using Loja.Dominio.Pedidos;
using Marten;

namespace Loja.Infraestrutura.Pedidos;

public sealed class MartenPedidoRepository(IDocumentSession sessao) : IPedidoRepository
{
    public async Task<Pedido?> CarregarAsync(Guid pedidoId, CancellationToken ct)
    {
        var eventos = await sessao.Events.FetchStreamAsync(pedidoId, token: ct);
        if (eventos.Count == 0) return null;

        var pedido = new Pedido();
        pedido.CarregarDoHistorico(eventos.Select(e => (IEventoDominio)e.Data));
        return pedido;
    }

    public Task IniciarStreamAsync(Pedido pedido, CancellationToken ct)
    {
        var eventos = pedido.EventosPendentes.Cast<object>().ToArray();

        
        sessao.Events.StartStream<Pedido>(pedido.Id, eventos);
        pedido.LimparEventosPendentes();

        return sessao.SaveChangesAsync(ct);
    }

    public async Task AnexarAsync(Pedido pedido, CancellationToken ct)
    {
        var eventos = pedido.EventosPendentes.Cast<object>().ToArray();
        if (eventos.Length == 0) return;

       
        await sessao.Events.AppendOptimistic(pedido.Id, eventos);
        pedido.LimparEventosPendentes();

        await sessao.SaveChangesAsync(ct);
    }
}
