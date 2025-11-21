using System;
using Bogus;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorSimulacao
{
    private readonly Simulacao _simulacao;

    public ConstrutorSimulacao()
    {
        var cliente = new ConstrutorCliente().Construir();
        var produto = new ConstrutorProduto().Construir();

        _simulacao = new Faker<Simulacao>("pt_BR")
            .RuleFor(s => s.Id, _ => Guid.NewGuid())
            .RuleFor(s => s.ClienteId, _ => cliente.Id)
            .RuleFor(s => s.Cliente, _ => cliente)
            .RuleFor(s => s.ProdutoId, _ => produto.Id)
            .RuleFor(s => s.Produto, _ => produto)
            .RuleFor(s => s.ValorInvestido, faker => faker.Finance.Amount(1_000, 10_000))
            .RuleFor(s => s.ValorFinal, faker => faker.Finance.Amount(10_001, 20_000))
            .RuleFor(s => s.RentabilidadeEfetiva, faker => faker.Random.Decimal(0.01m, 0.30m))
            .RuleFor(s => s.PrazoMeses, faker => faker.Random.Int(6, 60))
            .RuleFor(s => s.DataSimulacao, faker => faker.Date.RecentOffset(30).UtcDateTime)
            .Generate();
    }

    public ConstrutorSimulacao ComProduto(Produto produto)
    {
        _simulacao.Produto = produto;
        _simulacao.ProdutoId = produto.Id;
        return this;
    }

    public ConstrutorSimulacao ComClienteId(Guid clienteId)
    {
        _simulacao.ClienteId = clienteId;
        _simulacao.Cliente = null;
        return this;
    }

    public ConstrutorSimulacao ComDataSimulacao(DateTime data)
    {
        _simulacao.DataSimulacao = data;
        return this;
    }

    public ConstrutorSimulacao ComValorInvestido(decimal valor)
    {
        _simulacao.ValorInvestido = valor;
        return this;
    }

    public ConstrutorSimulacao ComValorFinal(decimal valor)
    {
        _simulacao.ValorFinal = valor;
        return this;
    }

    public Simulacao Construir() => _simulacao;
}
