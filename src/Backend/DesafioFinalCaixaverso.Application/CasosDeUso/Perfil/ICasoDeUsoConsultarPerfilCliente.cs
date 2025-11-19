using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;

public interface ICasoDeUsoConsultarPerfilCliente
{
    Task<PerfilClienteJson> Executar(Guid clienteId, CancellationToken cancellationToken = default);
}
