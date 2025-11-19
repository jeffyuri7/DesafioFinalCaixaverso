using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;
using DesafioFinalCaixaverso.Communications.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.API.Controllers;

[ApiController]
[Route("v1/perfil-risco")]
public class PerfilRiscoController : ControllerBase
{
    [HttpGet("{clienteId:guid}")]
    [SwaggerOperation(
        Summary = "Consulta o perfil de risco calculado para o cliente.",
        Description = "Analisa o histórico de simulações e retorna a pontuação consolidada do investidor.",
        Tags = new[] { "Perfil de Risco" })]
    [ProducesResponseType(typeof(PerfilClienteJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarPerfil(
        Guid clienteId,
    [FromServices] ICasoDeUsoConsultarPerfilCliente casoDeUsoConsultarPerfilCliente,
    CancellationToken cancellationToken = default)
    {
        var perfil = await casoDeUsoConsultarPerfilCliente.Executar(clienteId, cancellationToken);

        return Ok(perfil);
    }
}
