using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Mapster;
using SimulacaoDominio = DesafioFinalCaixaverso.Dominio.Entidades.Simulacao;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class CasoDeUsoResponderQuestionarioCliente : ICasoDeUsoResponderQuestionarioCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IQuestionarioInvestidorRepositorio _questionarioRepositorio;
    private readonly IClientePerfilDinamicoRepositorio _clientePerfilDinamicoRepositorio;
    private readonly IClientePerfilRepositorio _clientePerfilRepositorio;
    private readonly ICalculadoraPerfilInvestidor _calculadoraPerfilInvestidor;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public CasoDeUsoResponderQuestionarioCliente(
        IClienteRepositorio clienteRepositorio,
        IQuestionarioInvestidorRepositorio questionarioRepositorio,
        IClientePerfilDinamicoRepositorio clientePerfilDinamicoRepositorio,
        IClientePerfilRepositorio clientePerfilRepositorio,
        ICalculadoraPerfilInvestidor calculadoraPerfilInvestidor,
        IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _clienteRepositorio = clienteRepositorio;
        _questionarioRepositorio = questionarioRepositorio;
        _clientePerfilDinamicoRepositorio = clientePerfilDinamicoRepositorio;
        _clientePerfilRepositorio = clientePerfilRepositorio;
        _calculadoraPerfilInvestidor = calculadoraPerfilInvestidor;
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

    await SincronizarPerfilQuestionario(clienteId, questionario, cancellationToken);
    await SincronizarPerfilDinamicoInicial(clienteId, cancellationToken);
        await _unidadeDeTrabalho.Commit();

        return questionario.Adapt<QuestionarioRespondidoJson>();
    }

    private async Task SincronizarPerfilDinamicoInicial(Guid clienteId, CancellationToken cancellationToken)
    {
        var perfilExistente = await _clientePerfilDinamicoRepositorio.ObterPorClienteAsync(clienteId, cancellationToken);
        if (perfilExistente is not null)
        {
            perfilExistente.AtualizadoEm = DateTime.UtcNow;
            await _clientePerfilDinamicoRepositorio.AtualizarAsync(perfilExistente, cancellationToken);
            return;
        }

        var perfilInicial = new ClientePerfilDinamico
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Perfil = PerfilInvestidor.NaoClassificado,
            Pontuacao = 0,
            VolumeTotalInvestido = 0m,
            FrequenciaMovimentacoes = 0,
            PreferenciaLiquidez = true,
            AtualizadoEm = DateTime.UtcNow
        };

        await _clientePerfilDinamicoRepositorio.AdicionarAsync(perfilInicial, cancellationToken);
    }

    private async Task SincronizarPerfilQuestionario(Guid clienteId, QuestionarioInvestidor questionario, CancellationToken cancellationToken)
    {
    var resultado = _calculadoraPerfilInvestidor.Calcular(clienteId, Array.Empty<SimulacaoDominio>(), questionario);
        var perfilExistente = await _clientePerfilRepositorio.ObterPorClienteAsync(clienteId, cancellationToken);

        if (perfilExistente is null)
        {
            var entidade = resultado.Adapt<ClientePerfil>();
            entidade.Id = Guid.NewGuid();
            await _clientePerfilRepositorio.AdicionarAsync(entidade, cancellationToken);
            return;
        }

        resultado.Adapt(perfilExistente);
        await _clientePerfilRepositorio.AtualizarAsync(perfilExistente, cancellationToken);
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
