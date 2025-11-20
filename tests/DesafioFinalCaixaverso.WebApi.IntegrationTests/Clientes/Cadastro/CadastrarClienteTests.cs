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
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Clientes.Cadastro;

public class CadastrarClienteTests : DesafioFinalCaixaversoClassFixture
{
    private const string Rota = "v1/clientes";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public CadastrarClienteTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_cadastrar_cliente_com_sucesso()
    {
        await Factory.ResetDatabaseAsync();

        var requisicao = new ConstrutorRequisicaoCadastroCliente()
            .ComNome("Renata Souto")
            .ComEmail("renata@teste.com")
            .ComSenha("Senha@123")
            .Construir();

        var resposta = await DoPost(Rota, requisicao);

        resposta.StatusCode.ShouldBe(HttpStatusCode.Created);

        var payload = await resposta.Content.ReadFromJsonAsync<ClienteCadastradoJson>(JsonOptions);
        payload.ShouldNotBeNull();
        payload!.ClienteId.ShouldNotBe(Guid.Empty);
        payload.Nome.ShouldBe("Renata Souto");
        payload.Email.ShouldBe("renata@teste.com");

        ClientePersistidoSnapshot? clientePersistido = null;
        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var entidade = await contexto.Clientes.SingleAsync();
            clientePersistido = new ClientePersistidoSnapshot(entidade.Nome, entidade.Email, entidade.Password);
        });

        clientePersistido.ShouldNotBeNull();
        clientePersistido!.Nome.ShouldBe("Renata Souto");
        clientePersistido.Email.ShouldBe("renata@teste.com");
        clientePersistido.PasswordHash.ShouldNotBe("Senha@123");
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_email_ja_existir()
    {
        await Factory.ResetDatabaseAsync();

        const string email = "duplicado@teste.com";
        var clienteExistente = new ConstrutorCliente().ComEmail(email).Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(clienteExistente);
        });

        var requisicao = new ConstrutorRequisicaoCadastroCliente()
            .ComEmail(email)
            .Construir();

        var resposta = await DoPost(Rota, requisicao);

        resposta.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var erros = await ObterErrosAsync(resposta);
        erros.ShouldContain(MensagensDeExcecao.CLIENTE_EMAIL_JA_CADASTRADO);
    }

    private sealed record ClientePersistidoSnapshot(string Nome, string Email, string PasswordHash);

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
