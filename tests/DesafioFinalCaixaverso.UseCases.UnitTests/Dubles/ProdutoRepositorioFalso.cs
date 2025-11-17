using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;

public class ProdutoRepositorioFalso : IProdutoRepositorio
{
    private IReadOnlyCollection<Produto> _produtos = new List<Produto>();

    public ProdutoRepositorioFalso ComProdutos(IEnumerable<Produto> produtos)
    {
        _produtos = produtos.ToList();
        return this;
    }

    public Task<IReadOnlyCollection<Produto>> ListarAtivosPorTipoAsync(string tipoProduto)
    {
        var produtosFiltrados = _produtos
            .Where(produto => produto.Tipo.Equals(tipoProduto, StringComparison.InvariantCultureIgnoreCase) && produto.Ativo)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyCollection<Produto>>(produtosFiltrados);
    }

    public Task AdicionarAsync(Produto produto)
    {
        var lista = _produtos.ToList();
        lista.Add(produto);
        _produtos = lista;
        return Task.CompletedTask;
    }
}
