using FluentValidation;
using MediatR;

namespace Loja.Aplicacao.Catalogo.Comandos;

/// <summary>
/// Comando — instrução para mudar o estado do sistema. Discutido na AULA 1.
/// Sempre no IMPERATIVO (CriarProduto, AtualizarProduto, Descontinuar...).
/// </summary>
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
