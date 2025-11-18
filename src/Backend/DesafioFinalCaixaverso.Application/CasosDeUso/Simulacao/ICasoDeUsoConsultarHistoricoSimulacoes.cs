using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public interface ICasoDeUsoConsultarHistoricoSimulacoes
{
    Task<IReadOnlyCollection<HistoricoSimulacaoJson>> Executar(CancellationToken cancellationToken = default);
}
