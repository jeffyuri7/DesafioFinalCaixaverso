using System;
using System.Threading;
using System.Threading.Tasks;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public interface ICasoDeUsoExcluirCliente
{
    Task Executar(Guid clienteId, CancellationToken cancellationToken = default);
}
