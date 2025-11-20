using System.Collections.Generic;

namespace DesafioFinalCaixaverso.Communications.Responses;

public class RespostaSimulacaoJson
{
    public IList<SimulacaoGeradaJson> Simulacoes { get; set; } = new List<SimulacaoGeradaJson>();
}
