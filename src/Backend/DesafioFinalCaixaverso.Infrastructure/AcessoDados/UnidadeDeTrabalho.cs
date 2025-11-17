using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.Infraestrutura.AcessoDados;

public class UnidadeDeTrabalho : IUnidadeDeTrabalho
{
    private readonly CaixaversoDbContext _dbContext;

    public UnidadeDeTrabalho(CaixaversoDbContext dbContext) => _dbContext = dbContext;
    public async Task Commit() => await _dbContext.SaveChangesAsync();
}
