namespace DesafioFinalCaixaverso.Communications.Requests;

public class RequisicaoSimulacaoJson
{
    public int ClienteId { get; set; }
    public decimal Valor { get; set; }
    public int PrazoMeses { get; set; }
    public string TipoProduto { get; set; } = string.Empty;
}
