namespace DesafioFinalCaixaverso.Communications.Responses;

public class TelemetriaServicosJson
{
    public string Servico { get; set; } = string.Empty;
    public int QuantidadeChamadas { get; set; }
    public DateTime UltimaChamada { get; set; }
}
