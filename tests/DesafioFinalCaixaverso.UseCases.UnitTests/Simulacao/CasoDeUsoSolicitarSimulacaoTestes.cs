using System;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Simulacao;

public class CasoDeUsoSolicitarSimulacaoTestes
{
    [Fact]
    public async Task Deve_lancar_excecao_quando_cliente_nao_existir()
    {
        var clienteRepositorio = new ClienteRepositorioFalso().SemCliente();
        var produtoRepositorio = new ProdutoRepositorioFalso();
        var simulacaoRepositorio = new SimulacaoRepositorioFalso();
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var casoDeUso = CriarCasoDeUso(clienteRepositorio, produtoRepositorio, simulacaoRepositorio, unidadeDeTrabalho);
        var requisicao = new ConstrutorRequisicaoSimulacao().Construir();

    await Should.ThrowAsync<NaoEncontradoException>(() => casoDeUso.Executar(requisicao));
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_produto_nao_foi_encontrado()
    {
        var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente Teste", Email = "teste@teste.com" };
        var clienteRepositorio = new ClienteRepositorioFalso().ComClienteExistente(cliente);
        var produtoRepositorio = new ProdutoRepositorioFalso();
        var simulacaoRepositorio = new SimulacaoRepositorioFalso();
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var casoDeUso = CriarCasoDeUso(clienteRepositorio, produtoRepositorio, simulacaoRepositorio, unidadeDeTrabalho);

        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComClienteId(cliente.Id)
            .ComTipoProduto("CDB")
            .Construir();

    await Should.ThrowAsync<NaoEncontradoException>(() => casoDeUso.Executar(requisicao));
    }

    [Fact]
    public async Task Deve_criar_simulacao_quando_dados_forem_validos()
    {
        var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente Teste", Email = "teste@teste.com" };
        var clienteRepositorio = new ClienteRepositorioFalso().ComClienteExistente(cliente);

        var produto = new ConstrutorProduto()
            .ComTipo("CDB")
            .ComMinimoInvestimento(500)
            .ComPrazo(6, 48)
            .ComRentabilidade(0.15m)
            .Ativo(true)
            .Construir();

        var produtoRepositorio = new ProdutoRepositorioFalso().ComProdutos(new[] { produto });
        var simulacaoRepositorio = new SimulacaoRepositorioFalso();
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var casoDeUso = CriarCasoDeUso(clienteRepositorio, produtoRepositorio, simulacaoRepositorio, unidadeDeTrabalho);

        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComClienteId(cliente.Id)
            .ComValor(2_000m)
            .ComPrazoMeses(12)
            .ComTipoProduto(produto.Tipo)
            .Construir();

        var resposta = await casoDeUso.Executar(requisicao);

    resposta.ShouldNotBeNull();
    resposta.ProdutoValidado.Id.ShouldBe(produto.Id);

    var simulacaoPersistida = simulacaoRepositorio.UltimaSimulacaoAdicionada;
    simulacaoPersistida.ShouldNotBeNull();
    simulacaoPersistida!.ClienteId.ShouldBe(cliente.Id);

    unidadeDeTrabalho.CommitFoiChamado.ShouldBeTrue();
    }

    private static CasoDeUsoSolicitarSimulacao CriarCasoDeUso(
        ClienteRepositorioFalso clienteRepositorio,
        ProdutoRepositorioFalso produtoRepositorio,
        SimulacaoRepositorioFalso simulacaoRepositorio,
        UnidadeDeTrabalhoFalsa unidadeDeTrabalho)
    {
        return new CasoDeUsoSolicitarSimulacao(
            clienteRepositorio,
            produtoRepositorio,
            simulacaoRepositorio,
            unidadeDeTrabalho);
    }
}
