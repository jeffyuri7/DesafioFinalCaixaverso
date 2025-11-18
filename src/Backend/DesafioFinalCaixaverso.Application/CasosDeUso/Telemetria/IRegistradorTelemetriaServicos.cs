using System.Threading;
using System.Threading.Tasks;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Telemetria;

public interface IRegistradorTelemetriaServicos
{
    Task RegistrarAsync(string servico, CancellationToken cancellationToken = default);
}
