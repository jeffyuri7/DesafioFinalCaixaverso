using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.Mapeamentos;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public class CasoDeUsoConsultarInvestimentosCliente : ICasoDeUsoConsultarInvestimentosCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly ISimulacaoRepositorio _simulacaoRepositorio;

    public CasoDeUsoConsultarInvestimentosCliente(
        IClienteRepositorio clienteRepositorio,
        ISimulacaoRepositorio simulacaoRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
        _simulacaoRepositorio = simulacaoRepositorio;
        MapsterConfiguracoes.Registrar();
    }

    public async Task<IReadOnlyCollection<InvestimentoClienteJson>> Executar(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);

        if (cliente is null)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        var simulacoes = await _simulacaoRepositorio.ListarPorClienteAsync(clienteId, cancellationToken);

        return simulacoes.Adapt<IReadOnlyCollection<InvestimentoClienteJson>>();
    }
}
