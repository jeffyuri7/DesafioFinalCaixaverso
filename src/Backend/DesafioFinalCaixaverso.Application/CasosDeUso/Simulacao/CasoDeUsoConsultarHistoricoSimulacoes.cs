using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public class CasoDeUsoConsultarHistoricoSimulacoes : ICasoDeUsoConsultarHistoricoSimulacoes
{
    private readonly ISimulacaoRepositorio _simulacaoRepositorio;

    public CasoDeUsoConsultarHistoricoSimulacoes(ISimulacaoRepositorio simulacaoRepositorio)
    {
        _simulacaoRepositorio = simulacaoRepositorio;
    }

    public async Task<IReadOnlyCollection<HistoricoSimulacaoJson>> Executar(CancellationToken cancellationToken = default)
    {
        var historico = await _simulacaoRepositorio.ListarHistoricoAsync(cancellationToken);

        return historico.Adapt<IReadOnlyCollection<HistoricoSimulacaoJson>>();
    }
}
