using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;

public class RegistradorTelemetriaServicos : IRegistradorTelemetriaServicos
{
    private readonly ITelemetriaServicoRepositorio _telemetriaServicoRepositorio;

    public RegistradorTelemetriaServicos(ITelemetriaServicoRepositorio telemetriaServicoRepositorio)
    {
        _telemetriaServicoRepositorio = telemetriaServicoRepositorio;
    }

    public async Task RegistrarAsync(string servico, long tempoRespostaMs, CancellationToken cancellationToken = default)
    {
        await _telemetriaServicoRepositorio.RegistrarChamadaAsync(servico, tempoRespostaMs, cancellationToken);
    }
}
