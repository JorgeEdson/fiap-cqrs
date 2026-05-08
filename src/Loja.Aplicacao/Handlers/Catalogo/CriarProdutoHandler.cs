using FluentValidation;
using Loja.Aplicacao.Handlers.Catalogo.Interfaces;
using Loja.Dominio.Catalogo;
using MediatR;

namespace Loja.Aplicacao.Handlers.Catalogo
{
    public sealed class CriarProdutoHandler(IProdutoStoreEscrita store) : IRequestHandler<CriarProdutoCommand, Guid>
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

    public record CriarProdutoCommand(string Nome, decimal Preco, int Estoque) : IRequest<Guid>;

    public sealed class CriarProdutoValidator : AbstractValidator<CriarProdutoCommand>
    {
        public CriarProdutoValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Preco).GreaterThan(0);
            RuleFor(x => x.Estoque).GreaterThanOrEqualTo(0);
        }
    }
}
