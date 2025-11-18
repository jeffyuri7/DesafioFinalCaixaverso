using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;
using Microsoft.AspNetCore.Mvc;

namespace DesafioFinalCaixaverso.API.Controllers;

public class TelemetriaController : ControllerBaseV1
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Consultar([FromServices] ICasoDeUsoConsultarTelemetriaServicos casoDeUsoConsultarTelemetriaServicos)
    {
        var telemetria = await casoDeUsoConsultarTelemetriaServicos.Executar();
        return Ok(telemetria);
    }
}
