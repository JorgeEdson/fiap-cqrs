using MediatR;

namespace Loja.Aplicacao.Pedidos.Comandos;

/// <summary>
/// Comandos do agregado Pedido. Discutidos nas AULAS 2 e 3.
///
/// Cada Comando vira um único load → execute → append no Marten:
///  1. carrega o stream e faz replay (CarregarDoHistorico) → estado atual;
///  2. invoca o método correspondente no Aggregate (que valida e emite eventos);
///  3. anexa eventos ao stream com ExpectedVersion (concorrência otimista).
/// </summary>
public record CriarPedidoCommand(Guid ClienteId, string? EmailCliente) : IRequest<Guid>;

public record AdicionarItemPedidoCommand(Guid PedidoId, Guid ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade) : IRequest<Unit>;

public record RemoverItemPedidoCommand(Guid PedidoId, Guid ProdutoId, int Quantidade) : IRequest<Unit>;

public record ConfirmarPedidoCommand(Guid PedidoId) : IRequest<Unit>;

public record PagarPedidoCommand(Guid PedidoId, string MeioPagamento, string TransacaoId) : IRequest<Unit>;

public record EnviarPedidoCommand(Guid PedidoId, string CodigoRastreio, string Transportadora) : IRequest<Unit>;

public record CancelarPedidoCommand(Guid PedidoId, string Motivo) : IRequest<Unit>;
