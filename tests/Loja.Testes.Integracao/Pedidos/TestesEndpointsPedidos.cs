using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Loja.Aplicacao.Handlers.Pedidos.Consultas.ModelosLeitura;
using Loja.Dominio.Pedidos;
using Loja.Testes.Integracao.Infraestrutura;
using Xunit;

namespace Loja.Testes.Integracao.Pedidos;

/// <summary>
/// AULA 7 — Teste end-to-end com Testcontainers + Marten real.
///
/// Verifica que:
///  - Comandos escrevem eventos no Event Store;
///  - A projeção INLINE atualiza o read model imediatamente;
///  - Cenários de erro retornam 422 (RegraNegocioException → ProblemDetails).
/// </summary>
[Collection("postgres")]
public sealed class TestesEndpointsPedidos(FixturePostgres pg) : IAsyncLifetime
{
    private LojaApiFactory _fabrica = null!;
    private HttpClient _cliente = null!;

    public Task InitializeAsync()
    {
        _fabrica = new LojaApiFactory(pg.StringConexao);
        _cliente = _fabrica.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _fabrica.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Criar_Adicionar_Confirmar_atualiza_read_model_inline()
    {
        // CRIAR
        var resp = await _cliente.PostAsJsonAsync("/api/pedidos",
            new { ClienteId = Guid.NewGuid(), EmailCliente = "ana@loja.local" });
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var corpo = await resp.Content.ReadFromJsonAsync<RespostaId>();
        var pedidoId = corpo!.Id;

        // ADICIONAR ITEM
        await _cliente.PostAsJsonAsync($"/api/pedidos/{pedidoId}/itens",
            new { ProdutoId = Guid.NewGuid(), NomeProduto = "Notebook", PrecoUnitario = 5000m, Quantidade = 1 });

        // CONFIRMAR
        await _cliente.PostAsJsonAsync($"/api/pedidos/{pedidoId}/confirmar", new { });

        // READ MODEL inline já reflete
        var resumo = await _cliente.GetFromJsonAsync<ResumoPedido>($"/api/pedidos/{pedidoId}");
        resumo!.Status.Should().Be(StatusPedido.Confirmado);
        resumo.Total.Should().Be(5000m);
    }

    [Fact]
    public async Task Confirmar_pedido_vazio_retorna_422()
    {
        var resp = await _cliente.PostAsJsonAsync("/api/pedidos",
            new { ClienteId = Guid.NewGuid(), EmailCliente = "x@y" });
        var corpo = await resp.Content.ReadFromJsonAsync<RespostaId>();

        var resposta = await _cliente.PostAsJsonAsync($"/api/pedidos/{corpo!.Id}/confirmar", new { });
        resposta.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private sealed record RespostaId(Guid Id);
}
