using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.ProdutosRecomendados;

public class ListarProdutosRecomendadosTests : DesafioFinalCaixaversoClassFixture
{
    private const string RotaBase = "v1/produtos-recomendados";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };
    private static readonly string[] ProdutosConservadoresOrdenados = { "CDB Liquidez Diária", "Tesouro IPCA 2030" };

    public ListarProdutosRecomendadosTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_produtos_conservadores_em_ordem_alfabetica()
    {
        await Factory.ResetDatabaseAsync();

        var produtoCdb = new ConstrutorProduto()
            .ComNome("CDB Liquidez Diária")
            .ComRisco(Risco.Baixo)
            .Ativo(true)
            .Construir();

        var produtoTesouro = new ConstrutorProduto()
            .ComNome("Tesouro IPCA 2030")
            .ComRisco(Risco.Baixo)
            .Ativo(true)
            .Construir();

        var produtoModerado = new ConstrutorProduto()
            .ComNome("Fundo Multimercado")
            .ComRisco(Risco.Medio)
            .Ativo(true)
            .Construir();

        var produtoInativo = new ConstrutorProduto()
            .ComNome("CDB Empresarial")
            .ComRisco(Risco.Baixo)
            .Ativo(false)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Produtos.AddRangeAsync(produtoCdb, produtoTesouro, produtoModerado, produtoInativo);
        });

        var resposta = await DoGet($"{RotaBase}/{(int)PerfilInvestidor.Conservador}");

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var produtos = await resposta.Content.ReadFromJsonAsync<List<ProdutoRecomendadoJson>>(JsonOptions);
        produtos.ShouldNotBeNull();
        produtos!.Count.ShouldBe(2);
        produtos[0].Nome.ShouldBe("CDB Liquidez Diária");
        produtos[0].Risco.ShouldBe("Baixo");
    produtos[1].Nome.ShouldBe("Tesouro IPCA 2030");
    produtos[1].Risco.ShouldBe("Baixo");
    produtos.Select(p => p.Nome).ShouldBe(ProdutosConservadoresOrdenados);
    }

    [Fact]
    public async Task Deve_retornar_lista_vazia_para_perfil_nao_classificado()
    {
        await Factory.ResetDatabaseAsync();

        var resposta = await DoGet($"{RotaBase}/{(int)PerfilInvestidor.NaoClassificado}");

        resposta.StatusCode.ShouldBe(HttpStatusCode.OK);

        var produtos = await resposta.Content.ReadFromJsonAsync<List<ProdutoRecomendadoJson>>(JsonOptions);
        produtos.ShouldNotBeNull();
        produtos!.ShouldBeEmpty();
    }
}
