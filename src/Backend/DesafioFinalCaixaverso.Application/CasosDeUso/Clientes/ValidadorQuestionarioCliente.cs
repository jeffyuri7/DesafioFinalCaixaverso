using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Exceptions;
using FluentValidation;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class ValidadorQuestionarioCliente : AbstractValidator<RequisicaoQuestionarioClienteJson>
{
    public ValidadorQuestionarioCliente()
    {
        RuleFor(questionario => questionario.PreferenciaLiquidez)
            .IsInEnum();

        RuleFor(questionario => questionario.ObjetivoInvestimento)
            .IsInEnum();

        RuleFor(questionario => questionario.NivelConhecimento)
            .IsInEnum();

        RuleFor(questionario => questionario.HorizonteMeses)
            .GreaterThan(0)
            .WithMessage(MensagensDeExcecao.QUESTIONARIO_HORIZONTE_INVALIDO);

        RuleFor(questionario => questionario.RendaMensal)
            .GreaterThanOrEqualTo(0)
            .WithMessage(MensagensDeExcecao.QUESTIONARIO_RENDA_INVALIDA);

        RuleFor(questionario => questionario.PatrimonioTotal)
            .GreaterThanOrEqualTo(0)
            .WithMessage(MensagensDeExcecao.QUESTIONARIO_PATRIMONIO_INVALIDO);

        RuleFor(questionario => questionario.ToleranciaPerdaPercentual)
            .InclusiveBetween(0, 100)
            .WithMessage(MensagensDeExcecao.QUESTIONARIO_TOLERANCIA_INVALIDA);
    }
}
