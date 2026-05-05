namespace Loja.Dominio.Pedidos.Snapshots;

public sealed class SnapshotPedido
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string? EmailCliente { get; set; }
    public StatusPedido Status { get; set; }
    public DateTimeOffset CriadoEm { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
    public decimal Total { get; set; }
    public long Versao { get; set; }
}
