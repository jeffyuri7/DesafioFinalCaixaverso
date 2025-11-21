using System;
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
    public async Task Deve_retornar_resumo_dos_dados_persistidos()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var perfilDinamico = new ClientePerfilDinamico
        {
            Id = Guid.NewGuid(),
            ClienteId = cliente.Id,
            Perfil = PerfilInvestidor.Moderado,
            Pontuacao = 72.4m,
            VolumeTotalInvestido = 50_000m,
            FrequenciaMovimentacoes = 4,
            PreferenciaLiquidez = false,
            AtualizadoEm = DateTime.UtcNow.AddDays(-1)
        };

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
            await contexto.ClientePerfisDinamicos.AddAsync(perfilDinamico);
        });

        var token = GerarToken(cliente.Id);
        var resposta = await DoGet($"{RotaBase}/{cliente.Id}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var perfil = await resposta.Content.ReadFromJsonAsync<PerfilClienteResumoJson>(JsonOptions);
        perfil.ShouldNotBeNull();
        perfil!.ClienteId.ShouldBe(cliente.Id);
        perfil.Perfil.ShouldBe(nameof(PerfilInvestidor.Moderado));
        perfil.Pontuacao.ShouldBe((int)Math.Round(perfilDinamico.Pontuacao, MidpointRounding.AwayFromZero));
        perfil.Descricao.ShouldBe("Equilibra segurança e rentabilidade, aceitando riscos moderados para alcançar ganhos consistentes.");

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var perfilPersistido = await contexto.ClientePerfisDinamicos.SingleAsync();
            perfilPersistido.ClienteId.ShouldBe(cliente.Id);
            perfilPersistido.Pontuacao.ShouldBe(perfilDinamico.Pontuacao);
            perfilPersistido.AtualizadoEm.ShouldBe(perfilDinamico.AtualizadoEm);
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
