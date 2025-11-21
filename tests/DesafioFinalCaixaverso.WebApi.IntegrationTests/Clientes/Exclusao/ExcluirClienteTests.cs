using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Clientes.Exclusao;

public class ExcluirClienteTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/clientes";

    public ExcluirClienteTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_remover_cliente_quando_existir()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
            await contexto.ClientePerfisDinamicos.AddAsync(new ClientePerfilDinamico
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Perfil = PerfilInvestidor.Conservador,
                Pontuacao = 0,
                VolumeTotalInvestido = 0,
                FrequenciaMovimentacoes = 0,
                PreferenciaLiquidez = true,
                AtualizadoEm = DateTime.UtcNow
            });
        });

        var token = GerarToken(cliente.Id);
        var resposta = await DoDelete($"{RotaBase}/{cliente.Id}", token);

        resposta.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            var existeCliente = await contexto.Clientes.AnyAsync();
            existeCliente.ShouldBeFalse();

            var perfilDinamicoExiste = await contexto.ClientePerfisDinamicos.AnyAsync();
            perfilDinamicoExiste.ShouldBeFalse();
        });
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_cliente_nao_for_encontrado()
    {
        await Factory.ResetDatabaseAsync();

        var token = GerarToken(Guid.NewGuid());
        var resposta = await DoDelete($"{RotaBase}/{Guid.NewGuid()}", token);

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
