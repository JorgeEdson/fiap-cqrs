using Loja.Dominio.Comum;

namespace Loja.Dominio.Pedidos.Eventos.V2;

public record PedidoCriadoV2(
    Guid PedidoId,
    Guid ClienteId,
    string EmailCliente,
    DateTimeOffset CriadoEm) : IEventoDominio;
