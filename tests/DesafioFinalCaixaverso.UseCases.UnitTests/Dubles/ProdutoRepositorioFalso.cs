using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
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
        var tipoNormalizado = NormalizarTipo(tipoProduto);
        var produtosFiltrados = _produtos
            .Where(produto => produto.Ativo && NormalizarTipo(produto.Tipo) == tipoNormalizado)
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

    public Task<IReadOnlyCollection<Produto>> ListarPorPerfilAsync(PerfilInvestidor perfil, CancellationToken cancellationToken = default)
    {
        var produtosFiltrados = _produtos
            .Where(produto => produto.Ativo && RiscoEhPermitido(perfil, produto.Risco))
            .OrderBy(produto => produto.Risco)
            .ThenBy(produto => produto.Nome)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyCollection<Produto>>(produtosFiltrados);
    }

    private static bool RiscoEhPermitido(PerfilInvestidor perfilInvestidor, Risco riscoProduto)
    {
        return perfilInvestidor switch
        {
            PerfilInvestidor.Conservador => riscoProduto == Risco.Baixo,
            PerfilInvestidor.Moderado => riscoProduto == Risco.Medio,
            PerfilInvestidor.Agressivo => riscoProduto == Risco.Alto,
            _ => false
        };
    }

    private static string NormalizarTipo(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        var trimmed = valor.Trim().ToUpperInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(trimmed.Length);

        foreach (var caractere in trimmed)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(caractere);
            if (categoria == UnicodeCategory.NonSpacingMark)
                continue;

            if (char.IsLetterOrDigit(caractere))
                builder.Append(caractere);
        }

        return builder.ToString();
    }
}
