using Loja.Dominio.Pedidos;

namespace Loja.Aplicacao.Handlers.Pedidos.Consultas.ModelosLeitura;

/// <summary>
/// AULA 4 — Outro Read Model assíncrono: histórico de pedidos por cliente.
///
/// Mostra que vários read models podem coexistir sobre o MESMO stream
/// de eventos, cada um modelado para um caso de uso específico.
/// </summary>
public sealed class PedidosCliente
{
    public Guid Id { get; set; }                    // ClienteId
    public List<EntradaPedidoCliente> Pedidos { get; set; } = new();
    public decimal ValorTotalCliente { get; set; }
}

public sealed record EntradaPedidoCliente(
    Guid PedidoId,
    StatusPedido Status,
    DateTimeOffset CriadoEm,
    decimal Total);
