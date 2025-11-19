using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public interface ICasoDeUsoCadastrarCliente
{
    Task<ClienteCadastradoJson> Executar(RequisicaoCadastroClienteJson requisicao, CancellationToken cancellationToken = default);
}
