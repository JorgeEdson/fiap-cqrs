namespace Loja.Dominio.Comum;


public abstract class RaizAgregada
{
    private readonly List<IEventoDominio> _eventosPendentes = new();
    
    public long Versao { get; private set; }
    
    public IReadOnlyList<IEventoDominio> EventosPendentes => _eventosPendentes.AsReadOnly();
    
    public void LimparEventosPendentes() => _eventosPendentes.Clear();

   
    public void CarregarDoHistorico(IEnumerable<IEventoDominio> historico)
    {
        foreach (var evento in historico)
        {
            Mutar(evento);
            Versao++;
        }
    }
    
    protected void Emitir(IEventoDominio evento)
    {
        Mutar(evento);
        _eventosPendentes.Add(evento);
        Versao++;
    }
    
    protected abstract void Aplicar(IEventoDominio evento);

    private void Mutar(IEventoDominio evento) => Aplicar(evento);
}
