namespace DesafioFinalCaixaverso.Dominio.Entidades;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Rentabilidade { get; set; }
    public enum Risco
    {
        Baixo,
        Medio,
        Alto
    }
}
