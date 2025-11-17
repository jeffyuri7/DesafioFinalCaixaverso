namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Rentabilidade { get; set; }
    public Enumeradores.Risco Risco { get; set; }
    public int LiquidezDias { get; set; }
    public decimal MinimoInvestimento { get; set; }
    public int PrazoMinimoMeses { get; set; }
    public int PrazoMaximoMeses { get; set; }
    public bool Ativo { get; set; } = true;
}
