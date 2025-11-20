using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Clientes.Detalhes;

public class ConsultarClienteTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/clientes";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ConsultarClienteTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_cliente_quando_existir()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        cliente.Nome = "Marina Ribeiro";
        cliente.Email = "marina@teste.com";

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
        });

        var token = GerarToken(cliente.Id);
        var resposta = await DoGet($"{RotaBase}/{cliente.Id}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await resposta.Content.ReadFromJsonAsync<ClienteCadastradoJson>(JsonOptions);
        payload.ShouldNotBeNull();
        payload!.ClienteId.ShouldBe(cliente.Id);
        payload.Nome.ShouldBe("Marina Ribeiro");
        payload.Email.ShouldBe("marina@teste.com");
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_cliente_nao_existir()
    {
        await Factory.ResetDatabaseAsync();

        var token = GerarToken(Guid.NewGuid());
        var resposta = await DoGet($"{RotaBase}/{Guid.NewGuid()}", token);

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
