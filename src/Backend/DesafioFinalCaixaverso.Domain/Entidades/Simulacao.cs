namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class Simulacao
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ProdutoId { get; set; }
    public decimal ValorInvestido { get; set; }
    public decimal ValorFinal { get; set; }
    public decimal RentabilidadeEfetiva { get; set; }
    public int PrazoMeses { get; set; }
    public DateTime DataSimulacao { get; set; } = DateTime.UtcNow;
}
