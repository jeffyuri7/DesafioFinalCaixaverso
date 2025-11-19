using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class CasoDeUsoResponderQuestionarioCliente : ICasoDeUsoResponderQuestionarioCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IQuestionarioInvestidorRepositorio _questionarioRepositorio;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public CasoDeUsoResponderQuestionarioCliente(
        IClienteRepositorio clienteRepositorio,
        IQuestionarioInvestidorRepositorio questionarioRepositorio,
        IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _clienteRepositorio = clienteRepositorio;
        _questionarioRepositorio = questionarioRepositorio;
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public async Task<QuestionarioRespondidoJson> Executar(Guid clienteId, RequisicaoQuestionarioClienteJson requisicao, CancellationToken cancellationToken = default)
    {
        await Validar(requisicao);

        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);
        if (cliente is null)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        var questionarioExistente = await _questionarioRepositorio.ObterPorClienteAsync(clienteId, cancellationToken);
        QuestionarioInvestidor questionario;

        if (questionarioExistente is null)
        {
            questionario = requisicao.Adapt<QuestionarioInvestidor>();
            questionario.Id = Guid.NewGuid();
            questionario.ClienteId = clienteId;
            questionario.AtualizadoEm = DateTime.UtcNow;
            await _questionarioRepositorio.AdicionarAsync(questionario, cancellationToken);
        }
        else
        {
            requisicao.Adapt(questionarioExistente);
            questionarioExistente.AtualizadoEm = DateTime.UtcNow;
            questionario = questionarioExistente;
            await _questionarioRepositorio.AtualizarAsync(questionario, cancellationToken);
        }

        await _unidadeDeTrabalho.Commit();

        return questionario.Adapt<QuestionarioRespondidoJson>();
    }

    private static async Task Validar(RequisicaoQuestionarioClienteJson requisicao)
    {
        var validador = new ValidadorQuestionarioCliente();
        var resultado = await validador.ValidateAsync(requisicao);

        if (!resultado.IsValid)
        {
            var erros = resultado.Errors.Select(erro => erro.ErrorMessage).ToList();
            throw new ErroValidacaoException(erros);
        }
    }
}
