using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public interface ICasoDeUsoConsultarCliente
{
    Task<ClienteCadastradoJson> Executar(Guid clienteId, CancellationToken cancellationToken = default);
}
