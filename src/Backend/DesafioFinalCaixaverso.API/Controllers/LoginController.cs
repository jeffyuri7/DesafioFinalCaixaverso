using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Login;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.API.Controllers;

public class LoginController : ControllerBaseV1
{
    [HttpPost]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Autentica o cliente e retorna o token JWT.",
        Description = "Informe e-mail e senha cadastrados para receber o bearer token utilizado nas demais chamadas protegidas.",
        Tags = new[] { "Autenticação" })]
    [ProducesResponseType(typeof(ClienteAutenticadoJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Autenticar(
        [FromServices] ICasoDeUsoLoginCliente casoDeUsoLoginCliente,
        [FromBody] RequisicaoLoginClienteJson requisicao,
        CancellationToken cancellationToken = default)
    {
        var resposta = await casoDeUsoLoginCliente.Executar(requisicao, cancellationToken);
        return Ok(resposta);
    }
}
