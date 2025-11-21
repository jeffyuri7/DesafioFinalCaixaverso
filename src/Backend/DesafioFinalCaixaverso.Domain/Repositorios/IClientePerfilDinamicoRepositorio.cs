using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios
{
    public interface IClientePerfilDinamicoRepositorio
    {
        Task<ClientePerfilDinamico?> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
        Task AdicionarAsync(ClientePerfilDinamico clientePerfil, CancellationToken cancellationToken = default);
        Task AtualizarAsync(ClientePerfilDinamico clientePerfil, CancellationToken cancellationToken = default);
    }
}
