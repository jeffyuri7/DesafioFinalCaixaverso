using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public interface ICasoDeUsoAtualizarCliente
{
    Task<ClienteCadastradoJson> Executar(Guid clienteId, RequisicaoAtualizacaoClienteJson requisicao, CancellationToken cancellationToken = default);
}
