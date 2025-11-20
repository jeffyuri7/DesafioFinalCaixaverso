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
    var tipoNormalizado = TipoProdutoNormalizador.NormalizarTipo(tipoProduto);
        if (string.IsNullOrEmpty(tipoNormalizado))
            return Array.Empty<Produto>();

        var produtosAtivos = await _dbContext
            .Produtos
            .AsNoTracking()
            .Where(produto => produto.Ativo)
            .ToListAsync();

        var produtosCompativeis = produtosAtivos
            .Where(produto => TipoProdutoNormalizador.NormalizarTipo(produto.Tipo) == tipoNormalizado)
            .ToList();

        return produtosCompativeis.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Produto>> ListarPorPerfilAsync(PerfilInvestidor perfil, CancellationToken cancellationToken = default)
    {
        var riscoEsperado = perfil switch
        {
            PerfilInvestidor.Conservador => Risco.Baixo,
            PerfilInvestidor.Moderado => Risco.Medio,
            PerfilInvestidor.Agressivo => Risco.Alto,
            _ => (Risco?)null
        };

        if (!riscoEsperado.HasValue)
        {
            return Array.Empty<Produto>();
        }

        var produtos = await _dbContext
            .Produtos
            .AsNoTracking()
            .Where(produto => produto.Ativo && produto.Risco == riscoEsperado.Value)
            .OrderBy(produto => produto.Nome)
            .ToListAsync(cancellationToken);

        return produtos.AsReadOnly();
    }
}

internal static class TipoProdutoNormalizador
{
    public static string NormalizarTipo(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        var trimmed = valor.Trim().ToUpperInvariant();
        var normalized = trimmed.Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder(normalized.Length);
        foreach (var caractere in normalized)
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
