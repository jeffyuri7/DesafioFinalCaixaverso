using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;

namespace DesafioFinalCaixaverso.Infraestrutura.Repositorios;

public class SimulacaoRepositorio : ISimulacaoRepositorio
{
    private readonly CaixaversoDbContext _dbContext;

    public SimulacaoRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task AdicionarAsync(Simulacao simulacao) => await _dbContext.Simulacoes.AddAsync(simulacao);
}
