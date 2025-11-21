using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.PerfilRisco;

public class ConsultarPerfilCompletoTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/perfil-risco-inicial";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ConsultarPerfilCompletoTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_snapshot_quando_registro_ja_existir()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var perfilExistente = new ClientePerfil
        {
            Id = Guid.NewGuid(),
            ClienteId = cliente.Id,
            Perfil = PerfilInvestidor.Moderado,
            Pontuacao = 68,
            ValorMedioInvestido = 10_000m,
            ValorTotalInvestido = 80_000m,
            SimulacoesUltimos30Dias = 2,
            RentabilidadeMediaProduto = 0.12m,
            LiquidezMediaDias = 15,
            PontuacaoComportamental = 40,
            PontuacaoQuestionario = 60,
            PermiteRecomendacao = true,
            MetodoCalculo = "snapshot_teste",
            Observacoes = "copiado",
            AtualizadoEm = DateTime.UtcNow.AddDays(-2),
            DadosSuficientes = true
        };

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
            await contexto.ClientePerfis.AddAsync(perfilExistente);
        });

        var token = GerarToken(cliente.Id);
        var resposta = await DoGet($"{RotaBase}/{cliente.Id}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var perfil = await resposta.Content.ReadFromJsonAsync<PerfilClienteJson>(JsonOptions);
        perfil.ShouldNotBeNull();
        perfil!.ClienteId.ShouldBe(cliente.Id);
        perfil.Pontuacao.ShouldBe(perfilExistente.Pontuacao);
        perfil.MetodoCalculo.ShouldBe(perfilExistente.MetodoCalculo);
        perfil.Observacoes.ShouldBe(perfilExistente.Observacoes);

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var registro = await contexto.ClientePerfis.SingleAsync();
            registro.Id.ShouldBe(perfilExistente.Id);
        });
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
    public async Task Deve_retornar_nao_classificado_quando_questionario_nao_existir()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var produto = new ConstrutorProduto()
            .ComNome("Fundo Crédito Privado")
            .ComRentabilidade(0.11m)
            .ComLiquidezDias(30)
            .Construir();

        var simulacao = new ConstrutorSimulacao()
            .ComClienteId(cliente.Id)
            .ComProduto(produto)
            .ComValorInvestido(35_000m)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
            await contexto.Produtos.AddAsync(produto);
            await contexto.Simulacoes.AddAsync(simulacao);
        });

        var token = GerarToken(cliente.Id);
        var resposta = await DoGet($"{RotaBase}/{cliente.Id}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var perfil = await resposta.Content.ReadFromJsonAsync<PerfilClienteJson>(JsonOptions);
        perfil.ShouldNotBeNull();
        perfil!.ClienteId.ShouldBe(cliente.Id);
        perfil.Perfil.ShouldBe(nameof(PerfilInvestidor.NaoClassificado));
        perfil.PermiteRecomendacao.ShouldBeFalse();
        perfil.MetodoCalculo.ShouldBe("motor_v2_comportamental_parcial");
        perfil.PontuacaoQuestionario.ShouldBe(0);
        perfil.PontuacaoComportamental.ShouldBeGreaterThan(0);
        perfil.DadosSuficientes.ShouldBeTrue();
        perfil.Observacoes.ShouldContain("Questionário obrigatório");

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var perfilPersistido = await contexto.ClientePerfis.SingleAsync();
            perfilPersistido.ClienteId.ShouldBe(cliente.Id);
            perfilPersistido.Perfil.ShouldBe(PerfilInvestidor.NaoClassificado);
            perfilPersistido.PermiteRecomendacao.ShouldBeFalse();
            perfilPersistido.MetodoCalculo.ShouldBe("motor_v2_comportamental_parcial");
        });
    }

    [Fact]
    public async Task Deve_retornar_sem_dados_quando_historico_e_questionario_inexistirem()
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

        var perfil = await resposta.Content.ReadFromJsonAsync<PerfilClienteJson>(JsonOptions);
        perfil.ShouldNotBeNull();
        perfil!.Perfil.ShouldBe(nameof(PerfilInvestidor.NaoClassificado));
        perfil.PermiteRecomendacao.ShouldBeFalse();
        perfil.DadosSuficientes.ShouldBeFalse();
        perfil.MetodoCalculo.ShouldBe("motor_v2_sem_dados");
        perfil.Observacoes.ShouldContain("Impossível classificar");
        perfil.Pontuacao.ShouldBe(0);
        perfil.PontuacaoComportamental.ShouldBe(0);
        perfil.PontuacaoQuestionario.ShouldBe(0);

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var perfilPersistido = await contexto.ClientePerfis.SingleAsync();
            perfilPersistido.ClienteId.ShouldBe(cliente.Id);
            perfilPersistido.Perfil.ShouldBe(PerfilInvestidor.NaoClassificado);
            perfilPersistido.PermiteRecomendacao.ShouldBeFalse();
            perfilPersistido.MetodoCalculo.ShouldBe("motor_v2_sem_dados");
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
