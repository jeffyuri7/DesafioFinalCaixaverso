using System;
using System.Collections.Generic;
using System.Linq;
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

public class ConsultarPerfilCompletoTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/perfil-risco-completo";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ConsultarPerfilCompletoTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_perfil_completo_e_salvar_historico()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var questionario = new ConstrutorQuestionarioInvestidor()
            .ComClienteId(cliente.Id)
            .ComPreferenciaLiquidez(PreferenciaLiquidez.Media)
            .ComObjetivoInvestimento(ObjetivoInvestimento.Equilibrio)
            .ComNivelConhecimento(NivelConhecimentoInvestidor.Intermediario)
            .ComHorizonteMeses(36)
            .ComRendaMensal(12_000m)
            .ComPatrimonioTotal(300_000m)
            .ComToleranciaPerdaPercentual(18m)
            .ComFonteRendaEstavel(true)
            .Construir();

        var produto = new ConstrutorProduto()
            .ComNome("LCI Performance")
            .ComRentabilidade(0.12m)
            .ComLiquidezDias(45)
            .Construir();

        var simulacoes = new[]
        {
            new ConstrutorSimulacao()
                .ComClienteId(cliente.Id)
                .ComProduto(produto)
                .ComValorInvestido(12_000m)
                .ComDataSimulacao(DateTime.UtcNow.AddDays(-5))
                .Construir(),
            new ConstrutorSimulacao()
                .ComClienteId(cliente.Id)
                .ComProduto(produto)
                .ComValorInvestido(9_000m)
                .ComDataSimulacao(DateTime.UtcNow.AddMonths(-3))
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

        var perfil = await resposta.Content.ReadFromJsonAsync<PerfilClienteJson>(JsonOptions);
        perfil.ShouldNotBeNull();
        perfil!.ClienteId.ShouldBe(cliente.Id);
        perfil.DadosSuficientes.ShouldBeTrue();
        perfil.PermiteRecomendacao.ShouldBeTrue();
        perfil.MetodoCalculo.ShouldBe("motor_v2_compliance");
        perfil.ValorTotalInvestido.ShouldBeGreaterThan(0m);
        perfil.Pontuacao.ShouldBeGreaterThan(0);

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var perfilPersistido = await contexto.ClientePerfis.SingleAsync();
            perfilPersistido.ClienteId.ShouldBe(cliente.Id);
            perfilPersistido.Perfil.ShouldNotBe(PerfilInvestidor.NaoClassificado);
            perfilPersistido.Pontuacao.ShouldBe(perfil.Pontuacao);
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
