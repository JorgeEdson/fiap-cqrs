using FluentAssertions;
using Loja.Dominio.Compartilhado;
using Loja.Dominio.Pedidos;
using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;
using Xunit;

namespace Loja.Dominio.Testes.Pedidos;

public sealed class TestesAgregadoPedido
{
    private static readonly Guid PedidoId  = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid ClienteId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid ProdutoA  = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    [Fact]
    public void Criar_emite_PedidoCriado()
    {
        var pedido = Pedido.Criar(PedidoId, ClienteId);

        pedido.EventosPendentes.Should().ContainSingle()
              .Which.Should().BeOfType<PedidoCriado>()
              .Which.PedidoId.Should().Be(PedidoId);
        pedido.Status.Should().Be(StatusPedido.Rascunho);
    }

    [Fact]
    public void Criar_com_email_emite_PedidoCriadoV2()
    {
        var pedido = Pedido.Criar(PedidoId, ClienteId, "ana@loja.local");

        pedido.EventosPendentes.Should().ContainSingle()
              .Which.Should().BeOfType<PedidoCriadoV2>();
        pedido.EmailCliente.Should().Be("ana@loja.local");
    }

    [Fact]
    public void Confirmar_pedido_vazio_dispara_RegraNegocioException()
    {
        var pedido = Dado(new PedidoCriado(PedidoId, ClienteId, DateTimeOffset.UtcNow));

        var acao = () => pedido.Confirmar();

        acao.Should().Throw<RegraNegocioException>().WithMessage("*pedido vazio*");
    }

    [Fact]
    public void Confirmar_acumula_total_e_atualiza_status()
    {
        var pedido = Dado(
            new PedidoCriado(PedidoId, ClienteId, DateTimeOffset.UtcNow),
            new ItemPedidoAdicionado(PedidoId, ProdutoA, "Notebook", 5000m, 2));
        pedido.LimparEventosPendentes();

        pedido.Confirmar();

        pedido.Status.Should().Be(StatusPedido.Confirmado);
        pedido.EventosPendentes.Should().ContainSingle()
              .Which.Should().BeOfType<PedidoConfirmado>()
              .Which.Total.Should().Be(10000m);
    }

    [Fact]
    public void Pagar_em_pedido_em_rascunho_lanca_excecao()
    {
        var pedido = Dado(new PedidoCriado(PedidoId, ClienteId, DateTimeOffset.UtcNow));

        var acao = () => pedido.Pagar("credit-card", "TX-1");

        acao.Should().Throw<RegraNegocioException>();
    }

    [Fact]
    public void Cancelar_apos_envio_eh_proibido()
    {
        var pedido = Dado(
            new PedidoCriado(PedidoId, ClienteId, DateTimeOffset.UtcNow),
            new ItemPedidoAdicionado(PedidoId, ProdutoA, "Notebook", 100m, 1),
            new PedidoConfirmado(PedidoId, 100m, DateTimeOffset.UtcNow),
            new PedidoPago(PedidoId, "credit-card", "TX-1", DateTimeOffset.UtcNow),
            new PedidoEnviado(PedidoId, "BR123", "Correios", DateTimeOffset.UtcNow));

        var acao = () => pedido.Cancelar("arrependimento");
        acao.Should().Throw<RegraNegocioException>();
    }

    [Fact]
    public void Concorrencia_otimista_versao_aumenta_a_cada_evento_aplicado()
    {
        var pedido = Dado(new PedidoCriado(PedidoId, ClienteId, DateTimeOffset.UtcNow));
        pedido.Versao.Should().Be(1);

        pedido.AdicionarItem(ProdutoA, "Camiseta", 50m, 3);
        pedido.Versao.Should().Be(2);
    }
    
    private static Pedido Dado(params IEventoDominio[] historico)
    {
        var pedido = new Pedido();
        pedido.CarregarDoHistorico(historico);
        return pedido;
    }
}
