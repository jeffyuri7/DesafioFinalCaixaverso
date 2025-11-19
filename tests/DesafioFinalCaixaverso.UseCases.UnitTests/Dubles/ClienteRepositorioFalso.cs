using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class ClienteRepositorioFalso : IClienteRepositorio
{
    private bool _clienteExiste;
    private Cliente? _cliente;
    private readonly HashSet<string> _emailsCadastrados = new(StringComparer.OrdinalIgnoreCase);

    public Cliente? ClienteAdicionado { get; private set; }

    public ClienteRepositorioFalso ComClienteExistente(Cliente cliente)
    {
        _clienteExiste = true;
        _cliente = cliente;
        _emailsCadastrados.Add(cliente.Email);
        return this;
    }

    public ClienteRepositorioFalso SemCliente()
    {
        _clienteExiste = false;
        _cliente = null;
        return this;
    }

    public ClienteRepositorioFalso ComEmailJaCadastrado(string email)
    {
        _emailsCadastrados.Add(email);
        return this;
    }

    public Task<bool> ExisteClienteAsync(Guid clienteId)
    {
        var existeClienteAtual = _clienteExiste && _cliente?.Id == clienteId;
        var existeClienteAdicionado = ClienteAdicionado?.Id == clienteId;
        return Task.FromResult(existeClienteAtual || existeClienteAdicionado);
    }

    public Task<Cliente?> ObterPorIdAsync(Guid clienteId)
    {
        if (_cliente is not null && _cliente.Id == clienteId)
            return Task.FromResult<Cliente?>(_cliente);

        if (ClienteAdicionado is not null && ClienteAdicionado.Id == clienteId)
            return Task.FromResult<Cliente?>(ClienteAdicionado);

        return Task.FromResult<Cliente?>(null);
    }

    public Task<Cliente?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (_cliente is not null && string.Equals(_cliente.Email, email, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult<Cliente?>(_cliente);

        if (ClienteAdicionado is not null && string.Equals(ClienteAdicionado.Email, email, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult<Cliente?>(ClienteAdicionado);

        return Task.FromResult<Cliente?>(null);
    }

    public Task<bool> EmailJaCadastradoAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_emailsCadastrados.Contains(email));
    }

    public Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        ClienteAdicionado = cliente;
        _cliente = cliente;
        _clienteExiste = true;
        _emailsCadastrados.Add(cliente.Email);
        return Task.CompletedTask;
    }
}
