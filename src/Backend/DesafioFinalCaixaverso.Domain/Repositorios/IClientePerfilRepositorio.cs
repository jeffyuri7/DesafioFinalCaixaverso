using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface IClientePerfilRepositorio
{
    Task<ClientePerfil?> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(ClientePerfil clientePerfil, CancellationToken cancellationToken = default);
    Task AtualizarAsync(ClientePerfil clientePerfil, CancellationToken cancellationToken = default);
}
