namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class TelemetriaServico
{
    public Guid Id { get; set; }
    public string Servico { get; set; } = string.Empty;
    public int QuantidadeChamadas { get; set; }
    public DateTime UltimaChamada { get; set; }
}
