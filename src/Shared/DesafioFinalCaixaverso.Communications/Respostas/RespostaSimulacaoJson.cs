using System;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class RespostaSimulacaoJson
{
    public ProdutoSimuladoJson ProdutoValidado { get; set; } = new();
    public ResultadoSimulacaoJson ResultadoSimulacao { get; set; } = new();
    public DateTime DataSimulacao { get; set; }
}
