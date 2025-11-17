using System;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class ClienteRepositorioFalso : IClienteRepositorio
{
    private bool _clienteExiste;
    private Cliente? _cliente;

    public ClienteRepositorioFalso ComClienteExistente(Cliente cliente)
    {
        _clienteExiste = true;
        _cliente = cliente;
        return this;
    }

    public ClienteRepositorioFalso SemCliente()
    {
        _clienteExiste = false;
        _cliente = null;
        return this;
    }

    public Task<bool> ExisteClienteAsync(Guid clienteId)
    {
        return Task.FromResult(_clienteExiste && _cliente?.Id == clienteId);
    }

    public Task<Cliente?> ObterPorIdAsync(Guid clienteId)
    {
        if (_cliente is null || _cliente.Id != clienteId)
        {
            return Task.FromResult<Cliente?>(null);
        }

        return Task.FromResult<Cliente?>(_cliente);
    }
}
