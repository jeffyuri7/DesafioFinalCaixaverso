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

public class ConsultarPerfilResumidoTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/perfil-risco";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ConsultarPerfilResumidoTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_perfil_resumido_com_descricao()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var questionario = new ConstrutorQuestionarioInvestidor()
            .ComClienteId(cliente.Id)
            .ComHorizonteMeses(60)
            .ComRendaMensal(20_000m)
            .ComPatrimonioTotal(800_000m)
            .ComPreferenciaLiquidez(PreferenciaLiquidez.Baixa)
            .ComObjetivoInvestimento(ObjetivoInvestimento.Crescimento)
            .ComNivelConhecimento(NivelConhecimentoInvestidor.Avancado)
            .ComToleranciaPerdaPercentual(30m)
            .ComFonteRendaEstavel(true)
            .Construir();

        var produto = new ConstrutorProduto()
            .ComNome("CDB Performance")
            .ComRentabilidade(0.18m)
            .ComLiquidezDias(15)
            .Construir();

        var simulacoes = new[]
        {
            new ConstrutorSimulacao()
                .ComClienteId(cliente.Id)
                .ComProduto(produto)
                .ComValorInvestido(25_000m)
                .ComDataSimulacao(DateTime.UtcNow.AddDays(-10))
                .Construir(),
            new ConstrutorSimulacao()
                .ComClienteId(cliente.Id)
                .ComProduto(produto)
                .ComValorInvestido(18_000m)
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

        var resumo = await resposta.Content.ReadFromJsonAsync<PerfilClienteResumoJson>(JsonOptions);
        resumo.ShouldNotBeNull();
        resumo!.ClienteId.ShouldBe(cliente.Id);
        resumo.Pontuacao.ShouldBeGreaterThan(0);
        resumo.Descricao.ShouldBe(DescricaoEsperada(resumo.Perfil));

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var perfilPersistido = await contexto.ClientePerfis.SingleAsync();
            perfilPersistido.ClienteId.ShouldBe(cliente.Id);
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

    private static string DescricaoEsperada(string? perfil)
    {
        if (string.IsNullOrWhiteSpace(perfil))
            return "Perfil ainda não classificado.";

        return perfil.ToLowerInvariant() switch
        {
            "conservador" => "Perfil voltado para preservar o capital e garantir liquidez.",
            "moderado" => "Perfil equilibrado entre segurança e rentabilidade.",
            "agressivo" => "Perfil que aceita maior volatilidade em busca de retorno.",
            _ => "Perfil ainda não classificado."
        };
    }
}
