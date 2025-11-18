namespace DesafioFinalCaixaverso.Communications.Responses;

public class HistoricoSimulacaoJson
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public decimal ValorInvestido { get; set; }
    public decimal ValorFinal { get; set; }
    public decimal RentabilidadeEfetiva { get; set; }
    public int PrazoMeses { get; set; }
    public DateTime DataSimulacao { get; set; }
}
