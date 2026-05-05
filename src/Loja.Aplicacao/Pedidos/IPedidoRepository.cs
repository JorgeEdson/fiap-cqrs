using Loja.Dominio.Pedidos;

namespace Loja.Aplicacao.Pedidos;

/// <summary>
/// AULA 3 — Repositório do agregado Pedido sobre Marten.
///
/// Esconde a IDocumentSession por trás de uma fachada didática que torna
/// o ciclo load → execute → append explícito no handler.
/// </summary>
public interface IPedidoRepository
{
    /// <summary>Carrega o agregado fazendo replay dos eventos do stream.</summary>
    Task<Pedido?> CarregarAsync(Guid pedidoId, CancellationToken ct);

    /// <summary>Inicia um novo stream para um agregado recém-criado.</summary>
    Task IniciarStreamAsync(Pedido pedido, CancellationToken ct);

    /// <summary>
    /// Anexa eventos novos ao stream existente, usando ExpectedVersion
    /// para garantir CONCORRÊNCIA OTIMISTA.
    /// </summary>
    Task AnexarAsync(Pedido pedido, CancellationToken ct);
}
