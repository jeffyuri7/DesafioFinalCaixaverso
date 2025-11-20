using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DesafioFinalCaixaverso.API.Controllers;

public class ClientesController : ControllerBaseV1
{
    [HttpGet]
    [Authorize]
    [SwaggerOperation(
        Summary = "Lista clientes cadastrados.",
        Description = "Retorna os clientes ordenados pelo nome para consultas administrativas.",
        Tags = new[] { "Clientes" })]
    [ProducesResponseType(typeof(IEnumerable<ClienteCadastradoJson>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromServices] ICasoDeUsoListarClientes casoDeUsoListarClientes,
        CancellationToken cancellationToken = default)
    {
        var clientes = await casoDeUsoListarClientes.Executar(cancellationToken);
        return Ok(clientes);
    }

    [HttpGet("{clienteId:guid}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Consulta os dados básicos do cliente.",
        Tags = new[] { "Clientes" })]
    [ProducesResponseType(typeof(ClienteCadastradoJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Detalhar(
        Guid clienteId,
        [FromServices] ICasoDeUsoConsultarCliente casoDeUsoConsultarCliente,
        CancellationToken cancellationToken = default)
    {
        var cliente = await casoDeUsoConsultarCliente.Executar(clienteId, cancellationToken);
        return Ok(cliente);
    }

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
        CancellationToken cancellationToken = default)
    {
        var cliente = await casoDeUsoCadastrarCliente.Executar(requisicao, cancellationToken);
        return Created(string.Empty, cliente);
    }

    [HttpPut("{clienteId:guid}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Atualiza os dados básicos do cliente.",
        Description = "Permite ajustar nome, e-mail e senha (opcional) do cliente.",
        Tags = new[] { "Clientes" })]
    [ProducesResponseType(typeof(ClienteCadastradoJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(
        Guid clienteId,
        [FromServices] ICasoDeUsoAtualizarCliente casoDeUsoAtualizarCliente,
        [FromBody] RequisicaoAtualizacaoClienteJson requisicao,
        CancellationToken cancellationToken = default)
    {
        var cliente = await casoDeUsoAtualizarCliente.Executar(clienteId, requisicao, cancellationToken);
        return Ok(cliente);
    }

    [HttpDelete("{clienteId:guid}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Remove o cliente e seus relacionamentos.",
        Tags = new[] { "Clientes" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(RespostaErroJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(
        Guid clienteId,
        [FromServices] ICasoDeUsoExcluirCliente casoDeUsoExcluirCliente,
        CancellationToken cancellationToken)
    {
        await casoDeUsoExcluirCliente.Executar(clienteId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{clienteId:guid}/questionario")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Registra ou atualiza o questionário do cliente.",
        Description = """
            Questionário ANBIMA obrigatório. Campos esperados:
            - preferenciaLiquidez: 0=Alta, 1=Média, 2=Baixa
            - objetivoInvestimento: 0=Preservação, 1=Renda, 2=Equilíbrio, 3=Crescimento
            - nivelConhecimento: 0=Iniciante, 1=Intermediário, 2=Avançado
            - horizonteMeses: mínimo 1 mês
            - rendaMensal/patrimonioTotal: valores em reais (decimal)
            - toleranciaPerdaPercentual: percentual permitido entre 0 e 100
            - fonteRendaEstavel: true/false indicando estabilidade de renda
            """,
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
