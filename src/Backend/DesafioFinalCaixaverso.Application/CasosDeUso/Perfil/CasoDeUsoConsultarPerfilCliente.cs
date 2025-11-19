using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;

public class CasoDeUsoConsultarPerfilCliente : ICasoDeUsoConsultarPerfilCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly ISimulacaoRepositorio _simulacaoRepositorio;
    private readonly IClientePerfilRepositorio _clientePerfilRepositorio;
    private readonly IQuestionarioInvestidorRepositorio _questionarioRepositorio;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
    private readonly ICalculadoraPerfilInvestidor _calculadoraPerfilInvestidor;

    public CasoDeUsoConsultarPerfilCliente(
        IClienteRepositorio clienteRepositorio,
        ISimulacaoRepositorio simulacaoRepositorio,
        IClientePerfilRepositorio clientePerfilRepositorio,
        IQuestionarioInvestidorRepositorio questionarioRepositorio,
        IUnidadeDeTrabalho unidadeDeTrabalho,
        ICalculadoraPerfilInvestidor calculadoraPerfilInvestidor)
    {
        _clienteRepositorio = clienteRepositorio;
        _simulacaoRepositorio = simulacaoRepositorio;
        _clientePerfilRepositorio = clientePerfilRepositorio;
        _questionarioRepositorio = questionarioRepositorio;
        _unidadeDeTrabalho = unidadeDeTrabalho;
        _calculadoraPerfilInvestidor = calculadoraPerfilInvestidor;
    }

    public async Task<PerfilClienteJson> Executar(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);

        if (cliente is null)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        var simulacoes = await _simulacaoRepositorio.ListarPorClienteAsync(clienteId, cancellationToken);
        var questionario = await _questionarioRepositorio.ObterPorClienteAsync(clienteId, cancellationToken);

        var resultado = _calculadoraPerfilInvestidor.Calcular(clienteId, simulacoes, questionario);

        await PersistirPerfil(resultado, cancellationToken);

        return resultado.Adapt<PerfilClienteJson>();
    }

    private async Task PersistirPerfil(PerfilInvestidorResultado resultado, CancellationToken cancellationToken)
    {
        var perfilExistente = await _clientePerfilRepositorio.ObterPorClienteAsync(resultado.ClienteId, cancellationToken);

        if (perfilExistente is null)
        {
            await _clientePerfilRepositorio.AdicionarAsync(CriarEntidade(resultado), cancellationToken);
        }
        else
        {
            AtualizarEntidade(perfilExistente, resultado);
            await _clientePerfilRepositorio.AtualizarAsync(perfilExistente, cancellationToken);
        }

        await _unidadeDeTrabalho.Commit();
    }

    private static ClientePerfil CriarEntidade(PerfilInvestidorResultado resultado)
    {
        var entidade = resultado.Adapt<ClientePerfil>();
        entidade.Id = Guid.NewGuid();
        return entidade;
    }

    private static void AtualizarEntidade(ClientePerfil perfil, PerfilInvestidorResultado resultado)
    {
        resultado.Adapt(perfil);
    }
}
