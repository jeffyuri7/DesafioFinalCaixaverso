using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;

namespace DesafioFinalCaixaverso.WebApi.IntegrationTests.Investimentos.Simulacao;

public class SolicitarSimulacaoTests : DesafioFinalCaixaversoClassFixture
{
    private const string Metodo = "v1/investimentos/simular-investimento";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public SolicitarSimulacaoTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Deve_retornar_simulacoes_para_produtos_compativeis()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();

        var produtoCompativel = new ConstrutorProduto()
            .ComTipo("CDB")
            .ComMinimoInvestimento(1000m)
            .ComPrazo(12, 48)
            .Ativo(true)
            .Construir();

        var produtoIncompativel = new ConstrutorProduto()
            .ComTipo("CDB")
            .ComMinimoInvestimento(50_000m)
            .ComPrazo(48, 96)
            .Ativo(true)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
            await contexto.Produtos.AddRangeAsync(produtoCompativel, produtoIncompativel);
        });

        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComClienteId(cliente.Id)
            .ComTipoProduto(produtoCompativel.Tipo)
            .ComValor(produtoCompativel.MinimoInvestimento + 500m)
            .ComPrazoMeses(produtoCompativel.PrazoMinimoMeses + 6)
            .Construir();

        var token = GerarToken(cliente.Id);

        var resposta = await DoPost(Metodo, requisicao, token);

    resposta.StatusCode.ShouldBe(HttpStatusCode.Created);

        var simulacoes = await resposta.Content.ReadFromJsonAsync<RespostaSimulacaoJson>(JsonOptions);

    simulacoes.ShouldNotBeNull();
    simulacoes!.Simulacoes.ShouldHaveSingleItem();
        var simulacao = simulacoes.Simulacoes.Single();
    simulacao.ProdutoValidado.Nome.ShouldBe(produtoCompativel.Nome);
    simulacao.ResultadoSimulacao.ValorFinal.ShouldBeGreaterThan(requisicao.Valor);
    simulacao.ResultadoSimulacao.PrazoMeses.ShouldBe(requisicao.PrazoMeses);
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_nao_existir_produto_compativel()
    {
        await Factory.ResetDatabaseAsync();

        var cliente = new ConstrutorCliente().Construir();
        var produto = new ConstrutorProduto()
            .ComTipo("CDB")
            .ComMinimoInvestimento(10_000m)
            .ComPrazo(12, 24)
            .Ativo(true)
            .Construir();

        await Factory.ExecutarNoContextoAsync(async contexto =>
        {
            await contexto.Clientes.AddAsync(cliente);
            await contexto.Produtos.AddAsync(produto);
        });

        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComClienteId(cliente.Id)
            .ComTipoProduto(produto.Tipo)
            .ComValor(produto.MinimoInvestimento)
            .ComPrazoMeses(produto.PrazoMaximoMeses + 1)
            .Construir();

        var token = GerarToken(cliente.Id);

        var resposta = await DoPost(Metodo, requisicao, token);

    resposta.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var payload = await resposta.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        var erros = json.RootElement
            .GetProperty("erros")
            .EnumerateArray()
            .Select(elemento => elemento.GetString())
            .Where(mensagem => !string.IsNullOrWhiteSpace(mensagem))
            .Select(mensagem => mensagem!.Trim())
            .ToList();

        erros.ShouldContain(MensagensDeExcecao.PRODUTO_NAO_ENCONTRADO);
    }
}
