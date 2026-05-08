using FluentValidation;
using Loja.Aplicacao.Handlers.Catalogo.Interfaces;
using Loja.Aplicacao.Handlers.Pedidos.Consultas.Projecoes;
using Loja.Aplicacao.Handlers.Pedidos.Consultas.Versionamento;
using Loja.Aplicacao.Handlers.Pedidos.Interfaces;
using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;
using Loja.Infraestrutura.Catalogo;
using Loja.Infraestrutura.Pedidos;
using Loja.Infraestrutura.Persistencia;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weasel.Core;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Marten;

namespace Loja.Infraestrutura;

public static class InjecaoDependencia
{
    
    public static IServiceCollection AdicionarInfraestrutura(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var stringConexao = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não configurada.");

        // ---------- AULA 1: EF Core para o Catálogo (CRUD/CQRS simples) ----------
        services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(stringConexao));
        services.AddScoped<IProdutoStoreEscrita, EfProdutoStoreEscrita>();
        services.AddScoped<IProdutoStoreLeitura, EfProdutoStoreLeitura>();

        // ---------- AULA 1: Mediator + Validators ----------
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(IProdutoStoreEscrita).Assembly /* Loja.Aplicacao */));
        services.AddValidatorsFromAssembly(typeof(IProdutoStoreEscrita).Assembly);

        // ---------- AULAS 3-5: Marten como Event Store ----------
        services.AddMarten(opts =>
        {
            opts.Connection(stringConexao);
            opts.DatabaseSchemaName = "loja_es";
            opts.Events.DatabaseSchemaName = "loja_es";

            // AULA 5: versionamento de eventos via upcaster
            opts.Events.Upcast<PedidoCriado, PedidoCriadoV2>(PedidoCriadoV1ParaV2Upcaster.Converter);

            // AULA 4: projeções
            opts.Projections.Add<ResumoPedidoProjection>(ProjectionLifecycle.Inline);
            opts.Projections.Add<DashboardPedidosProjection>(ProjectionLifecycle.Async);
            opts.Projections.Add<PedidosClienteProjection>(ProjectionLifecycle.Async);

            // AULA 5: snapshot do agregado como projeção inline
            opts.Projections.Add<SnapshotPedidoProjection>(ProjectionLifecycle.Inline);

            // AULA 7: serializer estável
            opts.UseSystemTextJsonForSerialization();

            // Em DEV: cria/atualiza schema automaticamente
            opts.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
        })
        .UseLightweightSessions()                       // sessões leves (didático)
        .AddAsyncDaemon(DaemonMode.Solo)                // AULA 4: ativa o async daemon
        .IntegrateWithWolverine();                      // AULA 6

        // ---------- AULAS 3-6: repositório do agregado Pedido ----------
        services.AddScoped<IPedidoRepository, MartenPedidoRepository>();

        return services;
    }

    
    public static IHostBuilder AdicionarWolverine(this IHostBuilder host)
    {
        return host.UseWolverine(opts =>
        {
            // Habilita storage durável de mensagens (inbox/outbox + sagas) via Marten.
            opts.Policies.UseDurableLocalQueues();

            // AULA 7: política de retry exponencial.
            opts.OnException<Exception>().RetryWithCooldown(
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromSeconds(2));
        });
    }
}
