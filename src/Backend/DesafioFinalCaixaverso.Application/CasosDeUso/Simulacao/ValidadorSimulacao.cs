using DesafioFinalCaixaverso.Communications.Requests;
using FluentValidation;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public class ValidadorSimulacao : AbstractValidator<RequisicaoSimulacaoJson>
{
    public ValidadorSimulacao()
    {
        RuleFor(simulacao => simulacao.ClienteId)
            .GreaterThan(0).WithMessage("O valor final deve ser maior que zero.");
        RuleFor(simulacao => simulacao.Valor)
            .GreaterThan(0).WithMessage("O valor do empréstimo deve ser maior que zero.");
        RuleFor(simulacao => simulacao.PrazoMeses)
            .InclusiveBetween(1, 60).WithMessage("O prazo deve ser entre 1 e 60 meses.");
        RuleFor(simulacao => simulacao.TipoProduto)
            .NotEmpty().WithMessage("O tipo de produto não pode ser vazio.");
    }
}
