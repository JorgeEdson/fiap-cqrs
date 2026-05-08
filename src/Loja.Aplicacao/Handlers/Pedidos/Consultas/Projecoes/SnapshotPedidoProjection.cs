using Loja.Dominio.Pedidos;
using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;
using Loja.Dominio.Pedidos.Snapshots;
using Marten.Events.Aggregation;

namespace Loja.Aplicacao.Handlers.Pedidos.Consultas.Projecoes;

/// <summary>
/// AULA 5 — Snapshot do agregado Pedido como uma SingleStreamProjection do Marten.
///
/// Funciona como um CACHE PERSISTENTE do estado: ao invés de fazer replay de N eventos,
/// carregamos o snapshot mais recente e aplicamos só os eventos posteriores.
///
/// Mostre na demo:
///  - Como configurar Inline (sempre atualizado) vs. Async (eventualmente);
///  - Como o "Live aggregation" do Marten usa o snapshot quando está disponível.
/// </summary>
public sealed class SnapshotPedidoProjection : SingleStreamProjection<SnapshotPedido>
{
    public SnapshotPedido Create(PedidoCriado e) => new()
    {
        Id = e.PedidoId,
        ClienteId = e.ClienteId,
        Status = StatusPedido.Rascunho,
        CriadoEm = e.CriadoEm
    };

    public SnapshotPedido Create(PedidoCriadoV2 e) => new()
    {
        Id = e.PedidoId,
        ClienteId = e.ClienteId,
        EmailCliente = e.EmailCliente,
        Status = StatusPedido.Rascunho,
        CriadoEm = e.CriadoEm
    };

    public void Apply(ItemPedidoAdicionado e, SnapshotPedido s)
    {
        var existente = s.Itens.FirstOrDefault(i => i.ProdutoId == e.ProdutoId);
        if (existente is null)
            s.Itens.Add(new ItemPedido(e.ProdutoId, e.NomeProduto, e.PrecoUnitario, e.Quantidade));
        else
        {
            s.Itens.Remove(existente);
            s.Itens.Add(existente with { Quantidade = existente.Quantidade + e.Quantidade });
        }
        s.Total = s.Itens.Sum(i => i.Subtotal);
    }

    public void Apply(ItemPedidoRemovido e, SnapshotPedido s)
    {
        var item = s.Itens.FirstOrDefault(i => i.ProdutoId == e.ProdutoId);
        if (item is null) return;
        s.Itens.Remove(item);
        if (item.Quantidade > e.Quantidade) s.Itens.Add(item with { Quantidade = item.Quantidade - e.Quantidade });
        s.Total = s.Itens.Sum(i => i.Subtotal);
    }

    public void Apply(PedidoConfirmado e, SnapshotPedido s)
    {
        s.Status = StatusPedido.Confirmado;
        s.Total = e.Total;
    }

    public void Apply(PedidoPago _, SnapshotPedido s) => s.Status = StatusPedido.Pago;
    public void Apply(PedidoEnviado _, SnapshotPedido s) => s.Status = StatusPedido.Enviado;
    public void Apply(PedidoCancelado _, SnapshotPedido s) => s.Status = StatusPedido.Cancelado;
}
