using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;

public interface ICasoDeUsoConsultarTelemetriaServicos
{
    Task<IReadOnlyCollection<TelemetriaServicosJson>> Executar(CancellationToken cancellationToken = default);
}
