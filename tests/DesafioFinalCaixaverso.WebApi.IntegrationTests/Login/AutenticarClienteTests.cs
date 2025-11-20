using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Seguranca;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Login;

public class AutenticarClienteTests : DesafioFinalCaixaversoClassFixture
{
    private const string Rota = "v1/login";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public AutenticarClienteTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_autenticar_cliente_existente()
    {
        await Factory.ResetDatabaseAsync();

        const string email = "cliente@login.com";
        const string senha = "Senha@123";

        string senhaHash;
        using (var scope = Factory.Services.CreateScope())
        {
            var hashService = scope.ServiceProvider.GetRequiredService<IServicoHashSenha>();
            senhaHash = hashService.Gerar(senha);
        }

        var cliente = new ConstrutorCliente()
            .ComEmail(email)
            .ComSenhaHash(senhaHash)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
        });

        var requisicao = new ConstrutorRequisicaoLoginCliente()
            .ComEmail(email)
            .ComSenha(senha)
            .Construir();

        var resposta = await DoPost(Rota, requisicao);

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await resposta.Content.ReadFromJsonAsync<ClienteAutenticadoJson>(JsonOptions);
        payload.ShouldNotBeNull();
        payload!.ClienteId.ShouldBe(cliente.Id);
        payload.Token.ShouldNotBeNull();
        payload.Token.Valor.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_credenciais_forem_invalidas()
    {
        await Factory.ResetDatabaseAsync();

        const string email = "falha@login.com";
        const string senhaCerta = "SenhaCorreta@123";

        string senhaHash;
        using (var scope = Factory.Services.CreateScope())
        {
            var hashService = scope.ServiceProvider.GetRequiredService<IServicoHashSenha>();
            senhaHash = hashService.Gerar(senhaCerta);
        }

        var cliente = new ConstrutorCliente()
            .ComEmail(email)
            .ComSenhaHash(senhaHash)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
        });

        var requisicao = new ConstrutorRequisicaoLoginCliente()
            .ComEmail(email)
            .ComSenha("senha-invalida")
            .Construir();

        var resposta = await DoPost(Rota, requisicao);

        resposta.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var erros = await ObterErrosAsync(resposta);
        erros.ShouldContain(MensagensDeExcecao.AUTENTICACAO_CREDENCIAIS_INVALIDAS);
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
