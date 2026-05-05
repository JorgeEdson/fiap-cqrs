using Loja.Dominio.Comum;
using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;

namespace Loja.Dominio.Pedidos;

public sealed class Pedido : RaizAgregada
{
    private readonly List<ItemPedido> _itens = new();

    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public string? EmailCliente { get; private set; }
    public StatusPedido Status { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public IReadOnlyList<ItemPedido> Itens => _itens.AsReadOnly();
    public decimal Total => _itens.Sum(i => i.Subtotal);

    
    public Pedido() { }

    
    public static Pedido Criar(Guid pedidoId, Guid clienteId, string? emailCliente = null)
    {
        if (pedidoId == Guid.Empty) throw new RegraNegocioException("PedidoId obrigatório.");
        if (clienteId == Guid.Empty) throw new RegraNegocioException("ClienteId obrigatório.");

        var pedido = new Pedido();

        if (string.IsNullOrWhiteSpace(emailCliente))
        {
            pedido.Emitir(new PedidoCriado(pedidoId, clienteId, DateTimeOffset.UtcNow));
        }
        else
        {
            
            pedido.Emitir(new PedidoCriadoV2(pedidoId, clienteId, emailCliente, DateTimeOffset.UtcNow));
        }

        return pedido;
    }

    public void AdicionarItem(Guid produtoId, string nomeProduto, decimal precoUnitario, int quantidade)
    {
        GarantirStatus(StatusPedido.Rascunho, "Só é possível adicionar itens em pedidos em rascunho.");
        if (quantidade <= 0) throw new RegraNegocioException("Quantidade deve ser positiva.");
        if (precoUnitario <= 0) throw new RegraNegocioException("Preço unitário deve ser positivo.");

        Emitir(new ItemPedidoAdicionado(Id, produtoId, nomeProduto, precoUnitario, quantidade));
    }

    public void RemoverItem(Guid produtoId, int quantidade)
    {
        GarantirStatus(StatusPedido.Rascunho, "Só é possível remover itens em pedidos em rascunho.");
        var existente = _itens.FirstOrDefault(i => i.ProdutoId == produtoId)
            ?? throw new RegraNegocioException("Item não encontrado no pedido.");
        if (quantidade <= 0 || quantidade > existente.Quantidade)
            throw new RegraNegocioException("Quantidade inválida para remoção.");

        Emitir(new ItemPedidoRemovido(Id, produtoId, quantidade));
    }

    public void Confirmar()
    {
        GarantirStatus(StatusPedido.Rascunho, "Apenas pedidos em rascunho podem ser confirmados.");
        if (_itens.Count == 0)
            throw new RegraNegocioException("Não é possível confirmar pedido vazio.");

        Emitir(new PedidoConfirmado(Id, Total, DateTimeOffset.UtcNow));
    }

    public void Pagar(string meioPagamento, string transacaoId)
    {
        GarantirStatus(StatusPedido.Confirmado, "Apenas pedidos confirmados podem ser pagos.");
        if (string.IsNullOrWhiteSpace(transacaoId))
            throw new RegraNegocioException("TransacaoId obrigatório.");

        Emitir(new PedidoPago(Id, meioPagamento, transacaoId, DateTimeOffset.UtcNow));
    }

    public void Enviar(string codigoRastreio, string transportadora)
    {
        GarantirStatus(StatusPedido.Pago, "Apenas pedidos pagos podem ser enviados.");
        if (string.IsNullOrWhiteSpace(codigoRastreio))
            throw new RegraNegocioException("Código de rastreio obrigatório.");

        Emitir(new PedidoEnviado(Id, codigoRastreio, transportadora, DateTimeOffset.UtcNow));
    }

    public void Cancelar(string motivo)
    {
        if (Status is StatusPedido.Enviado or StatusPedido.Cancelado)
            throw new RegraNegocioException($"Não é possível cancelar um pedido em status {Status}.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new RegraNegocioException("Motivo de cancelamento obrigatório.");

        Emitir(new PedidoCancelado(Id, motivo, DateTimeOffset.UtcNow));
    }    

    protected override void Aplicar(IEventoDominio evento)
    {
        switch (evento)
        {
            case PedidoCriado e:
                Id = e.PedidoId;
                ClienteId = e.ClienteId;
                CriadoEm = e.CriadoEm;
                Status = StatusPedido.Rascunho;
                break;

            case PedidoCriadoV2 e:
                Id = e.PedidoId;
                ClienteId = e.ClienteId;
                EmailCliente = e.EmailCliente;
                CriadoEm = e.CriadoEm;
                Status = StatusPedido.Rascunho;
                break;

            case ItemPedidoAdicionado e:
                var existente = _itens.FirstOrDefault(i => i.ProdutoId == e.ProdutoId);
                if (existente is null)
                    _itens.Add(new ItemPedido(e.ProdutoId, e.NomeProduto, e.PrecoUnitario, e.Quantidade));
                else
                {
                    _itens.Remove(existente);
                    _itens.Add(existente with { Quantidade = existente.Quantidade + e.Quantidade });
                }
                break;

            case ItemPedidoRemovido e:
                var item = _itens.First(i => i.ProdutoId == e.ProdutoId);
                _itens.Remove(item);
                if (item.Quantidade > e.Quantidade)
                    _itens.Add(item with { Quantidade = item.Quantidade - e.Quantidade });
                break;

            case PedidoConfirmado:
                Status = StatusPedido.Confirmado;
                break;

            case PedidoPago:
                Status = StatusPedido.Pago;
                break;

            case PedidoEnviado:
                Status = StatusPedido.Enviado;
                break;

            case PedidoCancelado:
                Status = StatusPedido.Cancelado;
                break;
        }
    }

    private void GarantirStatus(StatusPedido esperado, string mensagem)
    {
        if (Status != esperado) throw new RegraNegocioException(mensagem);
    }
}
