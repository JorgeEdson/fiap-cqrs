using Loja.Api.Endpoints.Catalogo;
using Loja.Api.Endpoints.Checkout;
using Loja.Api.Endpoints.Pedidos;
using Loja.Dominio.Compartilhado;
using Loja.Infraestrutura;
using Loja.Infraestrutura.Persistencia;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------
// AULA 7 — Logging estruturado com Serilog (correlation-id friendly).
// --------------------------------------------------------------------
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new() { Title = "Loja CQRS+ES (FIAP)", Version = "v1" });
});

// Toda a infra (EF + Marten + Wolverine + MediatR) está em Loja.Infraestrutura.
builder.Services.AdicionarInfraestrutura(builder.Configuration);
builder.Host.AdicionarWolverine();

// --------------------------------------------------------------------
// AULA 7 — Tradução padronizada das RegraNegocioException → 422.
// --------------------------------------------------------------------
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(handler =>
{
    handler.Run(async ctx =>
    {
        var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (feature?.Error is RegraNegocioException brx)
        {
            ctx.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await ctx.Response.WriteAsJsonAsync(new { erro = "violacao_regra_negocio", mensagem = brx.Message });
            return;
        }
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsJsonAsync(new { erro = "nao_tratado", mensagem = feature?.Error.Message });
    });
});


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// --------------------------------------------------------------------
//  ENDPOINTS — agrupados por aula, comentados em cada arquivo.
// --------------------------------------------------------------------
app.MapearCatalogoCrud();   // AULA 1 — antes
app.MapearCatalogoCqrs();   // AULA 1 — depois
app.MapearPedidos();        // AULAS 2-5
app.MapearCheckout();       // AULA 6

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

public partial class Program { }
