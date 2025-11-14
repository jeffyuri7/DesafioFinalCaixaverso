using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.Communications.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DesafioFinalCaixaverso.API.Controllers;

public class InvestimentosController : ControllerBaseV1
{
    [HttpPost("simular-investimento")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SolicitarSimulacao(
        [FromServices] ICasoDeUsoSolicitarSimulacao useCase,
        [FromBody] RequisicaoSimulacaoJson request)
    {
        var response = await useCase.Executar(request);

        return Created(string.Empty, response);
    }
}
