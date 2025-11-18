using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Consultas;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.Repositorios;

public class SimulacaoRepositorio : ISimulacaoRepositorio
{
    private readonly CaixaversoDbContext _dbContext;

    public SimulacaoRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task AdicionarAsync(Simulacao simulacao) => await _dbContext.Simulacoes.AddAsync(simulacao);

    public async Task<IReadOnlyCollection<Simulacao>> ListarHistoricoAsync(CancellationToken cancellationToken = default)
    {
        var historico = await _dbContext.Simulacoes
            .AsNoTracking()
            .Include(simulacao => simulacao.Produto)
            .OrderByDescending(simulacao => simulacao.DataSimulacao)
            .ToListAsync(cancellationToken);

        return historico.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<SimulacoesPorProdutoDiaResultado>> ListarAgrupadoPorProdutoEDiaAsync(CancellationToken cancellationToken = default)
    {
        var agrupado = await _dbContext.Simulacoes
            .AsNoTracking()
            .Include(simulacao => simulacao.Produto)
            .Where(simulacao => simulacao.Produto != null)
            .GroupBy(simulacao => new { Produto = simulacao.Produto!.Nome, Dia = simulacao.DataSimulacao.Date })
            .Select(grupo => new SimulacoesPorProdutoDiaResultado(
                grupo.Key.Produto,
                grupo.Key.Dia,
                grupo.Count(),
                grupo.Sum(simulacao => simulacao.ValorInvestido)))
            .OrderByDescending(resultado => resultado.Dia)
            .ThenBy(resultado => resultado.Produto)
            .ToListAsync(cancellationToken);

        return agrupado.AsReadOnly();
    }
}
