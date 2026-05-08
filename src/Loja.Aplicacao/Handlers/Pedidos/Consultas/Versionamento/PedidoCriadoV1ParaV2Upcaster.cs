using Loja.Dominio.Pedidos.Eventos.V1;
using Loja.Dominio.Pedidos.Eventos.V2;

namespace Loja.Aplicacao.Handlers.Pedidos.Consultas.Versionamento;

public static class PedidoCriadoV1ParaV2Upcaster
{
   
    public static PedidoCriadoV2 Converter(PedidoCriado legado)
        => new(legado.PedidoId, legado.ClienteId, "desconhecido@legado", legado.CriadoEm);
}
