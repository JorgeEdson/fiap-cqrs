using Loja.Dominio.Pedidos;

namespace Loja.Aplicacao.Pedidos.ModelosLeitura;

/// <summary>
/// AULA 4 — Read Model otimizado para "ver um pedido por id".
///
/// Construído por uma PROJEÇÃO INLINE (atualizada na mesma transação dos eventos).
/// Inline traz CONSISTÊNCIA FORTE para esta visão e é ideal para detalhes
/// que aparecem logo após o command.
/// </summary>
public sealed class ResumoPedido
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string? EmailCliente { get; set; }
    public StatusPedido Status { get; set; }
    public DateTimeOffset CriadoEm { get; set; }
    public DateTimeOffset? ConfirmadoEm { get; set; }
    public DateTimeOffset? PagoEm { get; set; }
    public DateTimeOffset? EnviadoEm { get; set; }
    public DateTimeOffset? CanceladoEm { get; set; }
    public string? MotivoCancelamento { get; set; }
    public string? CodigoRastreio { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
    public decimal Total { get; set; }
}
