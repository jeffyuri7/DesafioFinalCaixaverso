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

    public Task RegistrarChamadaAsync(string servico, CancellationToken cancellationToken = default)
    {
        UltimoServicoRegistrado = servico;
        var registro = Registros.Find(r => r.Servico == servico);
        if (registro is null)
        {
            registro = new TelemetriaServico
            {
                Id = Guid.NewGuid(),
                Servico = servico,
                QuantidadeChamadas = 1,
                UltimaChamada = DateTime.UtcNow
            };
            Registros.Add(registro);
        }
        else
        {
            registro.QuantidadeChamadas++;
            registro.UltimaChamada = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<TelemetriaServico>> ListarAsync(CancellationToken cancellationToken = default)
        => Task.FromResult((IReadOnlyCollection<TelemetriaServico>)Registros.AsReadOnly());
}
