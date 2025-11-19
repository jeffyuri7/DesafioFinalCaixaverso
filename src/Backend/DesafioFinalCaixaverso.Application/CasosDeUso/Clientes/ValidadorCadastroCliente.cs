using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Exceptions;
using FluentValidation;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class ValidadorCadastroCliente : AbstractValidator<RequisicaoCadastroClienteJson>
{
    public ValidadorCadastroCliente()
    {
        RuleFor(cliente => cliente.Nome)
            .NotEmpty()
            .WithMessage(MensagensDeExcecao.CLIENTE_NOME_OBRIGATORIO);

        RuleFor(cliente => cliente.Email)
            .NotEmpty().WithMessage(MensagensDeExcecao.CLIENTE_EMAIL_OBRIGATORIO)
            .EmailAddress().WithMessage(MensagensDeExcecao.CLIENTE_EMAIL_INVALIDO);

        RuleFor(cliente => cliente.Senha)
            .NotEmpty().WithMessage(MensagensDeExcecao.CLIENTE_SENHA_MINIMA)
            .MinimumLength(8).WithMessage(MensagensDeExcecao.CLIENTE_SENHA_MINIMA);
    }
}
