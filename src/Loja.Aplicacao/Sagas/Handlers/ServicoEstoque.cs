using Loja.Dominio.Sagas.ProcessamentoPedido;
using Microsoft.Extensions.Logging;

namespace Loja.Aplicacao.Sagas.Handlers;

/// <summary>AULA 6 — Estoque simulado: reserva/libera estoque.</summary>
public static class ServicoEstoque
{
    public static object Handle(ReservarEstoque cmd, ILogger<MarcadorEstoque> log)
    {
        log.LogInformation("[Estoque] Reservando {Qtd} itens (corr={Cid})",
            cmd.Itens.Sum(i => i.Quantidade), cmd.CorrelacaoId);

        // Para demo: itens com Quantidade > 100 falham.
        if (cmd.Itens.Any(i => i.Quantidade > 100))
            return new ReservaEstoqueFalhou(cmd.PedidoId, cmd.CorrelacaoId, "Sem estoque suficiente.");

        return new EstoqueReservado(cmd.PedidoId, cmd.CorrelacaoId);
    }

    public static void Handle(LiberarEstoque cmd, ILogger<MarcadorEstoque> log)
        => log.LogInformation("[Estoque] Compensando: liberando estoque (corr={Cid})", cmd.CorrelacaoId);
}

public sealed class MarcadorEstoque { }
