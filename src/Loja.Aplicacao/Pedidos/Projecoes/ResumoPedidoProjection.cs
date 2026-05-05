using Loja.Aplicacao.Pedidos.ModelosLeitura;
using Loja.Dominio.Pedidos;
using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;
using Marten.Events.Aggregation;

namespace Loja.Aplicacao.Pedidos.Projecoes;

/// <summary>
/// AULA 4 — Projeção INLINE (consistência forte) para o detalhe de um pedido.
///
/// Aplica eventos do stream sobre o documento <see cref="ResumoPedido"/>.
/// Por ser inline, é atualizada DENTRO da transação que persiste os eventos.
///
/// OBS: os métodos Create/Apply DEVEM permanecer em inglês —
///      são reflexionados pelo Marten por convenção de nome.
/// </summary>
public sealed class ResumoPedidoProjection : SingleStreamProjection<ResumoPedido>
{
    public ResumoPedido Create(PedidoCriado e) => new()
    {
        Id = e.PedidoId,
        ClienteId = e.ClienteId,
        Status = StatusPedido.Rascunho,
        CriadoEm = e.CriadoEm
    };

    public ResumoPedido Create(PedidoCriadoV2 e) => new()
    {
        Id = e.PedidoId,
        ClienteId = e.ClienteId,
        EmailCliente = e.EmailCliente,
        Status = StatusPedido.Rascunho,
        CriadoEm = e.CriadoEm
    };

    public void Apply(ItemPedidoAdicionado e, ResumoPedido atual)
    {
        var existente = atual.Itens.FirstOrDefault(i => i.ProdutoId == e.ProdutoId);
        if (existente is null)
            atual.Itens.Add(new ItemPedido(e.ProdutoId, e.NomeProduto, e.PrecoUnitario, e.Quantidade));
        else
        {
            atual.Itens.Remove(existente);
            atual.Itens.Add(existente with { Quantidade = existente.Quantidade + e.Quantidade });
        }
        atual.Total = atual.Itens.Sum(i => i.Subtotal);
    }

    public void Apply(ItemPedidoRemovido e, ResumoPedido atual)
    {
        var item = atual.Itens.FirstOrDefault(i => i.ProdutoId == e.ProdutoId);
        if (item is null) return;
        atual.Itens.Remove(item);
        if (item.Quantidade > e.Quantidade)
            atual.Itens.Add(item with { Quantidade = item.Quantidade - e.Quantidade });
        atual.Total = atual.Itens.Sum(i => i.Subtotal);
    }

    public void Apply(PedidoConfirmado e, ResumoPedido atual)
    {
        atual.Status = StatusPedido.Confirmado;
        atual.ConfirmadoEm = e.ConfirmadoEm;
        atual.Total = e.Total;
    }

    public void Apply(PedidoPago e, ResumoPedido atual)
    {
        atual.Status = StatusPedido.Pago;
        atual.PagoEm = e.PagoEm;
    }

    public void Apply(PedidoEnviado e, ResumoPedido atual)
    {
        atual.Status = StatusPedido.Enviado;
        atual.EnviadoEm = e.EnviadoEm;
        atual.CodigoRastreio = e.CodigoRastreio;
    }

    public void Apply(PedidoCancelado e, ResumoPedido atual)
    {
        atual.Status = StatusPedido.Cancelado;
        atual.CanceladoEm = e.CanceladoEm;
        atual.MotivoCancelamento = e.Motivo;
    }
}
