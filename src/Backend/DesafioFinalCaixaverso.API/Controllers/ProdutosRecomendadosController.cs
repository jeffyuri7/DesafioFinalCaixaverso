using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.API.Controllers;

[ApiController]
[Route("v1/produtos-recomendados")]
public class ProdutosRecomendadosController : ControllerBase
{
    [HttpGet("{perfil}")]
    [SwaggerOperation(
        Summary = "Lista produtos alinhados ao perfil informado.",
        Description = "Perfis aceitos: -1=Não classificado (sem recomendações), 0=Conservador (risco baixo), 1=Moderado (risco médio), 2=Agressivo (risco alto).",
        Tags = new[] { "Perfil de Risco" })]
    [ProducesResponseType(typeof(IEnumerable<ProdutoRecomendadoJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorPerfil(
        PerfilInvestidor perfil,
        [FromServices] ICasoDeUsoListarProdutosRecomendados casoDeUsoListarProdutosRecomendados,
        CancellationToken cancellationToken = default)
    {
        var produtos = await casoDeUsoListarProdutosRecomendados.Executar(perfil, cancellationToken);

        return Ok(produtos);
    }
}
