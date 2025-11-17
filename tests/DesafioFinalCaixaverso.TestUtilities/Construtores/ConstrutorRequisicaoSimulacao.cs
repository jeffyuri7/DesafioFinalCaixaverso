using System;
using Bogus;
using DesafioFinalCaixaverso.Communications.Requests;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorRequisicaoSimulacao
{
    private readonly RequisicaoSimulacaoJson _requisicao;

    public ConstrutorRequisicaoSimulacao()
    {
        _requisicao = new Faker<RequisicaoSimulacaoJson>(locale: "pt_BR")
            .RuleFor(r => r.ClienteId, _ => Guid.NewGuid())
            .RuleFor(r => r.Valor, faker => faker.Random.Decimal(500m, 100_000m))
            .RuleFor(r => r.PrazoMeses, faker => faker.Random.Int(6, 60))
            .RuleFor(r => r.TipoProduto, faker => faker.Commerce.ProductMaterial())
            .Generate();
    }

    public ConstrutorRequisicaoSimulacao ComClienteId(Guid clienteId)
    {
        _requisicao.ClienteId = clienteId;
        return this;
    }

    public ConstrutorRequisicaoSimulacao ComValor(decimal valor)
    {
        _requisicao.Valor = valor;
        return this;
    }

    public ConstrutorRequisicaoSimulacao ComPrazoMeses(int prazoMeses)
    {
        _requisicao.PrazoMeses = prazoMeses;
        return this;
    }

    public ConstrutorRequisicaoSimulacao ComTipoProduto(string tipoProduto)
    {
        _requisicao.TipoProduto = tipoProduto;
        return this;
    }

    public RequisicaoSimulacaoJson Construir()
    {
        return _requisicao;
    }
}
