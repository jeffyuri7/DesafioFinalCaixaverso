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

public class CasoDeUsoConsultarPerfilInicialCliente : ICasoDeUsoConsultarPerfilInicialCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IClientePerfilRepositorio _clientePerfilRepositorio;
    private readonly IQuestionarioInvestidorRepositorio _questionarioRepositorio;
    private readonly ICalculadoraPerfilInvestidor _calculadoraPerfilInvestidor;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public CasoDeUsoConsultarPerfilInicialCliente(
        IClienteRepositorio clienteRepositorio,
        IClientePerfilRepositorio clientePerfilRepositorio,
        IQuestionarioInvestidorRepositorio questionarioRepositorio,
        ICalculadoraPerfilInvestidor calculadoraPerfilInvestidor,
        IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _clienteRepositorio = clienteRepositorio;
        _clientePerfilRepositorio = clientePerfilRepositorio;
        _questionarioRepositorio = questionarioRepositorio;
        _calculadoraPerfilInvestidor = calculadoraPerfilInvestidor;
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public async Task<PerfilClienteJson> Executar(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);

        if (cliente is null)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        var perfilExistente = await _clientePerfilRepositorio.ObterPorClienteAsync(clienteId, cancellationToken);

        if (perfilExistente is not null)
            return perfilExistente.Adapt<PerfilClienteJson>();

        var questionario = await _questionarioRepositorio.ObterPorClienteAsync(clienteId, cancellationToken);
        var resultado = _calculadoraPerfilInvestidor.Calcular(clienteId, Array.Empty<Simulacao>(), questionario);
        var perfil = await PersistirPerfil(resultado, cancellationToken);

        return perfil.Adapt<PerfilClienteJson>();
    }

    private async Task<ClientePerfil> PersistirPerfil(PerfilInvestidorResultado resultado, CancellationToken cancellationToken)
    {
        var entidade = resultado.Adapt<ClientePerfil>();
        entidade.Id = Guid.NewGuid();

        await _clientePerfilRepositorio.AdicionarAsync(entidade, cancellationToken);
        await _unidadeDeTrabalho.Commit();

        return entidade;
    }
}
