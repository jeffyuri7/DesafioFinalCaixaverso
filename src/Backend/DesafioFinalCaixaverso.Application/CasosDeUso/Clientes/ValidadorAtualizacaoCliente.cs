using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Exceptions;
using FluentValidation;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class ValidadorAtualizacaoCliente : AbstractValidator<RequisicaoAtualizacaoClienteJson>
{
    public ValidadorAtualizacaoCliente()
    {
        RuleFor(cliente => cliente.Nome)
            .NotEmpty()
            .WithMessage(MensagensDeExcecao.CLIENTE_NOME_OBRIGATORIO);

        RuleFor(cliente => cliente.Email)
            .NotEmpty().WithMessage(MensagensDeExcecao.CLIENTE_EMAIL_OBRIGATORIO)
            .EmailAddress().WithMessage(MensagensDeExcecao.CLIENTE_EMAIL_INVALIDO);

        When(cliente => !string.IsNullOrWhiteSpace(cliente.Senha), () =>
        {
            RuleFor(cliente => cliente.Senha)
                .MinimumLength(8)
                .WithMessage(MensagensDeExcecao.CLIENTE_SENHA_MINIMA);
        });
    }
}
