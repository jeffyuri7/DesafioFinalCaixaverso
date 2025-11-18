using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface ITelemetriaServicoRepositorio
{
    Task RegistrarChamadaAsync(string servico, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TelemetriaServico>> ListarAsync(CancellationToken cancellationToken = default);
}
