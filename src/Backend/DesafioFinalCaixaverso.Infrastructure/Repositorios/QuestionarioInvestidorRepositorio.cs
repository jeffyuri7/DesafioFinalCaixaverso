using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.Repositorios;

public class QuestionarioInvestidorRepositorio : IQuestionarioInvestidorRepositorio
{
    private readonly CaixaversoDbContext _dbContext;

    public QuestionarioInvestidorRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task<QuestionarioInvestidor?> ObterPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .QuestionariosInvestidor
            .AsNoTracking()
            .FirstOrDefaultAsync(questionario => questionario.ClienteId == clienteId, cancellationToken);
    }
}
