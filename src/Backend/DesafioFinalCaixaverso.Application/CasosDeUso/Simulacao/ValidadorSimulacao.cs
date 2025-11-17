using System;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Exceptions;
using FluentValidation;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public class ValidadorSimulacao : AbstractValidator<RequisicaoSimulacaoJson>
{
    public ValidadorSimulacao()
    {
        RuleFor(simulacao => simulacao.ClienteId)
            .NotEqual(Guid.Empty)
            .WithMessage(MensagensDeExcecao.CLIENTE_ID_INVALIDO);
        RuleFor(simulacao => simulacao.Valor)
            .GreaterThan(0)
            .WithMessage(MensagensDeExcecao.VALOR_SIMULACAO_MINIMO);
        RuleFor(simulacao => simulacao.PrazoMeses)
            .InclusiveBetween(1, 60)
            .WithMessage(MensagensDeExcecao.PRAZO_SIMULACAO_INVALIDO);
        RuleFor(simulacao => simulacao.TipoProduto)
            .NotEmpty()
            .WithMessage(MensagensDeExcecao.TIPO_PRODUTO_OBRIGATORIO);
    }
}
