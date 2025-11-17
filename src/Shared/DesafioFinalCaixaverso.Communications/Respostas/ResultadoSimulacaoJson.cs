using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class ResultadoSimulacaoJson
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Guid ProdutoId { get; set; }
    public decimal ValorInvestido { get; set; }
    public decimal ValorFinal { get; set; }
    public decimal RentabilidadeEfetiva { get; set; }
    public int PrazoMeses { get; set; }
}
