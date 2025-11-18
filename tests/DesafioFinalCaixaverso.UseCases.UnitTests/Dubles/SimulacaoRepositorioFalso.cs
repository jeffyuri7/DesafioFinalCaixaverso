using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Consultas;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using SimulacaoDominio = DesafioFinalCaixaverso.Dominio.Entidades.Simulacao;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class SimulacaoRepositorioFalso : ISimulacaoRepositorio
{
    public SimulacaoDominio? UltimaSimulacaoAdicionada { get; private set; }
    private IReadOnlyCollection<SimulacaoDominio> _historico = new List<SimulacaoDominio>();
    private IReadOnlyCollection<SimulacoesPorProdutoDiaResultado> _agrupado = new List<SimulacoesPorProdutoDiaResultado>();

    public Task AdicionarAsync(SimulacaoDominio simulacao)
    {
        UltimaSimulacaoAdicionada = simulacao;
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
}
