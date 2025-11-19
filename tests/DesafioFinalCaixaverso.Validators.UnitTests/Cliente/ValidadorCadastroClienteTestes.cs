using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using Shouldly;

namespace DesafioFinalCaixaverso.Validators.UnitTests.Cliente;

public class ValidadorCadastroClienteTestes
{
    private readonly ValidadorCadastroCliente _validador = new();

    [Fact]
    public async Task Deve_validar_requisicao_valida()
    {
        var requisicao = new ConstrutorRequisicaoCadastroCliente().Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

        resultado.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Deve_rejeitar_nome_vazio(string nome)
    {
        var requisicao = new ConstrutorRequisicaoCadastroCliente().ComNome(nome).Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

        resultado.IsValid.ShouldBeFalse();
        resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.CLIENTE_NOME_OBRIGATORIO);
    }

    [Fact]
    public async Task Deve_rejeitar_email_invalido()
    {
        var requisicao = new ConstrutorRequisicaoCadastroCliente().ComEmail("email-invalido").Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

        resultado.IsValid.ShouldBeFalse();
        resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.CLIENTE_EMAIL_INVALIDO);
    }

    [Fact]
    public async Task Deve_rejeitar_senha_curta()
    {
        var requisicao = new ConstrutorRequisicaoCadastroCliente().ComSenha("1234567").Construir();

        var resultado = await _validador.ValidateAsync(requisicao);

        resultado.IsValid.ShouldBeFalse();
        resultado.Errors.ShouldContain(erro => erro.ErrorMessage == MensagensDeExcecao.CLIENTE_SENHA_MINIMA);
    }
}
