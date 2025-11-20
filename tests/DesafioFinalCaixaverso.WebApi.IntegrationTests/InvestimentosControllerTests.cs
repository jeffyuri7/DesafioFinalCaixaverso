using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests;

public class InvestimentosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public InvestimentosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_retornar_historico_de_simulacoes()
    {
        await _factory.ResetDatabaseAsync();

        var produto = new ConstrutorProduto().ComNome("CDB Premium").Construir();
        var dataRecente = DateTime.UtcNow.Date;
        var dataAntiga = dataRecente.AddDays(-2);

        var simulacaoRecente = new ConstrutorSimulacao()
            .ComProduto(produto)
            .ComValorInvestido(10_000m)
            .ComDataSimulacao(dataRecente)
            .Construir();

        var simulacaoAntiga = new ConstrutorSimulacao()
            .ComProduto(produto)
            .ComValorInvestido(2_500m)
            .ComDataSimulacao(dataAntiga)
            .Construir();

        await _factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Produtos.AddAsync(produto);
            await contexto.Simulacoes.AddRangeAsync(simulacaoRecente, simulacaoAntiga);
        });

        var resposta = await _client.GetAsync("v1/investimentos/simulacoes");

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var historico = await resposta.Content.ReadFromJsonAsync<List<HistoricoSimulacaoJson>>(JsonOptions);
        historico.ShouldNotBeNull();
        historico.Count.ShouldBe(2);
        historico.First().Id.ShouldBe(simulacaoRecente.Id);
        historico.First().ValorInvestido.ShouldBe(10_000m);
        historico.Last().ProdutoNome.ShouldBe(produto.Nome);
        historico.Last().ValorInvestido.ShouldBe(2_500m);
    }

    [Fact]
    public async Task Deve_retornar_simulacoes_agrupadas_por_produto_e_dia()
    {
        await _factory.ResetDatabaseAsync();

        var produtoCdb = new ConstrutorProduto().ComNome("CDB Corporativo").Construir();
        var produtoLci = new ConstrutorProduto().ComNome("LCI Premium").Construir();
        var diaAtual = DateTime.UtcNow.Date;
        var diaAnterior = diaAtual.AddDays(-1);

        var simulacoes = new[]
        {
            new ConstrutorSimulacao().ComProduto(produtoCdb).ComValorInvestido(5_000m).ComDataSimulacao(diaAtual).Construir(),
            new ConstrutorSimulacao().ComProduto(produtoCdb).ComValorInvestido(7_500m).ComDataSimulacao(diaAtual).Construir(),
            new ConstrutorSimulacao().ComProduto(produtoLci).ComValorInvestido(3_000m).ComDataSimulacao(diaAnterior).Construir()
        };

        await _factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Produtos.AddRangeAsync(produtoCdb, produtoLci);
            await contexto.Simulacoes.AddRangeAsync(simulacoes);
        });

        using (var scope = _factory.Services.CreateScope())
        {
            var repositorio = scope.ServiceProvider.GetRequiredService<ISimulacaoRepositorio>();
            var resultados = await repositorio.ListarAgrupadoPorProdutoEDiaAsync();
            resultados.Count.ShouldBe(2);
        }

        var resposta = await _client.GetAsync("v1/investimentos/simulacoes/por-produto-dia");

    var conteudo = await resposta.Content.ReadAsStringAsync();
    resposta.StatusCode.ShouldBe(HttpStatusCode.OK, conteudo);

    var agrupado = JsonSerializer.Deserialize<List<SimulacoesPorProdutoDiaJson>>(conteudo, JsonOptions);
        agrupado.ShouldNotBeNull();
        agrupado.Count.ShouldBe(2);

        var grupoCdb = agrupado.First(resultado => resultado.Produto == produtoCdb.Nome);
        grupoCdb.Quantidade.ShouldBe(2);
        grupoCdb.ValorTotalInvestido.ShouldBe(12_500m);
        grupoCdb.Dia.Date.ShouldBe(diaAtual);

        var grupoLci = agrupado.First(resultado => resultado.Produto == produtoLci.Nome);
        grupoLci.Quantidade.ShouldBe(1);
        grupoLci.ValorTotalInvestido.ShouldBe(3_000m);
        grupoLci.Dia.Date.ShouldBe(diaAnterior);
    }

    [Fact]
    public async Task Deve_retornar_lista_vazia_quando_nao_existir_simulacoes()
    {
        await _factory.ResetDatabaseAsync();

        var resposta = await _client.GetAsync("v1/investimentos/simulacoes/por-produto-dia");

        var conteudo = await resposta.Content.ReadAsStringAsync();
        resposta.StatusCode.ShouldBe(HttpStatusCode.OK, conteudo);

        var agrupado = JsonSerializer.Deserialize<List<SimulacoesPorProdutoDiaJson>>(conteudo, JsonOptions);
        agrupado.ShouldNotBeNull();
        agrupado.ShouldBeEmpty();
    }
}
