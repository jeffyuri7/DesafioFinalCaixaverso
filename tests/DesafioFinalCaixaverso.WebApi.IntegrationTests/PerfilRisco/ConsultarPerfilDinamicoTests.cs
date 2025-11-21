using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.PerfilRisco;

public class ConsultarPerfilDinamicoTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/perfil-risco";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ConsultarPerfilDinamicoTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_perfil_dinamico_para_cliente_com_simulacoes()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var questionario = new ConstrutorQuestionarioInvestidor()
            .ComClienteId(cliente.Id)
            .ComHorizonteMeses(48)
            .ComRendaMensal(10_000m)
            .ComPatrimonioTotal(400_000m)
            .ComPreferenciaLiquidez(PreferenciaLiquidez.Media)
            .ComObjetivoInvestimento(ObjetivoInvestimento.Crescimento)
            .ComNivelConhecimento(NivelConhecimentoInvestidor.Intermediario)
            .ComToleranciaPerdaPercentual(20m)
            .ComFonteRendaEstavel(true)
            .Construir();

        var produto = new ConstrutorProduto()
            .ComNome("CDB Performance Dinamico")
            .ComRentabilidade(0.16m)
            .ComLiquidezDias(20)
            .Construir();

        var simulacoes = new[]
        {
            new ConstrutorSimulacao()
                .ComClienteId(cliente.Id)
                .ComProduto(produto)
                .ComValorInvestido(30_000m)
                .ComDataSimulacao(DateTime.UtcNow.AddDays(-7))
                .Construir(),
            new ConstrutorSimulacao()
                .ComClienteId(cliente.Id)
                .ComProduto(produto)
                .ComValorInvestido(15_000m)
                .ComDataSimulacao(DateTime.UtcNow.AddDays(-40))
                .Construir()
        };

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
            await contexto.Produtos.AddAsync(produto);
            await contexto.QuestionariosInvestidor.AddAsync(questionario);
            await contexto.Simulacoes.AddRangeAsync(simulacoes);
        });

        var token = GerarToken(cliente.Id);
        var resposta = await DoGet($"{RotaBase}/{cliente.Id}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

    var perfil = await resposta.Content.ReadFromJsonAsync<PerfilClienteResumoJson>(JsonOptions);
    perfil.ShouldNotBeNull();
    perfil!.ClienteId.ShouldBe(cliente.Id);
    perfil.Pontuacao.ShouldBeGreaterThan(0);
    perfil.Perfil.ShouldNotBeNullOrWhiteSpace();
    perfil.Descricao.ShouldNotBeNullOrWhiteSpace();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var perfilPersistido = await contexto.ClientePerfisDinamicos.SingleAsync();
            perfilPersistido.ClienteId.ShouldBe(cliente.Id);
            perfilPersistido.Pontuacao.ShouldBeGreaterThan(0);
        });
    }

    [Fact]
    public async Task Deve_retornar_perfil_padrao_quando_nao_existirem_simulacoes()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
        });

        var token = GerarToken(cliente.Id);
        var resposta = await DoGet($"{RotaBase}/{cliente.Id}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var perfil = await resposta.Content.ReadFromJsonAsync<PerfilClienteResumoJson>(JsonOptions);
        perfil.ShouldNotBeNull();
        perfil!.Pontuacao.ShouldBe(0);
        perfil.Perfil.ShouldBe(nameof(PerfilInvestidor.NaoClassificado));
        perfil.Descricao.ShouldNotBeNullOrWhiteSpace();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            (await contexto.ClientePerfisDinamicos.CountAsync()).ShouldBe(0);
        });
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_cliente_nao_existir()
    {
        await Factory.ResetDatabaseAsync();

        var clienteId = Guid.NewGuid();
        var token = GerarToken(clienteId);

        var resposta = await DoGet($"{RotaBase}/{clienteId}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var erros = await ObterErrosAsync(resposta);
        erros.ShouldContain(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);
    }

    private static async Task<List<string>> ObterErrosAsync(HttpResponseMessage resposta)
    {
        var payload = await resposta.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        return json.RootElement
            .GetProperty("erros")
            .EnumerateArray()
            .Select(elemento => elemento.GetString())
            .Where(mensagem => !string.IsNullOrWhiteSpace(mensagem))
            .Select(mensagem => mensagem!.Trim())
            .ToList();
    }
}
