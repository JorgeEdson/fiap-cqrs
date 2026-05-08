using MediatR;

namespace Loja.Aplicacao.Handlers.Catalogo.Consultas;







public record ProdutoModeloLeitura(
    Guid Id,
    string Nome,
    decimal Preco,
    int Estoque,
    bool Descontinuado);
