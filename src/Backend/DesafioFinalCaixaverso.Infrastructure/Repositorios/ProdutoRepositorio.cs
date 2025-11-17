using System.Collections.Generic;
using System.Linq;
using DesafioFinalCaixaverso.Dominio.Entidades;
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
}
