namespace DesafioFinalCaixaverso.Communications.Responses;

public class SimulacoesPorProdutoDiaJson
{
    public string Produto { get; set; } = string.Empty;
    public DateTime Dia { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotalInvestido { get; set; }
}
