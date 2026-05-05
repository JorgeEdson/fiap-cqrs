using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;

namespace Loja.Aplicacao.Pedidos.Versionamento;

/// <summary>
/// AULA 5 — Upcaster: converte eventos antigos (V1) para a forma atual (V2)
/// EM TEMPO DE LEITURA, sem reescrever o histórico (que é imutável).
///
/// Usado em <c>StoreOptions.Events.Upcast&lt;PedidoCriado, PedidoCriadoV2&gt;(...)</c>.
///
/// Estratégias alternativas discutidas em aula:
///   - default value (campo novo recebe valor padrão);
///   - lookup em fonte externa (ex: serviço de cliente);
///   - múltiplos upcasters em cadeia (V1 → V2 → V3).
/// </summary>
public static class PedidoCriadoV1ParaV2Upcaster
{
    /// <summary>
    /// Estratégia simples: V1 não tem EmailCliente; preenchemos com placeholder.
    /// Em sala, mostre como trocar por um lookup real (ex: IClienteLookup).
    /// </summary>
    public static PedidoCriadoV2 Converter(PedidoCriado legado)
        => new(legado.PedidoId, legado.ClienteId, "desconhecido@legado", legado.CriadoEm);
}
