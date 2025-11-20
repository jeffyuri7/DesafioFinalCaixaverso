using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.API.Controllers;

public class InvestimentosController : ControllerBaseV1
{
    [HttpPost("simular-investimento")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Solicita uma nova simulação.",
        Description = "Valida o cliente, localiza todos os produtos compatíveis e retorna uma simulação para cada um deles.",
        Tags = new[] { "Investimentos" })]
    [ProducesResponseType(typeof(RespostaSimulacaoJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SolicitarSimulacao(
        [FromServices] ICasoDeUsoSolicitarSimulacao casoDeUsoSolicitarSimulacao,
        [FromBody] RequisicaoSimulacaoJson requisicao)
    {
        var resposta = await casoDeUsoSolicitarSimulacao.Executar(requisicao);

        return CreatedAtAction(
            nameof(ListarInvestimentosPorCliente),
            new { clienteId = requisicao.ClienteId },
            resposta);
    }

    [HttpGet("simulacoes")]
    [SwaggerOperation(
        Summary = "Lista o histórico completo de simulações.",
        Description = "Retorna todas as simulações ordenadas pela data mais recente primeiro.",
        Tags = new[] { "Investimentos" })]
    [ProducesResponseType(typeof(IEnumerable<HistoricoSimulacaoJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarHistorico(
        [FromServices] ICasoDeUsoConsultarHistoricoSimulacoes casoDeUsoConsultarHistoricoSimulacoes)
    {
        var historico = await casoDeUsoConsultarHistoricoSimulacoes.Executar();
        return Ok(historico);
    }

    [HttpGet("simulacoes/por-produto-dia")]
    [SwaggerOperation(
        Summary = "Agrupa simulações por produto e dia.",
        Description = "Fornece indicadores de volume diário por produto para dashboards.",
        Tags = new[] { "Investimentos" })]
    [ProducesResponseType(typeof(IEnumerable<SimulacoesPorProdutoDiaJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarSimulacoesPorProdutoDia(
        [FromServices] ICasoDeUsoConsultarSimulacoesPorProdutoDia casoDeUsoConsultarSimulacoesPorProdutoDia)
    {
        var agrupado = await casoDeUsoConsultarSimulacoesPorProdutoDia.Executar();
        return Ok(agrupado);
    }

    [HttpGet("{clienteId:guid}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Consulta as simulações do cliente.",
        Description = "Filtra o histórico retornando apenas os investimentos realizados pelo cliente informado.",
        Tags = new[] { "Investimentos" })]
    [ProducesResponseType(typeof(IEnumerable<HistoricoSimulacaoJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarInvestimentosPorCliente(
        Guid clienteId,
        [FromServices] ICasoDeUsoConsultarInvestimentosCliente casoDeUsoConsultarInvestimentosCliente,
        CancellationToken cancellationToken = default)
    {
        var investimentos = await casoDeUsoConsultarInvestimentosCliente.Executar(clienteId, cancellationToken);
        return Ok(investimentos);
    }
}
