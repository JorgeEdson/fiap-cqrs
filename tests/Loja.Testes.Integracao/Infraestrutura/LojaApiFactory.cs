using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Loja.Testes.Integracao.Infraestrutura;

/// <summary>
/// AULA 7 — WebApplicationFactory que aponta para o Postgres do <see cref="FixturePostgres"/>.
/// </summary>
public sealed class LojaApiFactory(string stringConexao) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = stringConexao
            });
        });
    }
}
