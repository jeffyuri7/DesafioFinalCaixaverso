using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public interface ICasoDeUsoResponderQuestionarioCliente
{
    Task<QuestionarioRespondidoJson> Executar(Guid clienteId, RequisicaoQuestionarioClienteJson requisicao, CancellationToken cancellationToken = default);
}
