namespace Loja.Aplicacao.Pedidos.ModelosLeitura;

/// <summary>
/// AULA 4 — Read Model AGREGADO para um dashboard operacional.
///
/// Construído por projeção ASSÍNCRONA (Async Daemon do Marten).
/// Tolera consistência eventual em troca de baixa latência no write-side.
/// </summary>
public sealed class DashboardPedidos
{
    public Guid Id { get; set; } = SingletonId; // sempre o mesmo doc
    public int Rascunhos { get; set; }
    public int Confirmados { get; set; }
    public int Pagos { get; set; }
    public int Enviados { get; set; }
    public int Cancelados { get; set; }
    public decimal TotalVendido { get; set; }
    public DateTimeOffset AtualizadoEm { get; set; }

    public static readonly Guid SingletonId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");
}
