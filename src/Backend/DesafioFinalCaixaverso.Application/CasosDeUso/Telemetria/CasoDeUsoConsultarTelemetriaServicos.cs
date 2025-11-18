using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;

public class CasoDeUsoConsultarTelemetriaServicos : ICasoDeUsoConsultarTelemetriaServicos
{
    private readonly ITelemetriaServicoRepositorio _telemetriaServicoRepositorio;

    public CasoDeUsoConsultarTelemetriaServicos(ITelemetriaServicoRepositorio telemetriaServicoRepositorio)
    {
        _telemetriaServicoRepositorio = telemetriaServicoRepositorio;
    }

    public async Task<IReadOnlyCollection<TelemetriaServicosJson>> Executar(CancellationToken cancellationToken = default)
    {
        var registros = await _telemetriaServicoRepositorio.ListarAsync(cancellationToken);

        return registros.Adapt<IReadOnlyCollection<TelemetriaServicosJson>>();
    }
}
