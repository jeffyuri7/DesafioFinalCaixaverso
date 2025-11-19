using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class ProdutoRecomendadoJson
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Rentabilidade { get; set; }
    public string Risco { get; set; } = string.Empty;
    public int LiquidezDias { get; set; }
    public decimal MinimoInvestimento { get; set; }
    public int PrazoMinimoMeses { get; set; }
    public int PrazoMaximoMeses { get; set; }
}
