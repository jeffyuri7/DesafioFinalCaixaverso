namespace DesafioFinalCaixaverso.Communications.Responses;

public class HistoricoSimulacaoJson
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string Produto { get; set; } = string.Empty;
    public decimal ValorInvestido { get; set; }
    public decimal ValorFinal { get; set; }
    public int PrazoMeses { get; set; }
    public DateTime DataSimulacao { get; set; }
}
