using Testcontainers.PostgreSql;
using Xunit;

namespace Loja.Testes.Integracao.Infraestrutura;

/// <summary>
/// AULA 7 — Fixture de Postgres real via Testcontainers.
///
/// Substitui InMemory database (que não suporta JSONB / Marten).
/// Garante que projeções e Event Store sejam testados em condições idênticas
/// às de produção.
/// </summary>
public sealed class FixturePostgres : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("loja_testes")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string StringConexao => Container.GetConnectionString();

    public Task InitializeAsync() => Container.StartAsync();
    public Task DisposeAsync() => Container.DisposeAsync().AsTask();
}

[CollectionDefinition("postgres")]
public sealed class ColecaoPostgres : ICollectionFixture<FixturePostgres> { }
