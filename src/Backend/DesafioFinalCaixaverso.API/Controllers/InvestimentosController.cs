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

    [HttpGet("simulacoes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarHistorico(
        [FromServices] ICasoDeUsoConsultarHistoricoSimulacoes casoDeUsoConsultarHistoricoSimulacoes)
    {
        var historico = await casoDeUsoConsultarHistoricoSimulacoes.Executar();
        return Ok(historico);
    }

    [HttpGet("simulacoes/por-produto-dia")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarSimulacoesPorProdutoDia(
        [FromServices] ICasoDeUsoConsultarSimulacoesPorProdutoDia casoDeUsoConsultarSimulacoesPorProdutoDia)
    {
        var agrupado = await casoDeUsoConsultarSimulacoesPorProdutoDia.Executar();
        return Ok(agrupado);
    }
}
