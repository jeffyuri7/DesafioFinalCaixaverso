using System;
using Bogus;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorProduto
{
    private readonly Produto _produto;

    public ConstrutorProduto()
    {
        _produto = new Faker<Produto>("pt_BR")
            .RuleFor(p => p.Id, _ => Guid.NewGuid())
            .RuleFor(p => p.Nome, faker => faker.Commerce.ProductName())
            .RuleFor(p => p.Tipo, faker => faker.Commerce.ProductMaterial())
            .RuleFor(p => p.Rentabilidade, faker => Math.Round(faker.Random.Decimal(0.05m, 0.20m), 4))
            .RuleFor(p => p.Risco, faker => faker.PickRandom<Risco>())
            .RuleFor(p => p.LiquidezDias, faker => faker.Random.Int(1, 90))
            .RuleFor(p => p.MinimoInvestimento, faker => faker.Random.Decimal(500m, 50_000m))
            .RuleFor(p => p.PrazoMinimoMeses, faker => faker.Random.Int(1, 12))
            .RuleFor(p => p.PrazoMaximoMeses, (faker, produto) => faker.Random.Int(produto.PrazoMinimoMeses + 1, 120))
            .RuleFor(p => p.Ativo, faker => faker.Random.Bool(0.85f))
            .Generate();
    }

    public ConstrutorProduto ComId(Guid id)
    {
        _produto.Id = id;
        return this;
    }

    public ConstrutorProduto ComNome(string nome)
    {
        _produto.Nome = nome;
        return this;
    }

    public ConstrutorProduto ComTipo(string tipo)
    {
    _produto.Tipo = string.IsNullOrWhiteSpace(tipo) ? "CDB" : tipo.Trim().ToUpper();
        return this;
    }

    public ConstrutorProduto ComRisco(Risco risco)
    {
        _produto.Risco = risco;
        return this;
    }

    public ConstrutorProduto ComRentabilidade(decimal rentabilidade)
    {
        _produto.Rentabilidade = rentabilidade;
        return this;
    }

    public ConstrutorProduto ComMinimoInvestimento(decimal minimo)
    {
        _produto.MinimoInvestimento = minimo;
        return this;
    }

    public ConstrutorProduto ComPrazo(int minimoMeses, int maximoMeses)
    {
        _produto.PrazoMinimoMeses = minimoMeses;
        _produto.PrazoMaximoMeses = maximoMeses;
        return this;
    }

    public ConstrutorProduto ComLiquidezDias(int dias)
    {
        _produto.LiquidezDias = dias;
        return this;
    }

    public ConstrutorProduto Ativo(bool ativo)
    {
        _produto.Ativo = ativo;
        return this;
    }

    public Produto Construir()
    {
    _produto.Tipo = string.IsNullOrWhiteSpace(_produto.Tipo) ? "CDB" : _produto.Tipo.Trim().ToUpper();
    return _produto;
    }
}
