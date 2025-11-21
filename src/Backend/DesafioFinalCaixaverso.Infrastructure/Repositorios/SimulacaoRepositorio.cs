using System;
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
        var existemSimulacoes = await _dbContext.Simulacoes.AsNoTracking().AnyAsync(cancellationToken);
        if (!existemSimulacoes)
            return Array.Empty<SimulacoesPorProdutoDiaResultado>();

        var query = _dbContext.Simulacoes
            .AsNoTracking()
            .Join(
                _dbContext.Produtos.AsNoTracking(),
                simulacao => simulacao.ProdutoId,
                produto => produto.Id,
                (simulacao, produto) => new { simulacao, produto });

        if (_dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            cancellationToken.ThrowIfCancellationRequested();

            var agrupadoMemoria = query
                .AsEnumerable()
                .GroupBy(item => new { Produto = item.produto.Nome, Data = item.simulacao.DataSimulacao.Date })
                .Select(grupo => new SimulacoesPorProdutoDiaResultado(
                    grupo.Key.Produto,
                    grupo.Key.Data,
                    grupo.Count(),
                    grupo.Average(item => item.simulacao.ValorFinal)))
                .OrderByDescending(resultado => resultado.Data)
                .ThenBy(resultado => resultado.Produto)
                .ToList();

            return agrupadoMemoria.AsReadOnly();
        }

        var agrupadoSql = await query
            .GroupBy(item => new
            {
                Produto = item.produto.Nome,
                DiaOrdinal = EF.Functions.DateDiffDay(DateTime.UnixEpoch, item.simulacao.DataSimulacao)
            })
            .Select(grupo => new
            {
                grupo.Key.Produto,
                grupo.Key.DiaOrdinal,
                QuantidadeSimulacoes = grupo.Count(),
                MediaValorFinal = grupo.Average(item => item.simulacao.ValorFinal)
            })
            .OrderByDescending(resultado => resultado.DiaOrdinal)
            .ThenBy(resultado => resultado.Produto)
            .ToListAsync(cancellationToken);

        var agrupado = agrupadoSql
            .Select(resultado =>
            {
                var diasDesdeEpoch = resultado.DiaOrdinal;
                var dia = DateTime.UnixEpoch.AddDays(diasDesdeEpoch).Date;

                return new SimulacoesPorProdutoDiaResultado(
                    resultado.Produto,
                    dia,
                    resultado.QuantidadeSimulacoes,
                    resultado.MediaValorFinal);
            })
            .ToList();

        return agrupado.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Simulacao>> ListarPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var simulacoes = await _dbContext.Simulacoes
            .AsNoTracking()
            .Include(simulacao => simulacao.Produto)
            .Where(simulacao => simulacao.ClienteId == clienteId)
            .OrderByDescending(simulacao => simulacao.DataSimulacao)
            .ToListAsync(cancellationToken);

        return simulacoes.AsReadOnly();
    }
}
