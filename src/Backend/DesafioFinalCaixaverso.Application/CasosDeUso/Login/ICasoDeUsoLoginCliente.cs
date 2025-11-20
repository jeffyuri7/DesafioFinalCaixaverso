using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Login;

public interface ICasoDeUsoLoginCliente
{
    Task<ClienteAutenticadoJson> Executar(RequisicaoLoginClienteJson requisicao, CancellationToken cancellationToken = default);
}
