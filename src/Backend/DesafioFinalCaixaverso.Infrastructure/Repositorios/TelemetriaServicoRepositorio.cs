using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.Repositorios;

public class TelemetriaServicoRepositorio : ITelemetriaServicoRepositorio
{
    private const int TamanhoMaximoNomeServico = 300;
    private readonly CaixaversoDbContext _dbContext;

    public TelemetriaServicoRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task RegistrarChamadaAsync(string servico, long tempoRespostaMs, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(servico))
            return;

        var servicoNormalizado = servico.Trim();
        if (servicoNormalizado.Length > TamanhoMaximoNomeServico)
            servicoNormalizado = servicoNormalizado[..TamanhoMaximoNomeServico];

        var agora = DateTime.UtcNow;
        var anoReferencia = agora.Year;
        var mesReferencia = agora.Month;
        var tempoRegistrado = tempoRespostaMs < 0 ? 0 : tempoRespostaMs;

        var registro = await _dbContext.TelemetriaServicos
            .FirstOrDefaultAsync(
                t => t.Servico == servicoNormalizado &&
                     t.AnoReferencia == anoReferencia &&
                     t.MesReferencia == mesReferencia,
                cancellationToken);

        if (registro is null)
        {
            registro = new TelemetriaServico
            {
                Id = Guid.NewGuid(),
                Servico = servicoNormalizado,
                AnoReferencia = anoReferencia,
                MesReferencia = mesReferencia,
                QuantidadeChamadas = 1,
                TempoTotalRespostaMs = tempoRegistrado,
                UltimaChamada = agora
            };

            await _dbContext.TelemetriaServicos.AddAsync(registro, cancellationToken);
        }
        else
        {
            registro.QuantidadeChamadas++;
            registro.TempoTotalRespostaMs += tempoRegistrado;
            registro.UltimaChamada = agora;
            _dbContext.TelemetriaServicos.Update(registro);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TelemetriaServico>> ListarPorPeriodoAsync(int anoReferencia, int mesReferencia, CancellationToken cancellationToken = default)
    {
        var registros = await _dbContext.TelemetriaServicos
            .AsNoTracking()
            .Where(t => t.AnoReferencia == anoReferencia && t.MesReferencia == mesReferencia)
            .OrderByDescending(t => t.QuantidadeChamadas)
            .ThenByDescending(t => t.UltimaChamada)
            .ToListAsync(cancellationToken);

        return registros.AsReadOnly();
    }
}
