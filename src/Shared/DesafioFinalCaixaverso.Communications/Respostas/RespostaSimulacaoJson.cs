using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class RespostaSimulacaoJson
{
    public Produto ProdutoValidado { get; set; } = new Produto();
    public Simulacao ResultadoSimulacao { get; set; } = new Simulacao();
    public DateTime DataSimulacao { get; set; }
}
