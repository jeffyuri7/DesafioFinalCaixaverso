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
using DesafioFinalCaixaverso.Dominio.Seguranca;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Clientes.Atualizacao;

public class AtualizarClienteTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/clientes";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public AtualizarClienteTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_atualizar_cliente_com_sucesso()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        cliente.Nome = "Joana Viana";
        cliente.Email = "joana@teste.com";

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
        });

        var token = GerarToken(cliente.Id);
        var requisicao = new RequisicaoAtualizacaoClienteJson
        {
            Nome = "Joana Silva",
            Email = "joana.silva@teste.com",
            Senha = "NovaSenha@123"
        };

        var resposta = await DoPut($"{RotaBase}/{cliente.Id}", requisicao, token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await resposta.Content.ReadFromJsonAsync<ClienteCadastradoJson>(JsonOptions);
        payload.ShouldNotBeNull();
        payload!.Nome.ShouldBe("Joana Silva");
        payload.Email.ShouldBe("joana.silva@teste.com");

        string senhaEsperada;
        using (var scope = Factory.Services.CreateScope())
        {
            var hashService = scope.ServiceProvider.GetRequiredService<IServicoHashSenha>();
            senhaEsperada = hashService.Gerar(requisicao.Senha!);
        }

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var atualizado = await contexto.Clientes.SingleAsync();
            atualizado.Nome.ShouldBe("Joana Silva");
            atualizado.Email.ShouldBe("joana.silva@teste.com");
            atualizado.Password.ShouldBe(senhaEsperada);
        });
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_cliente_nao_existir()
    {
        await Factory.ResetDatabaseAsync();

        var token = GerarToken(Guid.NewGuid());
        var requisicao = new RequisicaoAtualizacaoClienteJson
        {
            Nome = "Cliente Inexistente",
            Email = "naoexiste@teste.com",
            Senha = "Senha@123"
        };

        var resposta = await DoPut($"{RotaBase}/{Guid.NewGuid()}", requisicao, token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var erros = await ObterErrosAsync(resposta);
        erros.ShouldContain(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_email_ja_utilizado_por_outro_cliente()
    {
        await Factory.ResetDatabaseAsync();

        var clientePrincipal = new ConstrutorCliente().Construir();
        clientePrincipal.Nome = "Cliente Principal";
        clientePrincipal.Email = "principal@teste.com";

        var clienteComEmail = new ConstrutorCliente().Construir();
        clienteComEmail.Nome = "Outro Cliente";
        clienteComEmail.Email = "email@teste.com";

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddRangeAsync(clientePrincipal, clienteComEmail);
        });

        var token = GerarToken(clientePrincipal.Id);
        var requisicao = new RequisicaoAtualizacaoClienteJson
        {
            Nome = "Cliente Principal",
            Email = "email@teste.com"
        };

        var resposta = await DoPut($"{RotaBase}/{clientePrincipal.Id}", requisicao, token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var erros = await ObterErrosAsync(resposta);
        erros.ShouldContain(MensagensDeExcecao.CLIENTE_EMAIL_JA_CADASTRADO);
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
