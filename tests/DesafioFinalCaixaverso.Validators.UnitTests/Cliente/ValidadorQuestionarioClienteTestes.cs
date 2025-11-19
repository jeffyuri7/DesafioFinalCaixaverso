using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;

namespace DesafioFinalCaixaverso.Validators.UnitTests.Cliente;

public class ValidadorQuestionarioClienteTestes
{
    private readonly ValidadorQuestionarioCliente _validador = new();

    [Fact]
    public async Task Deve_validar_requisicao_valida()
    {
        var requisicao = new ConstrutorRequisicaoQuestionarioCliente().Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

        resultado.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Deve_rejeitar_horizonte_invalido(int horizonte)
    {
        var requisicao = new ConstrutorRequisicaoQuestionarioCliente().ComHorizonte(horizonte).Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

        resultado.IsValid.ShouldBeFalse();
        resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.QUESTIONARIO_HORIZONTE_INVALIDO);
    }

    [Fact]
    public async Task Deve_rejeitar_renda_negativa()
    {
        var requisicao = new ConstrutorRequisicaoQuestionarioCliente().ComRenda(-1).Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

        resultado.IsValid.ShouldBeFalse();
        resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.QUESTIONARIO_RENDA_INVALIDA);
    }

    [Fact]
    public async Task Deve_rejeitar_tolerancia_fora_do_intervalo()
    {
        var requisicao = new ConstrutorRequisicaoQuestionarioCliente().ComTolerancia(150).Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

        resultado.IsValid.ShouldBeFalse();
        resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.QUESTIONARIO_TOLERANCIA_INVALIDA);
    }
}
