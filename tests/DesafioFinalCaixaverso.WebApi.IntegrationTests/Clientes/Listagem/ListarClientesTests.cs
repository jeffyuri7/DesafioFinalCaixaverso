using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Clientes.Listagem;

public class ListarClientesTests : DesafioFinalCaixaversoClassFixture
{
    private const string Rota = "v1/clientes";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public ListarClientesTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_clientes_ordenados_por_nome()
    {
        await Factory.ResetDatabaseAsync();

        var clienteAna = new ConstrutorCliente().Construir();
        clienteAna.Nome = "Ana Lira";
        clienteAna.Email = "ana@teste.com";

        var clienteBruno = new ConstrutorCliente().Construir();
        clienteBruno.Nome = "Bruno Dias";
        clienteBruno.Email = "bruno@teste.com";

        var clienteCarlos = new ConstrutorCliente().Construir();
        clienteCarlos.Nome = "Carlos Souza";
        clienteCarlos.Email = "carlos@teste.com";

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddRangeAsync(clienteAna, clienteBruno, clienteCarlos);
        });

        var token = GerarToken(clienteAna.Id);
        var resposta = await DoGet(Rota, token);

        var payload = await resposta.Content.ReadAsStringAsync();
        resposta.StatusCode.ShouldBe(HttpStatusCode.OK, payload);

        var clientes = JsonSerializer.Deserialize<List<ClienteCadastradoJson>>(payload, JsonOptions);
        clientes.ShouldNotBeNull();
        clientes!.Count.ShouldBe(3);
        clientes.Select(cliente => cliente.Nome).ShouldBe(new[] { "Ana Lira", "Bruno Dias", "Carlos Souza" });
        clientes.First().ClienteId.ShouldBe(clienteAna.Id);
    }
}
