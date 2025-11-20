using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Consultas;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using SimulacaoDominio = DesafioFinalCaixaverso.Dominio.Entidades.Simulacao;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class SimulacaoRepositorioFalso : ISimulacaoRepositorio
{
    public SimulacaoDominio? UltimaSimulacaoAdicionada { get; private set; }
    public List<SimulacaoDominio> SimulacoesAdicionadas { get; } = new();
    private IReadOnlyCollection<SimulacaoDominio> _historico = new List<SimulacaoDominio>();
    private IReadOnlyCollection<SimulacoesPorProdutoDiaResultado> _agrupado = new List<SimulacoesPorProdutoDiaResultado>();

    public Task AdicionarAsync(SimulacaoDominio simulacao)
    {
        UltimaSimulacaoAdicionada = simulacao;
        SimulacoesAdicionadas.Add(simulacao);
        return Task.CompletedTask;
    }

    public SimulacaoRepositorioFalso ComHistorico(IReadOnlyCollection<SimulacaoDominio> historico)
    {
        _historico = historico;
        return this;
    }

    public SimulacaoRepositorioFalso ComAgrupado(IReadOnlyCollection<SimulacoesPorProdutoDiaResultado> agrupado)
    {
        _agrupado = agrupado;
        return this;
    }

    public Task<IReadOnlyCollection<SimulacaoDominio>> ListarHistoricoAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_historico);

    public Task<IReadOnlyCollection<SimulacoesPorProdutoDiaResultado>> ListarAgrupadoPorProdutoEDiaAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_agrupado);

    public Task<IReadOnlyCollection<SimulacaoDominio>> ListarPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var filtrado = _historico
            .Where(simulacao => simulacao.ClienteId == clienteId)
            .OrderByDescending(simulacao => simulacao.DataSimulacao)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyCollection<SimulacaoDominio>>(filtrado);
    }
}
