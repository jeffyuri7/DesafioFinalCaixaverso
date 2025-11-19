using System;
using System.Collections.Generic;
using DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;
using ProdutoEntidade = DesafioFinalCaixaverso.Dominio.Entidades.Produto;
using SimulacaoEntidade = DesafioFinalCaixaverso.Dominio.Entidades.Simulacao;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Perfil;

public class CalculadoraPerfilInvestidorTestes
{
    [Fact]
    public void Deve_classificar_agressivo_com_dados_completos()
    {
        var clienteId = Guid.NewGuid();
        var simulacoes = CriarSimulacoesComportamentoRobusto(clienteId);
        var questionario = new ConstrutorQuestionarioInvestidor()
            .ComClienteId(clienteId)
            .ComPreferenciaLiquidez(PreferenciaLiquidez.Baixa)
            .ComObjetivoInvestimento(ObjetivoInvestimento.Crescimento)
            .ComNivelConhecimento(NivelConhecimentoInvestidor.Avancado)
            .ComHorizonteMeses(72)
            .ComToleranciaPerdaPercentual(30m)
            .ComRendaMensal(25_000m)
            .ComPatrimonioTotal(1_500_000m)
            .Construir();

        var calculadora = new CalculadoraPerfilInvestidor();

        var resultado = calculadora.Calcular(clienteId, simulacoes, questionario);

        resultado.Perfil.ShouldBe(PerfilInvestidor.Agressivo);
        resultado.PermiteRecomendacao.ShouldBeTrue();
        resultado.DadosSuficientes.ShouldBeTrue();
        resultado.MetodoCalculo.ShouldBe("motor_v2_compliance");
        resultado.PontuacaoQuestionario.ShouldBeGreaterThan(80m);
        resultado.PontuacaoComportamental.ShouldBeGreaterThan(60m);
    }

    [Fact]
    public void Deve_bloquear_recomendacao_quando_questionario_inexistente()
    {
        var clienteId = Guid.NewGuid();
        var simulacoes = CriarSimulacoesComportamentoRobusto(clienteId);
        var calculadora = new CalculadoraPerfilInvestidor();

        var resultado = calculadora.Calcular(clienteId, simulacoes, null);

        resultado.Perfil.ShouldBe(PerfilInvestidor.NaoClassificado);
        resultado.PermiteRecomendacao.ShouldBeFalse();
        resultado.DadosSuficientes.ShouldBeTrue();
        resultado.MetodoCalculo.ShouldBe("motor_v2_comportamental_parcial");
        resultado.PontuacaoComportamental.ShouldBeGreaterThan(0);
        resultado.Observacoes.ShouldContain("Questionário obrigatório");
    }

    [Fact]
    public void Deve_retornar_nao_classificado_quando_nao_existir_dado()
    {
        var clienteId = Guid.NewGuid();
        var calculadora = new CalculadoraPerfilInvestidor();

    var resultado = calculadora.Calcular(clienteId, Array.Empty<SimulacaoEntidade>(), null);

        resultado.Perfil.ShouldBe(PerfilInvestidor.NaoClassificado);
        resultado.DadosSuficientes.ShouldBeFalse();
        resultado.PermiteRecomendacao.ShouldBeFalse();
        resultado.Pontuacao.ShouldBe(0);
        resultado.Observacoes.ShouldContain("Questionário e histórico indisponíveis");
    }

    private static IReadOnlyCollection<SimulacaoEntidade> CriarSimulacoesComportamentoRobusto(Guid clienteId)
    {
        var produto = new ConstrutorProduto()
            .ComRentabilidade(0.12m)
            .ComLiquidezDias(120)
            .Construir();

        var simulacoes = new List<SimulacaoEntidade>();
        var dataReferencia = DateTime.UtcNow;

        for (var i = 0; i < 12; i++)
        {
            simulacoes.Add(CriarSimulacao(
                clienteId,
                produto,
                20_000m + (i * 2_000m),
                dataReferencia.AddMonths(-i)));
        }

        return simulacoes;
    }

    private static SimulacaoEntidade CriarSimulacao(Guid clienteId, ProdutoEntidade produto, decimal valorInvestido, DateTime dataSimulacao)
    {
        return new SimulacaoEntidade
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            ProdutoId = produto.Id,
            Produto = produto,
            ValorInvestido = valorInvestido,
            ValorFinal = valorInvestido * 1.1m,
            RentabilidadeEfetiva = produto.Rentabilidade,
            PrazoMeses = 12,
            DataSimulacao = dataSimulacao
        };
    }
}
