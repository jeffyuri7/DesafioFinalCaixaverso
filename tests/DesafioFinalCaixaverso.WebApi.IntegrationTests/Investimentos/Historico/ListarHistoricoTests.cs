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

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Investimentos.Historico;

public class ListarHistoricoTests : DesafioFinalCaixaversoClassFixture
{
    private const string Metodo = "v1/investimentos/simulacoes";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ListarHistoricoTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_historico_em_ordem_decrescente()
    {
        await Factory.ResetDatabaseAsync();

        var produto = new ConstrutorProduto().ComNome("CDB Premium").Construir();
        var dataMaisRecente = DateTime.UtcNow.Date;
        var dataAntiga = dataMaisRecente.AddDays(-2);

        var simulacaoRecente = new ConstrutorSimulacao()
            .ComProduto(produto)
            .ComValorInvestido(10_000m)
            .ComDataSimulacao(dataMaisRecente)
            .Construir();

        var simulacaoAntiga = new ConstrutorSimulacao()
            .ComProduto(produto)
            .ComValorInvestido(2_500m)
            .ComDataSimulacao(dataAntiga)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Produtos.AddAsync(produto);
            await contexto.Simulacoes.AddRangeAsync(simulacaoRecente, simulacaoAntiga);
        });

        var resposta = await DoGet(Metodo);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var historico = await resposta.Content.ReadFromJsonAsync<List<HistoricoSimulacaoJson>>(JsonOptions);
        historico.ShouldNotBeNull();
        var lista = historico!;
        lista.Count.ShouldBe(2);
    lista.First().Id.ShouldBe(simulacaoRecente.Id);
    lista.First().ValorInvestido.ShouldBe(10_000m);
    lista.Last().Produto.ShouldBe(produto.Nome);
    lista.Last().ValorInvestido.ShouldBe(2_500m);
    }

    [Fact]
    public async Task Deve_retornar_lista_vazia_quando_nao_houver_simulacoes()
    {
        await Factory.ResetDatabaseAsync();

        var resposta = await DoGet(Metodo);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var historico = await resposta.Content.ReadFromJsonAsync<List<HistoricoSimulacaoJson>>(JsonOptions);
        historico.ShouldNotBeNull();
        historico!.ShouldBeEmpty();
    }
}
