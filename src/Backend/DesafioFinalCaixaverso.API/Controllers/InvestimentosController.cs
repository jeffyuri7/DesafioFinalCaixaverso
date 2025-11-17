using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.Communications.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DesafioFinalCaixaverso.API.Controllers;

public class InvestimentosController : ControllerBaseV1
{
    [HttpPost("simular-investimento")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SolicitarSimulacao(
        [FromServices] ICasoDeUsoSolicitarSimulacao casoDeUsoSolicitarSimulacao,
        [FromBody] RequisicaoSimulacaoJson requisicao)
    {
        var resposta = await casoDeUsoSolicitarSimulacao.Executar(requisicao);

        return Created(string.Empty, resposta);
    }
}
