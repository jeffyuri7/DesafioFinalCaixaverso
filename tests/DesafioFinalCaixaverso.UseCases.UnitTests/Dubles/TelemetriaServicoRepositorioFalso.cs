using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class TelemetriaServicoRepositorioFalso : ITelemetriaServicoRepositorio
{
    public List<TelemetriaServico> Registros { get; } = new();
    public string? UltimoServicoRegistrado { get; private set; }

    public Task RegistrarChamadaAsync(string servico, long tempoRespostaMs, CancellationToken cancellationToken = default)
    {
        UltimoServicoRegistrado = servico;
        var registro = Registros.Find(r => r.Servico == servico);
        if (registro is null)
        {
            registro = new TelemetriaServico
            {
                Id = Guid.NewGuid(),
                Servico = servico,
                AnoReferencia = DateTime.UtcNow.Year,
                MesReferencia = DateTime.UtcNow.Month,
                QuantidadeChamadas = 1,
                TempoTotalRespostaMs = tempoRespostaMs,
                UltimaChamada = DateTime.UtcNow
            };
            Registros.Add(registro);
        }
        else
        {
            registro.QuantidadeChamadas++;
            registro.TempoTotalRespostaMs += tempoRespostaMs;
            registro.UltimaChamada = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<TelemetriaServico>> ListarPorPeriodoAsync(int anoReferencia, int mesReferencia, CancellationToken cancellationToken = default)
    {
        var filtrado = Registros
            .FindAll(r => r.AnoReferencia == anoReferencia && r.MesReferencia == mesReferencia)
            .AsReadOnly();

        return Task.FromResult((IReadOnlyCollection<TelemetriaServico>)filtrado);
    }
}
