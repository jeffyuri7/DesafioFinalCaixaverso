using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Exceptions;
using Shouldly;

namespace DesafioFinalCaixaverso.Validators.UnitTests.Cliente;

public class ValidadorAtualizacaoClienteTestes
{
	private readonly ValidadorAtualizacaoCliente _validador = new();

	[Fact]
	public async Task Deve_validar_requisicao_valida()
	{
		var requisicao = CriarRequisicao();

		var resultado = await _validador.ValidateAsync(requisicao);

		resultado.IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public async Task Deve_rejeitar_nome_vazio(string nome)
	{
		var requisicao = CriarRequisicao(nome: nome);

		var resultado = await _validador.ValidateAsync(requisicao);

		resultado.IsValid.ShouldBeFalse();
		resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.CLIENTE_NOME_OBRIGATORIO);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public async Task Deve_rejeitar_email_vazio(string email)
	{
		var requisicao = CriarRequisicao(email: email);

		var resultado = await _validador.ValidateAsync(requisicao);

		resultado.IsValid.ShouldBeFalse();
		resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.CLIENTE_EMAIL_OBRIGATORIO);
	}

	[Fact]
	public async Task Deve_rejeitar_email_invalido()
	{
		var requisicao = CriarRequisicao(email: "email-invalido");

		var resultado = await _validador.ValidateAsync(requisicao);

		resultado.IsValid.ShouldBeFalse();
		resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.CLIENTE_EMAIL_INVALIDO);
	}

	[Fact]
	public async Task Deve_permitir_senha_nao_informada()
	{
		var requisicao = CriarRequisicao(senha: null);

		var resultado = await _validador.ValidateAsync(requisicao);

		resultado.IsValid.ShouldBeTrue();
	}

	[Fact]
	public async Task Deve_rejeitar_senha_curta_quando_informada()
	{
		var requisicao = CriarRequisicao(senha: "1234567");

		var resultado = await _validador.ValidateAsync(requisicao);

		resultado.IsValid.ShouldBeFalse();
		resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.CLIENTE_SENHA_MINIMA);
	}

	private static RequisicaoAtualizacaoClienteJson CriarRequisicao(string nome = "Cliente Atualizado", string email = "cliente@teste.com", string? senha = null)
		=> new()
		{
			Nome = nome,
			Email = email,
			Senha = senha
		};
}
