using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.Repositorios;

public class ClientePerfilRepositorio : IClientePerfilRepositorio
{
    private readonly CaixaversoDbContext _dbContext;

    public ClientePerfilRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task<ClientePerfil?> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .ClientePerfis
            .AsNoTracking()
            .FirstOrDefaultAsync(perfil => perfil.ClienteId == clienteId, cancellationToken);
    }

    public async Task AdicionarAsync(ClientePerfil clientePerfil, CancellationToken cancellationToken = default)
    {
        await _dbContext.ClientePerfis.AddAsync(clientePerfil, cancellationToken);
    }

    public Task AtualizarAsync(ClientePerfil clientePerfil, CancellationToken cancellationToken = default)
    {
        _dbContext.ClientePerfis.Update(clientePerfil);
        return Task.CompletedTask;
    }
}
