using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class SimulacaoGeradaJson
{
    public ProdutoSimuladoJson Produto { get; set; } = new();
    public ResultadoSimulacaoJson Resultado { get; set; } = new();
    public DateTime DataSimulacao { get; set; }
}
