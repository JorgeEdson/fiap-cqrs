using Loja.Dominio.Sagas.ProcessamentoPedido;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Loja.Aplicacao.Sagas;

public sealed class SagaProcessamentoPedido : Saga
{
    public EstadoProcessamentoPedido State { get; set; } = new();
 
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
    
    public LiberarEstoque? Handle(TimeoutSaga msg, ILogger<SagaProcessamentoPedido> log)
    {
        if (State.Etapa is EtapaProcessamentoPedido.Concluido or EtapaProcessamentoPedido.Falhou)
            return null;
        log.LogWarning("[Saga {Cid}] Timeout em {Etapa}", State.Id, msg.Etapa);
        State.Etapa = EtapaProcessamentoPedido.Falhou;
        State.MotivoFalha = $"Timeout em {msg.Etapa}";
        MarkCompleted();
        return new LiberarEstoque(State.PedidoId, State.Id);
    }}


public sealed record IniciarProcessamentoPedido(
    Guid PedidoId,
    Guid ClienteId,
    Guid CorrelacaoId,
    decimal Valor,
    IReadOnlyList<LinhaEstoque> Itens);
