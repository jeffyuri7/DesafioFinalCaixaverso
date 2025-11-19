using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Infraestrutura.AcessoDados;
using Microsoft.EntityFrameworkCore;

namespace DesafioFinalCaixaverso.Infraestrutura.Repositorios;

public class ProdutoRepositorio : IProdutoRepositorio
{
    private readonly CaixaversoDbContext _dbContext;

    public ProdutoRepositorio(CaixaversoDbContext dbContext) => _dbContext = dbContext;

    public async Task AdicionarAsync(Produto produto) => await _dbContext.Produtos.AddAsync(produto);

    public async Task<IReadOnlyCollection<Produto>> ListarAtivosPorTipoAsync(string tipoProduto)
    {
        var tipoNormalizado = tipoProduto.Trim().ToUpperInvariant();

        return await _dbContext
            .Produtos
            .AsNoTracking()
            .Where(produto => produto.Ativo && produto.Tipo.ToUpper() == tipoNormalizado)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Produto>> ListarPorPerfilAsync(PerfilInvestidor perfil, CancellationToken cancellationToken = default)
    {
        var riscosPermitidos = perfil switch
        {
            PerfilInvestidor.Conservador => new[] { Risco.Baixo },
            PerfilInvestidor.Moderado => new[] { Risco.Baixo, Risco.Medio },
            _ => new[] { Risco.Baixo, Risco.Medio, Risco.Alto }
        };

        var produtos = await _dbContext
            .Produtos
            .AsNoTracking()
            .Where(produto => produto.Ativo && riscosPermitidos.Contains(produto.Risco))
            .OrderBy(produto => produto.Risco)
            .ThenBy(produto => produto.Nome)
            .ToListAsync(cancellationToken);

        return produtos.AsReadOnly();
    }
}
