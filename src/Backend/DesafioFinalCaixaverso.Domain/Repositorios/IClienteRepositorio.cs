using System;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.Dominio.Repositorios;

public interface IClienteRepositorio
{
    Task<bool> ExisteClienteAsync(Guid clienteId);
    Task<Cliente?> ObterPorIdAsync(Guid clienteId);
}
