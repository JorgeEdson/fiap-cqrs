using Loja.Dominio.Sagas.ProcessamentoPedido;
using Microsoft.Extensions.Logging;

namespace Loja.Aplicacao.Sagas.Handlers;

/// <summary>
/// AULA 6 — Serviço de pagamento SIMULADO.
/// Em arquitetura real, viraria um microsserviço próprio com seu DB e infra.
///
/// OBS: o método Handle DEVE permanecer em inglês — convenção do Wolverine.
/// </summary>
public static class ServicoPagamento
{
    public static object Handle(CobrarPagamento cmd, ILogger<MarcadorPagamento> log)
    {
        log.LogInformation("[Pagamento] Cobrando R$ {Valor} via {Meio} (corr={Cid})",
            cmd.Valor, cmd.MeioPagamento, cmd.CorrelacaoId);

        if (cmd.Valor > 10_000m)
            return new PagamentoFalhou(cmd.PedidoId, cmd.CorrelacaoId, "Valor acima do limite.");
        if (cmd.MeioPagamento.Contains("falha", StringComparison.OrdinalIgnoreCase))
            return new PagamentoFalhou(cmd.PedidoId, cmd.CorrelacaoId, "Pagamento recusado pela operadora.");

        return new PagamentoConcluido(cmd.PedidoId, cmd.CorrelacaoId, $"TX-{Guid.NewGuid():N}");
    }
}

// ILogger<T> precisa de um T concreto para identificar o canal de log.
public sealed class MarcadorPagamento { }
