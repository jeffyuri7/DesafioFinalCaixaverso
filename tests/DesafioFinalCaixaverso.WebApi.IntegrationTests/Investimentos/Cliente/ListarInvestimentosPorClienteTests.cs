using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Investimentos.Cliente;

public class ListarInvestimentosPorClienteTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/investimentos";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ListarInvestimentosPorClienteTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_simulacoes_do_cliente_autenticado()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var outroCliente = new ConstrutorCliente().Construir();
        var produto = new ConstrutorProduto().ComNome("LCI Premium").Construir();

        var simulacaoDoCliente = new ConstrutorSimulacao()
            .ComClienteId(cliente.Id)
            .ComProduto(produto)
            .ComValorInvestido(8_000m)
            .Construir();

        var simulacaoOutroCliente = new ConstrutorSimulacao()
            .ComClienteId(outroCliente.Id)
            .ComProduto(produto)
            .ComValorInvestido(30_000m)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddRangeAsync(cliente, outroCliente);
            await contexto.Produtos.AddAsync(produto);
            await contexto.Simulacoes.AddRangeAsync(simulacaoDoCliente, simulacaoOutroCliente);
        });

        var token = GerarToken(cliente.Id);

        var resposta = await DoGet($"{RotaBase}/{cliente.Id}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var historico = await resposta.Content.ReadFromJsonAsync<List<HistoricoSimulacaoJson>>(JsonOptions);
        historico.ShouldNotBeNull();
        var lista = historico!;
        lista.Count.ShouldBe(1);
        lista.Single().Id.ShouldBe(simulacaoDoCliente.Id);
        lista.Single().ValorInvestido.ShouldBe(8_000m);
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_cliente_nao_existir()
    {
        await Factory.ResetDatabaseAsync();

        var clienteId = Guid.NewGuid();
        var token = GerarToken(clienteId);

    var resposta = await DoGet($"{RotaBase}/{clienteId}", token);

    resposta.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var payload = await resposta.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        var erros = json.RootElement
            .GetProperty("erros")
            .EnumerateArray()
            .Select(elemento => elemento.GetString())
            .Where(mensagem => !string.IsNullOrWhiteSpace(mensagem))
            .Select(mensagem => mensagem!.Trim())
            .ToList();

        erros.ShouldContain(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);
    }
}
