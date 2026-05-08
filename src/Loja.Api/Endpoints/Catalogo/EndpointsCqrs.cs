using Loja.Aplicacao.Handlers.Catalogo;
using MediatR;

namespace Loja.Api.Endpoints.Catalogo;

/// <summary>
/// AULA 1 — Endpoints CQRS "simples": Comandos e Consultas via Mediator.
///
/// Compare com <see cref="EndpointsCrud"/>:
///  - cada rota expressa intenção (descontinuar, busca) — não só verbos HTTP;
///  - request/response usam DTOs do Application Layer, não a entidade nua;
///  - escrita e leitura passam por handlers diferentes, com responsabilidade clara.
/// </summary>
public static class EndpointsCqrs
{
    public static IEndpointRouteBuilder MapearCatalogoCqrs(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/produtos").WithTags("Aula 1 · CQRS Simples");

        // CONSULTAS
        g.MapGet("/", async (IMediator m, bool incluirDescontinuados = false) =>
            Results.Ok(await m.Send(new ListarProdutosQuery(incluirDescontinuados))));

        g.MapGet("/{id:guid}", async (Guid id, IMediator m) =>
            await m.Send(new ObterProdutoPorIdQuery(id)) is { } p ? Results.Ok(p) : Results.NotFound());

        g.MapGet("/buscar", async (IMediator m, string? termo, decimal? precoMinimo, decimal? precoMaximo) =>
            Results.Ok(await m.Send(new BuscarProdutosQuery(termo, precoMinimo, precoMaximo))));

        // COMANDOS
        g.MapPost("/", async (CriarProdutoCommand cmd, IMediator m) =>
        {
            var id = await m.Send(cmd);
            return Results.Created($"/api/produtos/{id}", new { id });
        });

        g.MapPut("/{id:guid}", async (Guid id, AtualizarProdutoCommand corpo, IMediator m) =>
            await m.Send(corpo with { Id = id }) ? Results.NoContent() : Results.NotFound());

        g.MapPost("/{id:guid}/descontinuar", async (Guid id, IMediator m) =>
            await m.Send(new DescontinuarProdutoCommand(id)) ? Results.NoContent() : Results.NotFound());

        g.MapDelete("/{id:guid}", async (Guid id, IMediator m) =>
            await m.Send(new ExcluirProdutoCommand(id)) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}
