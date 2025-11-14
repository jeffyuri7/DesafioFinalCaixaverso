using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using Mapster;
using System.Net.Http.Headers;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public class CasoDeUsoSolicitarSimulacao : ICasoDeUsoSolicitarSimulacao
{
    // Implementar os repositórios
    async Task<RespostaSimulacaoJson> ICasoDeUsoSolicitarSimulacao.Executar(RequisicaoSimulacaoJson request)
    {
        await Validate(request);

        var simulacao = request.Adapt<Dominio.Entidades.Simulacao>();

        // await _repositorioSimulacao.Adicionar(simulacao);

        // await _unidadeDeTrabalho.Commit();

        return simulacao.Adapt<RespostaSimulacaoJson>();
    }

    private async Task Validate(RequisicaoSimulacaoJson request)
    {
        var validador = new ValidadorSimulacao();

        var resultado = await validador.ValidateAsync(request);

        if (!resultado.IsValid)
        {
            throw new ApplicationException(string.Join("; ", resultado.Errors.Select(e => e.ErrorMessage)));
        }
    }
}
