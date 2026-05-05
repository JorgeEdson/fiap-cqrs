namespace Loja.Dominio.Comum;

public sealed class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem) { }
}
