using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Telemetria;

public class TelemetriaControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TelemetriaControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_retornar_resumo_do_mes_atual()
    {
        await _factory.ResetDatabaseAsync();

        var agora = DateTime.UtcNow;
        var anoAtual = agora.Year;
        var mesAtual = agora.Month;
        var mesAnterior = mesAtual == 1 ? 12 : mesAtual - 1;
        var anoMesAnterior = mesAtual == 1 ? anoAtual - 1 : anoAtual;

        var servicoSimulacao = new ConstrutorTelemetriaServico()
            .ComServico("investimentos/simular")
            .ComAno(anoAtual)
            .ComMes(mesAtual)
            .ComQuantidade(15)
            .ComTempoTotal(3500)
            .ComUltimaChamada(agora.AddMinutes(-5))
            .Construir();

        var servicoDashboard = new ConstrutorTelemetriaServico()
            .ComServico("dashboard")
            .ComAno(anoAtual)
            .ComMes(mesAtual)
            .ComQuantidade(6)
            .ComTempoTotal(600)
            .ComUltimaChamada(agora.AddMinutes(-15))
            .Construir();

        var servicoPerfil = new ConstrutorTelemetriaServico()
            .ComServico("perfil-risco")
            .ComAno(anoAtual)
            .ComMes(mesAtual)
            .ComQuantidade(6)
            .ComTempoTotal(900)
            .ComUltimaChamada(agora.AddMinutes(-1))
            .Construir();

        var servicoOutroMes = new ConstrutorTelemetriaServico()
            .ComServico("servico-antigo")
            .ComAno(anoMesAnterior)
            .ComMes(mesAnterior)
            .ComQuantidade(50)
            .ComTempoTotal(10_000)
            .ComUltimaChamada(agora.AddMonths(-1))
            .Construir();

        await _factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.TelemetriaServicos.AddRangeAsync(servicoSimulacao, servicoDashboard, servicoPerfil, servicoOutroMes);
        });

        var resposta = await _client.GetAsync("v1/telemetria");

        var conteudo = await resposta.Content.ReadAsStringAsync();
        resposta.StatusCode.ShouldBe(HttpStatusCode.OK, conteudo);

        var resumo = JsonSerializer.Deserialize<TelemetriaResumoJson>(conteudo, JsonOptions);
        resumo.ShouldNotBeNull();
        resumo.Servicos.Count.ShouldBe(3);

        var inicioEsperado = new DateTime(anoAtual, mesAtual, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimEsperado = inicioEsperado.AddMonths(1).AddTicks(-1);
        resumo.Periodo.Inicio.ShouldBe(inicioEsperado);
        resumo.Periodo.Fim.ShouldBe(fimEsperado);

        resumo.Servicos[0].Nome.ShouldBe("investimentos/simular");
        resumo.Servicos[0].QuantidadeChamadas.ShouldBe(15);
        resumo.Servicos[0].MediaTempoRespostaMs.ShouldBe(Math.Round(3500d / 15, 2, MidpointRounding.AwayFromZero));

        resumo.Servicos[1].Nome.ShouldBe("dashboard");
        resumo.Servicos[1].QuantidadeChamadas.ShouldBe(6);
        resumo.Servicos[1].MediaTempoRespostaMs.ShouldBe(Math.Round(600d / 6, 2, MidpointRounding.AwayFromZero));

        resumo.Servicos[2].Nome.ShouldBe("perfil-risco");
        resumo.Servicos[2].QuantidadeChamadas.ShouldBe(6);
        resumo.Servicos[2].MediaTempoRespostaMs.ShouldBe(Math.Round(900d / 6, 2, MidpointRounding.AwayFromZero));
    }

    [Fact]
    public async Task Deve_retornar_lista_vazia_quando_nao_existir_registros()
    {
        await _factory.ResetDatabaseAsync();

        var resposta = await _client.GetAsync("v1/telemetria");

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var resumo = await resposta.Content.ReadFromJsonAsync<TelemetriaResumoJson>(JsonOptions);
        resumo.ShouldNotBeNull();
        resumo.Servicos.ShouldNotBeNull();
        resumo.Servicos.ShouldBeEmpty();
    }
}
