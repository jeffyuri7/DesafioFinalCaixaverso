using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.Repositorios;

public class ClientePerfilDinamicoRepositorio : IClientePerfilDinamicoRepositorio
{
    private readonly CaixaversoDbContext _dbContext;

    public ClientePerfilDinamicoRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task<ClientePerfilDinamico?> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClientePerfisDinamicos
            .AsNoTracking()
            .FirstOrDefaultAsync(perfil => perfil.ClienteId == clienteId, cancellationToken);
    }

    public async Task AdicionarAsync(ClientePerfilDinamico clientePerfil, CancellationToken cancellationToken = default)
    {
        await _dbContext.ClientePerfisDinamicos.AddAsync(clientePerfil, cancellationToken);
    }

    public Task AtualizarAsync(ClientePerfilDinamico clientePerfil, CancellationToken cancellationToken = default)
    {
        _dbContext.ClientePerfisDinamicos.Update(clientePerfil);
        return Task.CompletedTask;
    }
}
