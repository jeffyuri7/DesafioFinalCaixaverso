using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public interface ICasoDeUsoConsultarInvestimentosCliente
{
    Task<IReadOnlyCollection<InvestimentoClienteJson>> Executar(Guid clienteId, CancellationToken cancellationToken = default);
}
