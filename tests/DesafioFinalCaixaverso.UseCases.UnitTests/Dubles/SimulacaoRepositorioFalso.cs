using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using SimulacaoDominio = DesafioFinalCaixaverso.Dominio.Entidades.Simulacao;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class SimulacaoRepositorioFalso : ISimulacaoRepositorio
{
    public SimulacaoDominio? UltimaSimulacaoAdicionada { get; private set; }

    public Task AdicionarAsync(SimulacaoDominio simulacao)
    {
        UltimaSimulacaoAdicionada = simulacao;
        return Task.CompletedTask;
    }
}
