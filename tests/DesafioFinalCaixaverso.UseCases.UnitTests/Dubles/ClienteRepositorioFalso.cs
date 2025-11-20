using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class ClienteRepositorioFalso : IClienteRepositorio
{
    private readonly List<Cliente> _clientes = new();
    private readonly HashSet<string> _emailsCadastrados = new(StringComparer.OrdinalIgnoreCase);

    public Cliente? ClienteAdicionado { get; private set; }

    public ClienteRepositorioFalso ComClienteExistente(Cliente cliente)
    {
        AdicionarOuAtualizar(cliente);
        _emailsCadastrados.Add(cliente.Email);
        SincronizarEmails();
        return this;
    }

    public ClienteRepositorioFalso SemCliente()
    {
        _clientes.Clear();
        ClienteAdicionado = null;
        _emailsCadastrados.Clear();
        return this;
    }

    public ClienteRepositorioFalso ComEmailJaCadastrado(string email)
    {
        _emailsCadastrados.Add(email);
        return this;
    }

    public Task<bool> ExisteClienteAsync(Guid clienteId)
    {
        var existeCliente = _clientes.Any(cliente => cliente.Id == clienteId);
        return Task.FromResult(existeCliente);
    }

    public Task<Cliente?> ObterPorIdAsync(Guid clienteId)
    {
        var cliente = _clientes.FirstOrDefault(cliente => cliente.Id == clienteId);
        return Task.FromResult<Cliente?>(cliente);
    }

    public Task<Cliente?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var cliente = _clientes.FirstOrDefault(cliente => string.Equals(cliente.Email, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<Cliente?>(cliente);
    }

    public Task<bool> EmailJaCadastradoAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_emailsCadastrados.Contains(email));
    }

    public Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        AdicionarOuAtualizar(cliente);
        ClienteAdicionado = cliente;
        SincronizarEmails();
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Cliente>> ListarAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Cliente> clientes = _clientes.AsReadOnly();
        return Task.FromResult(clientes);
    }

    public Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        AdicionarOuAtualizar(cliente);
        SincronizarEmails();
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _clientes.RemoveAll(c => c.Id == cliente.Id);
        if (ClienteAdicionado?.Id == cliente.Id)
            ClienteAdicionado = null;

        SincronizarEmails();

        return Task.CompletedTask;
    }

    private void AdicionarOuAtualizar(Cliente cliente)
    {
        var indice = _clientes.FindIndex(c => c.Id == cliente.Id);
        if (indice >= 0)
            _clientes[indice] = cliente;
        else
            _clientes.Add(cliente);

        _emailsCadastrados.Add(cliente.Email);
    }

    private void SincronizarEmails()
    {
        _emailsCadastrados.RemoveWhere(email => !_clientes.Any(cliente => string.Equals(cliente.Email, email, StringComparison.OrdinalIgnoreCase)));
    }
}
