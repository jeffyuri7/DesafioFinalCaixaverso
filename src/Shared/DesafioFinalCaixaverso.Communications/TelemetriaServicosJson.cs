namespace DesafioFinalCaixaverso.Communications.Responses;

public class TelemetriaServicosJson
{
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeChamadas { get; set; }
    public double MediaTempoRespostaMs { get; set; }
}
