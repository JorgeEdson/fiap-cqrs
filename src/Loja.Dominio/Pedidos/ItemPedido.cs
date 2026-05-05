namespace Loja.Dominio.Pedidos;

public sealed record ItemPedido(Guid ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade)
{
    public decimal Subtotal => PrecoUnitario * Quantidade;
}
