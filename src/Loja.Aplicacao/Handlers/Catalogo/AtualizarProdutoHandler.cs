using FluentValidation;
using Loja.Aplicacao.Handlers.Catalogo.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loja.Aplicacao.Handlers.Catalogo
{
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

    public record AtualizarProdutoCommand(Guid Id, string Nome, decimal Preco, int Estoque) : IRequest<bool>;

    public sealed class AtualizarProdutoValidator : AbstractValidator<AtualizarProdutoCommand>
    {
        public AtualizarProdutoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Nome).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Preco).GreaterThan(0);
            RuleFor(x => x.Estoque).GreaterThanOrEqualTo(0);
        }
    }
}
