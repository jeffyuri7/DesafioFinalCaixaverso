using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Investimentos.SimulacoesPorProdutoDia;

public class ListarSimulacoesPorProdutoDiaTests : DesafioFinalCaixaversoClassFixture
{
    private const string Metodo = "v1/investimentos/simulacoes/por-produto-dia";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ListarSimulacoesPorProdutoDiaTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_simulacoes_agrupadas_por_produto_e_dia()
    {
        await Factory.ResetDatabaseAsync();

        var produtoCdb = new ConstrutorProduto().ComNome("CDB Corporativo").Construir();
        var produtoLci = new ConstrutorProduto().ComNome("LCI Premium").Construir();
        var diaAtual = DateTime.UtcNow.Date;
        var diaAnterior = diaAtual.AddDays(-1);

        var simulacoes = new[]
        {
            new ConstrutorSimulacao().ComProduto(produtoCdb).ComValorInvestido(5_000m).ComValorFinal(5_500m).ComDataSimulacao(diaAtual).Construir(),
            new ConstrutorSimulacao().ComProduto(produtoCdb).ComValorInvestido(7_500m).ComValorFinal(8_000m).ComDataSimulacao(diaAtual).Construir(),
            new ConstrutorSimulacao().ComProduto(produtoLci).ComValorInvestido(3_000m).ComValorFinal(3_300m).ComDataSimulacao(diaAnterior).Construir()
        };

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Produtos.AddRangeAsync(produtoCdb, produtoLci);
            await contexto.Simulacoes.AddRangeAsync(simulacoes);
        });

        var resposta = await DoGet(Metodo);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var agrupado = await resposta.Content.ReadFromJsonAsync<List<SimulacoesPorProdutoDiaJson>>(JsonOptions);
        agrupado.ShouldNotBeNull();
        var lista = agrupado!;
        lista.Count.ShouldBe(2);

    var grupoCdb = lista.First(resultado => resultado.Produto == produtoCdb.Nome);
    grupoCdb.QuantidadeSimulacoes.ShouldBe(2);
    grupoCdb.MediaValorFinal.ShouldBe(6_750m);
    grupoCdb.Data.Date.ShouldBe(diaAtual);

    var grupoLci = lista.First(resultado => resultado.Produto == produtoLci.Nome);
    grupoLci.QuantidadeSimulacoes.ShouldBe(1);
    grupoLci.MediaValorFinal.ShouldBe(3_300m);
    grupoLci.Data.Date.ShouldBe(diaAnterior);
    }

    [Fact]
    public async Task Deve_retornar_lista_vazia_quando_nao_existirem_simulacoes()
    {
        await Factory.ResetDatabaseAsync();

        var resposta = await DoGet(Metodo);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var agrupado = await resposta.Content.ReadFromJsonAsync<List<SimulacoesPorProdutoDiaJson>>(JsonOptions);
        agrupado.ShouldNotBeNull();
        agrupado!.ShouldBeEmpty();
    }
}
