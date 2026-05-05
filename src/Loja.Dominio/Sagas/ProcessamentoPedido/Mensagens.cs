namespace Loja.Dominio.Sagas.ProcessamentoPedido;

// ---------------- Pagamento ----------------
public record CobrarPagamento(Guid PedidoId, Guid CorrelacaoId, Guid ClienteId, decimal Valor, string MeioPagamento);
public record PagamentoConcluido(Guid PedidoId, Guid CorrelacaoId, string TransacaoId);
public record PagamentoFalhou(Guid PedidoId, Guid CorrelacaoId, string Motivo);

// ---------------- Estoque ------------------
public record ReservarEstoque(Guid PedidoId, Guid CorrelacaoId, IReadOnlyList<LinhaEstoque> Itens);
public record EstoqueReservado(Guid PedidoId, Guid CorrelacaoId);
public record ReservaEstoqueFalhou(Guid PedidoId, Guid CorrelacaoId, string Motivo);
public record LiberarEstoque(Guid PedidoId, Guid CorrelacaoId);  // ação compensatória
public record LinhaEstoque(Guid ProdutoId, int Quantidade);

// ---------------- Envio --------------------
public record AgendarEmbalagem(Guid PedidoId, Guid CorrelacaoId);
public record EmbalagemAgendada(Guid PedidoId, Guid CorrelacaoId, string CodigoRastreio, string Transportadora);
public record EmbalagemFalhou(Guid PedidoId, Guid CorrelacaoId, string Motivo);

// ---------------- Saga timeouts ------------
public record TimeoutSaga(Guid PedidoId, Guid CorrelacaoId, string Etapa);
