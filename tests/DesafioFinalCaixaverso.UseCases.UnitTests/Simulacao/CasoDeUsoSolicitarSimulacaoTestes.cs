using System;
using System.Linq;
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
    public async Task Deve_criar_simulacoes_para_todos_produtos_compativeis()
    {
        var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente Teste", Email = "teste@teste.com" };
        var clienteRepositorio = new ClienteRepositorioFalso().ComClienteExistente(cliente);

        var produtoCdb = new ConstrutorProduto()
            .ComTipo("CDB")
            .ComMinimoInvestimento(500)
            .ComPrazo(6, 48)
            .ComRentabilidade(0.15m)
            .Ativo(true)
            .Construir();

        var produtoLci = new ConstrutorProduto()
            .ComTipo("CDB")
            .ComMinimoInvestimento(500)
            .ComPrazo(6, 60)
            .ComRentabilidade(0.12m)
            .Ativo(true)
            .Construir();

        var produtoRepositorio = new ProdutoRepositorioFalso().ComProdutos(new[] { produtoCdb, produtoLci });
        var simulacaoRepositorio = new SimulacaoRepositorioFalso();
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var casoDeUso = CriarCasoDeUso(clienteRepositorio, produtoRepositorio, simulacaoRepositorio, unidadeDeTrabalho);

        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComClienteId(cliente.Id)
            .ComValor(2_000m)
            .ComPrazoMeses(12)
            .ComTipoProduto(produtoCdb.Tipo)
            .Construir();

        var resposta = await casoDeUso.Executar(requisicao);

        resposta.ShouldNotBeNull();
        resposta.Simulacoes.Count.ShouldBe(2);
        var produtosSimulados = resposta.Simulacoes.Select(s => s.Produto.Id).ToList();
        produtosSimulados.ShouldContain(produtoCdb.Id);
        produtosSimulados.ShouldContain(produtoLci.Id);

        simulacaoRepositorio.SimulacoesAdicionadas.Count.ShouldBe(2);
        simulacaoRepositorio.SimulacoesAdicionadas.ShouldAllBe(s => s.ClienteId == cliente.Id);

        unidadeDeTrabalho.CommitFoiChamado.ShouldBeTrue();
    }

    [Fact]
    public async Task Deve_simular_produtos_sem_prazo_maximo_definido()
    {
        var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente Teste", Email = "teste@teste.com" };
        var clienteRepositorio = new ClienteRepositorioFalso().ComClienteExistente(cliente);

        var produtoSemPrazoMaximo = new ConstrutorProduto()
            .ComTipo("Tesouro Direto")
            .ComMinimoInvestimento(100)
            .ComPrazo(0, 0)
            .ComRentabilidade(0.06m)
            .Ativo(true)
            .Construir();

        var produtoRepositorio = new ProdutoRepositorioFalso().ComProdutos(new[] { produtoSemPrazoMaximo });
        var simulacaoRepositorio = new SimulacaoRepositorioFalso();
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var casoDeUso = CriarCasoDeUso(clienteRepositorio, produtoRepositorio, simulacaoRepositorio, unidadeDeTrabalho);

        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComClienteId(cliente.Id)
            .ComValor(produtoSemPrazoMaximo.MinimoInvestimento + 50)
            .ComPrazoMeses(24)
            .ComTipoProduto(produtoSemPrazoMaximo.Tipo)
            .Construir();

        var resposta = await casoDeUso.Executar(requisicao);

        resposta.Simulacoes.ShouldHaveSingleItem();
        resposta.Simulacoes.Single().Produto.Id.ShouldBe(produtoSemPrazoMaximo.Id);
    }

    [Fact]
    public async Task Deve_encontrar_produtos_quando_tipo_possuir_formatacao_diferente()
    {
        var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente Teste", Email = "teste@teste.com" };
        var clienteRepositorio = new ClienteRepositorioFalso().ComClienteExistente(cliente);

        var produto = new ConstrutorProduto()
            .ComTipo("Fundo (Renda Fixa)")
            .ComMinimoInvestimento(200)
            .ComPrazo(3, 24)
            .Ativo(true)
            .Construir();

        var produtoRepositorio = new ProdutoRepositorioFalso().ComProdutos(new[] { produto });
        var simulacaoRepositorio = new SimulacaoRepositorioFalso();
        var unidadeDeTrabalho = new UnidadeDeTrabalhoFalsa();
        var casoDeUso = CriarCasoDeUso(clienteRepositorio, produtoRepositorio, simulacaoRepositorio, unidadeDeTrabalho);

        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComClienteId(cliente.Id)
            .ComValor(500)
            .ComPrazoMeses(12)
            .ComTipoProduto("fundo-renda-fixa")
            .Construir();

        var resposta = await casoDeUso.Executar(requisicao);

        resposta.Simulacoes.ShouldHaveSingleItem();
        resposta.Simulacoes.Single().Produto.Id.ShouldBe(produto.Id);
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
