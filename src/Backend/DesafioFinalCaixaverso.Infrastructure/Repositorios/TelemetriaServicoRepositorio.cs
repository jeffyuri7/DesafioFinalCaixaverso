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
    private readonly CaixaversoDbContext _dbContext;

    public TelemetriaServicoRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task RegistrarChamadaAsync(string servico, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(servico))
            return;

        var servicoNormalizado = servico.Trim();

        var registro = await _dbContext.TelemetriaServicos
            .FirstOrDefaultAsync(t => t.Servico == servicoNormalizado, cancellationToken);

        if (registro is null)
        {
            registro = new TelemetriaServico
            {
                Id = Guid.NewGuid(),
                Servico = servicoNormalizado,
                QuantidadeChamadas = 1,
                UltimaChamada = DateTime.UtcNow
            };

            await _dbContext.TelemetriaServicos.AddAsync(registro, cancellationToken);
        }
        else
        {
            registro.QuantidadeChamadas++;
            registro.UltimaChamada = DateTime.UtcNow;
            _dbContext.TelemetriaServicos.Update(registro);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TelemetriaServico>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var registros = await _dbContext.TelemetriaServicos
            .AsNoTracking()
            .OrderByDescending(t => t.QuantidadeChamadas)
            .ThenByDescending(t => t.UltimaChamada)
            .ToListAsync(cancellationToken);

        return registros.AsReadOnly();
    }
}
