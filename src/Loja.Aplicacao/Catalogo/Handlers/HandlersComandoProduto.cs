using Loja.Aplicacao.Catalogo.Comandos;
using Loja.Dominio.Catalogo;
using MediatR;

namespace Loja.Aplicacao.Catalogo.Handlers;

/// <summary>
/// AULA 1 — Handlers dos Comandos do Catálogo.
///
/// Cada handler tem UMA responsabilidade clara, com SQL/operação otimizada.
/// Isso é o contraste com um service "obeso" do paradigma CRUD.
/// </summary>
public sealed class CriarProdutoHandler(IProdutoStoreEscrita store)
    : IRequestHandler<CriarProdutoCommand, Guid>
{
    public async Task<Guid> Handle(CriarProdutoCommand request, CancellationToken ct)
    {
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Preco = request.Preco,
            Estoque = request.Estoque,
            Descontinuado = false
        };
        await store.AdicionarAsync(produto, ct);
        await store.SalvarAlteracoesAsync(ct);
        return produto.Id;
    }
}

public sealed class AtualizarProdutoHandler(IProdutoStoreEscrita store)
    : IRequestHandler<AtualizarProdutoCommand, bool>
{
    public async Task<bool> Handle(AtualizarProdutoCommand request, CancellationToken ct)
    {
        var existente = await store.ObterAsync(request.Id, ct);
        if (existente is null) return false;

        existente.Nome = request.Nome;
        existente.Preco = request.Preco;
        existente.Estoque = request.Estoque;

        return await store.SalvarAlteracoesAsync(ct);
    }
}

public sealed class DescontinuarProdutoHandler(IProdutoStoreEscrita store)
    : IRequestHandler<DescontinuarProdutoCommand, bool>
{
    public async Task<bool> Handle(DescontinuarProdutoCommand request, CancellationToken ct)
    {
        var existente = await store.ObterAsync(request.Id, ct);
        if (existente is null) return false;
        existente.Descontinuado = true;
        return await store.SalvarAlteracoesAsync(ct);
    }
}

public sealed class ExcluirProdutoHandler(IProdutoStoreEscrita store)
    : IRequestHandler<ExcluirProdutoCommand, bool>
{
    public Task<bool> Handle(ExcluirProdutoCommand request, CancellationToken ct)
        => store.RemoverAsync(request.Id, ct);
}
