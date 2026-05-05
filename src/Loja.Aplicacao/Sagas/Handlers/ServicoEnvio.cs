using Loja.Dominio.Sagas.ProcessamentoPedido;
using Microsoft.Extensions.Logging;

namespace Loja.Aplicacao.Sagas.Handlers;

/// <summary>AULA 6 — Envio simulado.</summary>
public static class ServicoEnvio
{
    public static EmbalagemAgendada Handle(AgendarEmbalagem cmd, ILogger<MarcadorEnvio> log)
    {
        var rastreio = $"BR{Random.Shared.Next(10_000_000, 99_999_999)}";
        log.LogInformation("[Envio] Pedido {PedidoId} agendado: {Rastreio} (corr={Cid})",
            cmd.PedidoId, rastreio, cmd.CorrelacaoId);

        return new EmbalagemAgendada(cmd.PedidoId, cmd.CorrelacaoId, rastreio, "Correios");
    }
}

public sealed class MarcadorEnvio { }
