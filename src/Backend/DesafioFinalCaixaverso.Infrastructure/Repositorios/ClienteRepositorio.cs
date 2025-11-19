using System;
using System.Threading;
using System.Threading.Tasks;
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
        .Clientes.AsNoTracking()
        .AnyAsync(cliente => cliente.Id == clienteId);

    public async Task<Cliente?> ObterPorIdAsync(Guid clienteId) => await _dbContext
        .Clientes.AsNoTracking().FirstOrDefaultAsync(cliente => cliente.Id == clienteId);

    public async Task<Cliente?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(cliente => cliente.Email == email, cancellationToken);
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Clientes
            .AsNoTracking()
            .AnyAsync(cliente => cliente.Email == email, cancellationToken);
    }

    public async Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _dbContext.Clientes.AddAsync(cliente, cancellationToken);
    }
}
