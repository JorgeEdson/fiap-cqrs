using Loja.Aplicacao.Pedidos.Comandos;
using Loja.Dominio.Pedidos;
using MediatR;

namespace Loja.Aplicacao.Pedidos.Handlers;

/// <summary>
/// AULA 3 — Padrão load–execute–append em todos os handlers.
///
/// O handler sempre:
///   1. CARREGA o stream (replay → estado);
///   2. EXECUTA o command (Aggregate valida e emite events);
///   3. ANEXA os events novos ao stream com ExpectedVersion (Marten cuida).
/// </summary>
internal static class PedidoRepositoryExtensoes
{
    public static async Task<Pedido> CarregarOuFalharAsync(this IPedidoRepository repo, Guid id, CancellationToken ct)
        => await repo.CarregarAsync(id, ct) ?? throw new InvalidOperationException($"Pedido {id} não encontrado.");
}

public sealed class CriarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<CriarPedidoCommand, Guid>
{
    public async Task<Guid> Handle(CriarPedidoCommand request, CancellationToken ct)
    {
        var pedidoId = Guid.NewGuid();
        var pedido = Pedido.Criar(pedidoId, request.ClienteId, request.EmailCliente);
        await repositorio.IniciarStreamAsync(pedido, ct);
        return pedidoId;
    }
}

public sealed class AdicionarItemPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<AdicionarItemPedidoCommand, Unit>
{
    public async Task<Unit> Handle(AdicionarItemPedidoCommand request, CancellationToken ct)
    {
        var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
        pedido.AdicionarItem(request.ProdutoId, request.NomeProduto, request.PrecoUnitario, request.Quantidade);
        await repositorio.AnexarAsync(pedido, ct);
        return Unit.Value;
    }
}

public sealed class RemoverItemPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<RemoverItemPedidoCommand, Unit>
{
    public async Task<Unit> Handle(RemoverItemPedidoCommand request, CancellationToken ct)
    {
        var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
        pedido.RemoverItem(request.ProdutoId, request.Quantidade);
        await repositorio.AnexarAsync(pedido, ct);
        return Unit.Value;
    }
}

public sealed class ConfirmarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<ConfirmarPedidoCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmarPedidoCommand request, CancellationToken ct)
    {
        var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
        pedido.Confirmar();
        await repositorio.AnexarAsync(pedido, ct);
        return Unit.Value;
    }
}

public sealed class PagarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<PagarPedidoCommand, Unit>
{
    public async Task<Unit> Handle(PagarPedidoCommand request, CancellationToken ct)
    {
        var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
        pedido.Pagar(request.MeioPagamento, request.TransacaoId);
        await repositorio.AnexarAsync(pedido, ct);
        return Unit.Value;
    }
}

public sealed class EnviarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<EnviarPedidoCommand, Unit>
{
    public async Task<Unit> Handle(EnviarPedidoCommand request, CancellationToken ct)
    {
        var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
        pedido.Enviar(request.CodigoRastreio, request.Transportadora);
        await repositorio.AnexarAsync(pedido, ct);
        return Unit.Value;
    }
}

public sealed class CancelarPedidoHandler(IPedidoRepository repositorio) : IRequestHandler<CancelarPedidoCommand, Unit>
{
    public async Task<Unit> Handle(CancelarPedidoCommand request, CancellationToken ct)
    {
        var pedido = await repositorio.CarregarOuFalharAsync(request.PedidoId, ct);
        pedido.Cancelar(request.Motivo);
        await repositorio.AnexarAsync(pedido, ct);
        return Unit.Value;
    }
}
