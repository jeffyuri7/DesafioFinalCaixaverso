using System;
using System.Threading;
using System.Threading.Tasks;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;

public interface ICasoDeUsoAtualizarPerfilDinamicoCliente
{
    Task Atualizar(Guid clienteId, CancellationToken cancellationToken = default);
}
