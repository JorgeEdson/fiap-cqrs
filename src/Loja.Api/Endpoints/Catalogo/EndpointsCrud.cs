using Loja.Dominio.Catalogo;
using Loja.Infraestrutura.Persistencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loja.Api.Endpoints.Catalogo;

/// <summary>
/// AULA 1 — Endpoints CRUD legados (o "antes").
///
/// Mantemos como referência para CONTRASTAR com a versão CQRS:
/// observe a entidade anêmica circulando como request/response,
/// um único service obeso (substituído aqui pelo DbContext direto)
/// e a ausência de semântica entre leitura e escrita.
/// </summary>
public static class EndpointsCrud
{
    public static IEndpointRouteBuilder MapearCatalogoCrud(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/legacy/produtos").WithTags("Aula 1 · CRUD Legado");

        g.MapGet("/", async (AppDbContext db) =>
            Results.Ok(await db.Produtos.AsNoTracking().ToListAsync()));

        g.MapGet("/{id:guid}", async (Guid id, AppDbContext db) =>
            await db.Produtos.FindAsync(id) is { } p ? Results.Ok(p) : Results.NotFound());

        g.MapPost("/", async ([FromBody] Produto entrada, AppDbContext db) =>
        {
            entrada.Id = Guid.NewGuid();
            db.Produtos.Add(entrada);
            await db.SaveChangesAsync();
            return Results.Created($"/api/legacy/produtos/{entrada.Id}", entrada);
        });

        g.MapPut("/{id:guid}", async (Guid id, [FromBody] Produto entrada, AppDbContext db) =>
        {
            var existente = await db.Produtos.FindAsync(id);
            if (existente is null) return Results.NotFound();
            existente.Nome = entrada.Nome;
            existente.Preco = entrada.Preco;
            existente.Estoque = entrada.Estoque;
            existente.Descontinuado = entrada.Descontinuado;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        g.MapDelete("/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var existente = await db.Produtos.FindAsync(id);
            if (existente is null) return Results.NotFound();
            db.Produtos.Remove(existente);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return app;
    }
}
