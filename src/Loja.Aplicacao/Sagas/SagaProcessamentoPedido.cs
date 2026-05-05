using Loja.Dominio.Sagas.ProcessamentoPedido;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Loja.Aplicacao.Sagas;

/// <summary>
/// AULA 6 — Process Manager (Saga) que coordena o fluxo "Pedido confirmado → Entregue".
///
/// Estilo: ORQUESTRAÇÃO. A saga decide a próxima etapa em resposta a cada evento.
///
/// Fluxo do happy path:
///   PedidoConfirmado
///     → ReservarEstoque        ──> EstoqueReservado
///     → CobrarPagamento        ──> PagamentoConcluido
///     → AgendarEmbalagem       ──> EmbalagemAgendada
///     → Concluído
///
/// Falhas → executa transações COMPENSATÓRIAS (LiberarEstoque, etc.).
///
/// OBS: os métodos Start/Handle DEVEM permanecer em inglês — convenção do Wolverine.
/// </summary>
public sealed class SagaProcessamentoPedido : Saga
{
    public EstadoProcessamentoPedido State { get; set; } = new();

    /// <summary>
    /// Inicia a saga reagindo ao evento de domínio que vem do agregado Pedido.
    /// Aqui usamos uma mensagem de integração simples — em ambiente real,
    /// seria publicada pelo Marten Outbox quando PedidoConfirmado for persistido.
    /// </summary>
    public static (SagaProcessamentoPedido, ReservarEstoque) Start(IniciarProcessamentoPedido gatilho)
    {
        var saga = new SagaProcessamentoPedido
        {
            State =
            {
                Id = gatilho.CorrelacaoId,
                PedidoId = gatilho.PedidoId,
                ClienteId = gatilho.ClienteId,
                Valor = gatilho.Valor,
                Itens = gatilho.Itens.ToList(),
                Etapa = EtapaProcessamentoPedido.AguardandoReservaEstoque,
                IniciadoEm = DateTimeOffset.UtcNow
            }
        };

        var reservar = new ReservarEstoque(gatilho.PedidoId, gatilho.CorrelacaoId, gatilho.Itens);
        return (saga, reservar);
    }

    public CobrarPagamento Handle(EstoqueReservado msg, ILogger<SagaProcessamentoPedido> log)
    {
        log.LogInformation("[Saga {Cid}] Estoque reservado, cobrando pagamento...", State.Id);
        State.Etapa = EtapaProcessamentoPedido.AguardandoPagamento;
        return new CobrarPagamento(State.PedidoId, State.Id, State.ClienteId, State.Valor, "credit-card");
    }

    public LiberarEstoque Handle(ReservaEstoqueFalhou msg, ILogger<SagaProcessamentoPedido> log)
    {
        log.LogWarning("[Saga {Cid}] Falha ao reservar estoque: {Motivo}", State.Id, msg.Motivo);
        State.Etapa = EtapaProcessamentoPedido.Falhou;
        State.MotivoFalha = msg.Motivo;
        MarkCompleted();
        return new LiberarEstoque(State.PedidoId, State.Id);
    }

    public AgendarEmbalagem Handle(PagamentoConcluido msg, ILogger<SagaProcessamentoPedido> log)
    {
        log.LogInformation("[Saga {Cid}] Pagamento aprovado ({Tx}), agendando envio.", State.Id, msg.TransacaoId);
        State.TransacaoId = msg.TransacaoId;
        State.Etapa = EtapaProcessamentoPedido.AguardandoEmbalagem;
        return new AgendarEmbalagem(State.PedidoId, State.Id);
    }

    public LiberarEstoque Handle(PagamentoFalhou msg, ILogger<SagaProcessamentoPedido> log)
    {
        log.LogWarning("[Saga {Cid}] Pagamento negado: {Motivo}", State.Id, msg.Motivo);
        State.Etapa = EtapaProcessamentoPedido.Compensando;
        State.MotivoFalha = msg.Motivo;
        MarkCompleted();
        return new LiberarEstoque(State.PedidoId, State.Id);
    }

    public void Handle(EmbalagemAgendada msg, ILogger<SagaProcessamentoPedido> log)
    {
        log.LogInformation("[Saga {Cid}] Envio agendado, tracking={Track}.", State.Id, msg.CodigoRastreio);
        State.CodigoRastreio = msg.CodigoRastreio;
        State.Etapa = EtapaProcessamentoPedido.Concluido;
        State.ConcluidoEm = DateTimeOffset.UtcNow;
        MarkCompleted();
    }

    public LiberarEstoque Handle(EmbalagemFalhou msg, ILogger<SagaProcessamentoPedido> log)
    {
        log.LogWarning("[Saga {Cid}] Falha no envio: {Motivo}", State.Id, msg.Motivo);
        State.Etapa = EtapaProcessamentoPedido.Compensando;
        State.MotivoFalha = msg.Motivo;
        MarkCompleted();
        return new LiberarEstoque(State.PedidoId, State.Id);
    }

    /// <summary>
    /// AULA 6 — Timeouts: se uma etapa não responde, dispara compensação.
    /// </summary>
    public LiberarEstoque? Handle(TimeoutSaga msg, ILogger<SagaProcessamentoPedido> log)
    {
        if (State.Etapa is EtapaProcessamentoPedido.Concluido or EtapaProcessamentoPedido.Falhou)
            return null;
        log.LogWarning("[Saga {Cid}] Timeout em {Etapa}", State.Id, msg.Etapa);
        State.Etapa = EtapaProcessamentoPedido.Falhou;
        State.MotivoFalha = $"Timeout em {msg.Etapa}";
        MarkCompleted();
        return new LiberarEstoque(State.PedidoId, State.Id);
    }
}

/// <summary>
/// Mensagem de integração que dispara a saga.
/// Em produção, seria publicada via Outbox quando PedidoConfirmado é persistido.
/// </summary>
public sealed record IniciarProcessamentoPedido(
    Guid PedidoId,
    Guid ClienteId,
    Guid CorrelacaoId,
    decimal Valor,
    IReadOnlyList<LinhaEstoque> Itens);
