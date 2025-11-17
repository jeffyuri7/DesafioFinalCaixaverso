using System;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.Repositorios;

public class ClienteRepositorio : IClienteRepositorio
{
    private readonly CaixaversoDbContext _dbContext;

    public ClienteRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task<bool> ExisteClienteAsync(Guid clienteId) => await _dbContext
        .Clientes.AsNoTracking().AnyAsync(cliente => cliente.Id == clienteId);

    public async Task<Cliente?> ObterPorIdAsync(Guid clienteId) => await _dbContext
        .Clientes.AsNoTracking().FirstOrDefaultAsync(cliente => cliente.Id == clienteId);
}
