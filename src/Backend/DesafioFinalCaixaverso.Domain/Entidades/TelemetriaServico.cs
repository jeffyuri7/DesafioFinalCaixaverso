namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class TelemetriaServico
{
    public Guid Id { get; set; }
    public string Servico { get; set; } = string.Empty;
    public int AnoReferencia { get; set; }
    public int MesReferencia { get; set; }
    public int QuantidadeChamadas { get; set; }
    public long TempoTotalRespostaMs { get; set; }
    public DateTime UltimaChamada { get; set; }
}
