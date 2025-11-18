using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public class CasoDeUsoConsultarSimulacoesPorProdutoDia : ICasoDeUsoConsultarSimulacoesPorProdutoDia
{
    private readonly ISimulacaoRepositorio _simulacaoRepositorio;

    public CasoDeUsoConsultarSimulacoesPorProdutoDia(ISimulacaoRepositorio simulacaoRepositorio)
    {
        _simulacaoRepositorio = simulacaoRepositorio;
    }

    public async Task<IReadOnlyCollection<SimulacoesPorProdutoDiaJson>> Executar(CancellationToken cancellationToken = default)
    {
        var agrupado = await _simulacaoRepositorio.ListarAgrupadoPorProdutoEDiaAsync(cancellationToken);

        return agrupado.Adapt<IReadOnlyCollection<SimulacoesPorProdutoDiaJson>>();
    }
}
