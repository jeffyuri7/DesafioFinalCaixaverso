using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface ITelemetriaServicoRepositorio
{
    Task RegistrarChamadaAsync(string servico, long tempoRespostaMs, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TelemetriaServico>> ListarPorPeriodoAsync(int anoReferencia, int mesReferencia, CancellationToken cancellationToken = default);
}
