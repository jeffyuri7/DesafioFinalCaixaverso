using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Consultas;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface ISimulacaoRepositorio
{
    Task AdicionarAsync(Simulacao simulacao);
    Task<IReadOnlyCollection<Simulacao>> ListarHistoricoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SimulacoesPorProdutoDiaResultado>> ListarAgrupadoPorProdutoEDiaAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Simulacao>> ListarPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
}
