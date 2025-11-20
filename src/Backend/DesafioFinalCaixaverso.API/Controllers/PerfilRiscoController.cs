using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;
using DesafioFinalCaixaverso.Communications.Responses;
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
        Summary = "Consulta o perfil de risco resumido do cliente.",
        Description = "Retorna apenas as informações essenciais para exibir o perfil atual do investidor.",
        Tags = new[] { "Perfil de Risco" })]
    [ProducesResponseType(typeof(PerfilClienteResumoJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarPerfilResumido(
        Guid clienteId,
        [FromServices] ICasoDeUsoConsultarPerfilCliente casoDeUsoConsultarPerfilCliente,
        CancellationToken cancellationToken = default)
    {
        var perfil = await casoDeUsoConsultarPerfilCliente.Executar(clienteId, cancellationToken);

        var resumo = new PerfilClienteResumoJson
        {
            ClienteId = perfil.ClienteId,
            Perfil = perfil.Perfil,
            Pontuacao = perfil.Pontuacao,
            Descricao = DescreverPerfil(perfil.Perfil)
        };

        return Ok(resumo);
    }

    [HttpGet("perfil-risco-completo/{clienteId:guid}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Consulta o perfil de risco calculado para o cliente.",
        Description = "Analisa o histórico de simulações e retorna a pontuação consolidada do investidor.",
        Tags = new[] { "Perfil de Risco" })]
    [ProducesResponseType(typeof(PerfilClienteJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarPerfilCompleto(
        Guid clienteId,
        [FromServices] ICasoDeUsoConsultarPerfilCliente casoDeUsoConsultarPerfilCliente,
        CancellationToken cancellationToken = default)
    {
        var perfil = await casoDeUsoConsultarPerfilCliente.Executar(clienteId, cancellationToken);

        return Ok(perfil);
    }

    private static string DescreverPerfil(string perfil)
    {
        if (string.IsNullOrWhiteSpace(perfil))
            return "Perfil ainda não classificado.";

        return perfil.ToLowerInvariant() switch
        {
            "conservador" => "Perfil voltado para preservar o capital e garantir liquidez.",
            "moderado" => "Perfil equilibrado entre segurança e rentabilidade.",
            "agressivo" => "Perfil que aceita maior volatilidade em busca de retorno.",
            _ => "Perfil ainda não classificado."
        };
    }
}
