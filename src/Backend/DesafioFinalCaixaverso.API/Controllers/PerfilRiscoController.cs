using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.API.Controllers;

[ApiController]
[Route("v1")]
public class PerfilRiscoController : ControllerBase
{
    [HttpGet("perfil-risco/{clienteId:guid}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Consulta o perfil de risco dinâmico do cliente.",
        Description = "Retorna o perfil calculado dinamicamente com base no comportamento do investidor (simulações/investimentos). Se não houver dados suficientes, devolve o perfil inicial registrado.",
        Tags = new[] { "Perfil de Risco" })]
    [ProducesResponseType(typeof(PerfilClienteResumoJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarPerfilDinamico(
        Guid clienteId,
        [FromServices] ICasoDeUsoConsultarPerfilDinamicoCliente casoDeUsoConsultarPerfilDinamicoCliente,
        [FromServices] ICasoDeUsoAtualizarPerfilDinamicoCliente casoDeUsoAtualizarPerfilDinamicoCliente,
        CancellationToken cancellationToken = default)
    {
        await casoDeUsoAtualizarPerfilDinamicoCliente.Atualizar(clienteId, cancellationToken);
        var perfil = await casoDeUsoConsultarPerfilDinamicoCliente.Executar(clienteId, cancellationToken);

        if (string.IsNullOrWhiteSpace(perfil.Descricao))
            perfil.Descricao = GerarDescricao(perfil.Perfil);

        return Ok(perfil);
    }

    [HttpGet("perfil-risco-inicial/{clienteId:guid}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Consulta o perfil de risco calculado para o cliente via questionário - ANBIMA Código de Distribuição, Art. 12",
        Description = "Retorna o perfil de risco inicial do cliente, calculado exclusivamente com base nas respostas do questionário, conforme exigido pelo Art. 12 do Código de Distribuição da ANBIMA (Capítulo VI – Suitability).",
        Tags = new[] { "Perfil de Risco" })]
    [ProducesResponseType(typeof(PerfilClienteJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarPerfilInicial(
        Guid clienteId,
        [FromServices] ICasoDeUsoConsultarPerfilInicialCliente casoDeUsoConsultarPerfilInicialCliente,
        CancellationToken cancellationToken = default)
    {
        var perfil = await casoDeUsoConsultarPerfilInicialCliente.Executar(clienteId, cancellationToken);
        return Ok(perfil);
    }

    private static string GerarDescricao(string perfil, string? observacoes = null)
    {
        if (!string.IsNullOrWhiteSpace(observacoes))
            return observacoes;

        if (!Enum.TryParse(perfil, true, out PerfilInvestidor perfilEnum))
            return "Perfil em análise.";

        return perfilEnum switch
        {
            PerfilInvestidor.Conservador => "Focado em capital e liquidez, tolera oscilações mínimas e prioriza previsibilidade.",
            PerfilInvestidor.Moderado => "Equilibra segurança e rentabilidade, aceitando riscos moderados para alcançar ganhos consistentes.",
            PerfilInvestidor.Agressivo => "Busca retornos elevados, assume volatilidade e prazos longos para maximizar resultados.",
            _ => "Sem dados suficientes para classificar o investidor."
        };
    }
}
