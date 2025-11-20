using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;

public class CasoDeUsoConsultarTelemetriaServicos : ICasoDeUsoConsultarTelemetriaServicos
{
    private readonly ITelemetriaServicoRepositorio _telemetriaServicoRepositorio;

    public CasoDeUsoConsultarTelemetriaServicos(ITelemetriaServicoRepositorio telemetriaServicoRepositorio)
    {
        _telemetriaServicoRepositorio = telemetriaServicoRepositorio;
    }

    public async Task<TelemetriaResumoJson> Executar(CancellationToken cancellationToken = default)
    {
        var agora = DateTime.UtcNow;
        var anoReferencia = agora.Year;
        var mesReferencia = agora.Month;
        var inicioPeriodo = new DateTime(anoReferencia, mesReferencia, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimPeriodo = inicioPeriodo.AddMonths(1).AddTicks(-1);

        var registros = await _telemetriaServicoRepositorio.ListarPorPeriodoAsync(anoReferencia, mesReferencia, cancellationToken);

        var servicos = registros
            .OrderByDescending(registro => registro.QuantidadeChamadas)
            .ThenBy(registro => registro.Servico)
            .Select(registro => new TelemetriaServicosJson
            {
                Nome = registro.Servico,
                QuantidadeChamadas = registro.QuantidadeChamadas,
                MediaTempoRespostaMs = CalcularMediaResposta(registro)
            })
            .ToList();

        return new TelemetriaResumoJson
        {
            Servicos = servicos,
            Periodo = new TelemetriaPeriodoJson
            {
                Inicio = inicioPeriodo,
                Fim = fimPeriodo
            }
        };
    }

    private static double CalcularMediaResposta(TelemetriaServico registro)
    {
        if (registro.QuantidadeChamadas <= 0)
            return 0;

        var media = (double)registro.TempoTotalRespostaMs / registro.QuantidadeChamadas;
        return Math.Round(media, 2, MidpointRounding.AwayFromZero);
    }
}
