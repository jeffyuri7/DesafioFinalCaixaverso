using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Clientes.Questionario;

public class ResponderQuestionarioTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/clientes";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ResponderQuestionarioTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_registrar_questionario_para_cliente()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
        });

        var token = GerarToken(cliente.Id);
        var requisicao = new ConstrutorRequisicaoQuestionarioCliente()
            .ComPreferenciaLiquidez(PreferenciaLiquidez.Baixa)
            .ComObjetivoInvestimento(ObjetivoInvestimento.Crescimento)
            .ComNivelConhecimento(NivelConhecimentoInvestidor.Avancado)
            .ComHorizonte(48)
            .ComRenda(15_000m)
            .ComPatrimonio(450_000m)
            .ComTolerancia(25m)
            .ComFonteRendaEstavel(true)
            .Construir();

        var resposta = await DoPost($"{RotaBase}/{cliente.Id}/questionario", requisicao, token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await resposta.Content.ReadFromJsonAsync<QuestionarioRespondidoJson>(JsonOptions);
        payload.ShouldNotBeNull();
        payload!.ClienteId.ShouldBe(cliente.Id);
        payload.AtualizadoEm.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-5));

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var questionario = await contexto.QuestionariosInvestidor.SingleAsync();
            questionario.ClienteId.ShouldBe(cliente.Id);
            questionario.PreferenciaLiquidez.ShouldBe(PreferenciaLiquidez.Baixa);
            questionario.ObjetivoInvestimento.ShouldBe(ObjetivoInvestimento.Crescimento);
            questionario.NivelConhecimento.ShouldBe(NivelConhecimentoInvestidor.Avancado);
            questionario.HorizonteMeses.ShouldBe(48);
            questionario.RendaMensal.ShouldBe(15_000m);
            questionario.PatrimonioTotal.ShouldBe(450_000m);
            questionario.ToleranciaPerdaPercentual.ShouldBe(25m);
            questionario.FonteRendaEstavel.ShouldBeTrue();

            var perfil = await contexto.ClientePerfis.SingleAsync();
            perfil.ClienteId.ShouldBe(cliente.Id);
            perfil.PontuacaoQuestionario.ShouldBeGreaterThan(0);
        });
    }

    [Fact]
    public async Task Deve_atualizar_questionario_existente()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var questionarioExistente = new ConstrutorQuestionarioInvestidor()
            .ComClienteId(cliente.Id)
            .ComPreferenciaLiquidez(PreferenciaLiquidez.Media)
            .ComObjetivoInvestimento(ObjetivoInvestimento.Equilibrio)
            .ComNivelConhecimento(NivelConhecimentoInvestidor.Intermediario)
            .ComHorizonteMeses(12)
            .ComRendaMensal(5_000m)
            .ComPatrimonioTotal(80_000m)
            .ComToleranciaPerdaPercentual(5m)
            .ComFonteRendaEstavel(false)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
            await contexto.QuestionariosInvestidor.AddAsync(questionarioExistente);
        });

        var token = GerarToken(cliente.Id);
        var requisicao = new RequisicaoQuestionarioClienteJson
        {
            PreferenciaLiquidez = PreferenciaLiquidez.Alta,
            ObjetivoInvestimento = ObjetivoInvestimento.Renda,
            NivelConhecimento = NivelConhecimentoInvestidor.Iniciante,
            HorizonteMeses = 6,
            RendaMensal = 7_500m,
            PatrimonioTotal = 120_000m,
            ToleranciaPerdaPercentual = 12m,
            FonteRendaEstavel = true
        };

        var resposta = await DoPost($"{RotaBase}/{cliente.Id}/questionario", requisicao, token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var questionario = await contexto.QuestionariosInvestidor.SingleAsync();
            questionario.Id.ShouldBe(questionarioExistente.Id);
            questionario.PreferenciaLiquidez.ShouldBe(PreferenciaLiquidez.Alta);
            questionario.ObjetivoInvestimento.ShouldBe(ObjetivoInvestimento.Renda);
            questionario.NivelConhecimento.ShouldBe(NivelConhecimentoInvestidor.Iniciante);
            questionario.HorizonteMeses.ShouldBe(6);
            questionario.RendaMensal.ShouldBe(7_500m);
            questionario.PatrimonioTotal.ShouldBe(120_000m);
            questionario.ToleranciaPerdaPercentual.ShouldBe(12m);
            questionario.FonteRendaEstavel.ShouldBeTrue();

            var perfil = await contexto.ClientePerfis.SingleAsync();
            perfil.ClienteId.ShouldBe(cliente.Id);
            perfil.AtualizadoEm.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-5));
        });
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_cliente_nao_existir()
    {
        await Factory.ResetDatabaseAsync();

        var token = GerarToken(Guid.NewGuid());
        var requisicao = new ConstrutorRequisicaoQuestionarioCliente().Construir();

        var resposta = await DoPost($"{RotaBase}/{Guid.NewGuid()}/questionario", requisicao, token);

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
