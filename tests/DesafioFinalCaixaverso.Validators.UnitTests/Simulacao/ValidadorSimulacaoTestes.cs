using System;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;

namespace DesafioFinalCaixaverso.Validators.UnitTests.Simulacao;

public class ValidadorSimulacaoTestes
{
    private readonly ValidadorSimulacao _validador = new();

    [Fact]
    public async Task Deve_retornar_erro_quando_cliente_id_for_vazio()
    {
        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComClienteId(Guid.Empty)
            .Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

    resultado.IsValid.ShouldBeFalse();
    resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.CLIENTE_ID_INVALIDO);
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_valor_for_menor_ou_igual_a_zero()
    {
        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComValor(0)
            .Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

    resultado.IsValid.ShouldBeFalse();
    resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.VALOR_SIMULACAO_MINIMO);
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_prazo_nao_estiver_no_intervalo()
    {
        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComPrazoMeses(70)
            .Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

    resultado.IsValid.ShouldBeFalse();
    resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.PRAZO_SIMULACAO_INVALIDO);
    }

    [Fact]
    public async Task Deve_retornar_erro_quando_tipo_produto_estiver_vazio()
    {
        var requisicao = new ConstrutorRequisicaoSimulacao()
            .ComTipoProduto(string.Empty)
            .Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

    resultado.IsValid.ShouldBeFalse();
    resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.TIPO_PRODUTO_OBRIGATORIO);
    }

    [Fact]
    public async Task Deve_validar_quando_requisicao_estiver_correta()
    {
        var requisicao = new ConstrutorRequisicaoSimulacao().Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

    resultado.IsValid.ShouldBeTrue();
    resultado.Errors.ShouldBeEmpty();
    }
}
