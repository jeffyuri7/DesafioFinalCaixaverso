using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.API.Controllers;

public class ClientesController : ControllerBaseV1
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra um novo cliente.",
        Description = "Cria o registro do cliente aplicando hash seguro à senha informada.",
        Tags = new[] { "Clientes" })]
    [ProducesResponseType(typeof(ClienteCadastradoJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cadastrar(
        [FromServices] ICasoDeUsoCadastrarCliente casoDeUsoCadastrarCliente,
        [FromBody] RequisicaoCadastroClienteJson requisicao,
        CancellationToken cancellationToken)
    {
        var cliente = await casoDeUsoCadastrarCliente.Executar(requisicao, cancellationToken);
        return Created(string.Empty, cliente);
    }

    [HttpPost("{clienteId:guid}/questionario")]
    [SwaggerOperation(
        Summary = "Registra ou atualiza o questionário do cliente.",
        Description = "Salva as respostas do questionário ANBIMA exigido para recomendações.",
        Tags = new[] { "Clientes" })]
    [ProducesResponseType(typeof(QuestionarioRespondidoJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResponderQuestionario(
        Guid clienteId,
        [FromServices] ICasoDeUsoResponderQuestionarioCliente casoDeUsoResponderQuestionarioCliente,
        [FromBody] RequisicaoQuestionarioClienteJson requisicao,
        CancellationToken cancellationToken)
    {
        var resposta = await casoDeUsoResponderQuestionarioCliente.Executar(clienteId, requisicao, cancellationToken);
        return Ok(resposta);
    }
}
