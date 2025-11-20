using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public interface ICasoDeUsoListarClientes
{
    Task<IReadOnlyCollection<ClienteCadastradoJson>> Executar(CancellationToken cancellationToken = default);
}
