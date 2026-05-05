namespace Loja.Dominio.Sagas.ProcessamentoPedido;

public sealed class EstadoProcessamentoPedido
{
    public Guid Id { get; set; }          
    public Guid PedidoId { get; set; }
    public Guid ClienteId { get; set; }
    public decimal Valor { get; set; }
    public List<LinhaEstoque> Itens { get; set; } = new();
    public EtapaProcessamentoPedido Etapa { get; set; } = EtapaProcessamentoPedido.NaoIniciado;
    public string? MotivoFalha { get; set; }
    public string? TransacaoId { get; set; }
    public string? CodigoRastreio { get; set; }
    public DateTimeOffset IniciadoEm { get; set; }
    public DateTimeOffset? ConcluidoEm { get; set; }
}

public enum EtapaProcessamentoPedido
{
    NaoIniciado = 0,
    AguardandoReservaEstoque = 1,
    AguardandoPagamento = 2,
    AguardandoEmbalagem = 3,
    Concluido = 90,
    Compensando = 95,
    Falhou = 99
}
