using Loja.Dominio.Compartilhado;

namespace Loja.Dominio.Pedidos.Eventos.V1;

public record PedidoCriado(
    Guid PedidoId,
    Guid ClienteId,
    DateTimeOffset CriadoEm) : IEventoDominio;

public record ItemPedidoAdicionado(
    Guid PedidoId,
    Guid ProdutoId,
    string NomeProduto,
    decimal PrecoUnitario,
    int Quantidade) : IEventoDominio;

public record ItemPedidoRemovido(
    Guid PedidoId,
    Guid ProdutoId,
    int Quantidade) : IEventoDominio;

public record PedidoConfirmado(
    Guid PedidoId,
    decimal Total,
    DateTimeOffset ConfirmadoEm) : IEventoDominio;

public record PedidoPago(
    Guid PedidoId,
    string MeioPagamento,
    string TransacaoId,
    DateTimeOffset PagoEm) : IEventoDominio;

public record PedidoEnviado(
    Guid PedidoId,
    string CodigoRastreio,
    string Transportadora,
    DateTimeOffset EnviadoEm) : IEventoDominio;

public record PedidoCancelado(
    Guid PedidoId,
    string Motivo,
    DateTimeOffset CanceladoEm) : IEventoDominio;
