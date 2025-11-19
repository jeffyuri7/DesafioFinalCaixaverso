using System;
using System.Threading;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface IClienteRepositorio
{
    Task<bool> ExisteClienteAsync(Guid clienteId);
    Task<Cliente?> ObterPorIdAsync(Guid clienteId);
    Task<Cliente?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailJaCadastradoAsync(string email, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default);
}
