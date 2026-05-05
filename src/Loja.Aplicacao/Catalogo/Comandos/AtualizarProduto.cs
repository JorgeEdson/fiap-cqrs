using FluentValidation;
using MediatR;

namespace Loja.Aplicacao.Catalogo.Comandos;

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

public record DescontinuarProdutoCommand(Guid Id) : IRequest<bool>;

public record ExcluirProdutoCommand(Guid Id) : IRequest<bool>;
