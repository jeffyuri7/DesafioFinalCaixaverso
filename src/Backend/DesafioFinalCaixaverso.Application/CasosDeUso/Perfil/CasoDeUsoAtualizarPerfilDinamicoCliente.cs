using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;

public class CasoDeUsoAtualizarPerfilDinamicoCliente : ICasoDeUsoAtualizarPerfilDinamicoCliente
{
    private readonly ISimulacaoRepositorio _simulacaoRepositorio;
    private readonly IClientePerfilDinamicoRepositorio _perfilDinamicoRepositorio;
    private readonly ICalculadoraPerfilDinamico _calculadoraPerfilDinamico;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public CasoDeUsoAtualizarPerfilDinamicoCliente(
        ISimulacaoRepositorio simulacaoRepositorio,
        IClientePerfilDinamicoRepositorio perfilDinamicoRepositorio,
        ICalculadoraPerfilDinamico calculadoraPerfilDinamico,
        IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _simulacaoRepositorio = simulacaoRepositorio;
        _perfilDinamicoRepositorio = perfilDinamicoRepositorio;
        _calculadoraPerfilDinamico = calculadoraPerfilDinamico;
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public async Task Atualizar(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var simulacoes = await _simulacaoRepositorio.ListarPorClienteAsync(clienteId, cancellationToken);
        var perfilCalculado = _calculadoraPerfilDinamico.Calcular(clienteId, simulacoes);
        if (perfilCalculado.FrequenciaMovimentacoes == 0 && perfilCalculado.VolumeTotalInvestido == 0m)
            return;
        var perfilExistente = await _perfilDinamicoRepositorio.ObterPorClienteAsync(clienteId, cancellationToken);

        if (perfilExistente is null)
            await _perfilDinamicoRepositorio.AdicionarAsync(perfilCalculado, cancellationToken);
        else
        {
            perfilCalculado.Id = perfilExistente.Id;
            await _perfilDinamicoRepositorio.AtualizarAsync(perfilCalculado, cancellationToken);
        }

        await _unidadeDeTrabalho.Commit();
    }
}
