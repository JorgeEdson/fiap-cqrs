namespace Loja.Dominio.Compartilhado;

public sealed class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem) { }
}
