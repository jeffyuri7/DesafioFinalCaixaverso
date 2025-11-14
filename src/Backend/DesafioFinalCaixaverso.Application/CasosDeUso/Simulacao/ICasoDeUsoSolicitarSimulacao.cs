using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public interface ICasoDeUsoSolicitarSimulacao
{
    public Task<RespostaSimulacaoJson> Executar(RequisicaoSimulacaoJson request);
}
