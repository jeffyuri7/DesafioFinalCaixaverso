using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;
using DesafioFinalCaixaverso.Communications.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.API.Controllers;

public class TelemetriaController : ControllerBaseV1
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Consulta o consumo dos serviços.",
        Description = "Retorna a quantidade de chamadas e a última execução de cada endpoint monitorado.",
        Tags = new[] { "Telemetria" })]
    [ProducesResponseType(typeof(IEnumerable<TelemetriaServicosJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Consultar([FromServices] ICasoDeUsoConsultarTelemetriaServicos casoDeUsoConsultarTelemetriaServicos)
    {
        var telemetria = await casoDeUsoConsultarTelemetriaServicos.Executar();
        return Ok(telemetria);
    }
}
