using System.Threading.Tasks;
using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Login;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using DesafioFinalCaixaverso.TestUtilities.Construtores;
using DesafioFinalCaixaverso.UseCases.UnitTests.Dubles;
using Shouldly;
using Xunit;

namespace DesafioFinalCaixaverso.UseCases.UnitTests.Login;

public class CasoDeUsoLoginClienteTestes
{
    private readonly ClienteRepositorioFalso _clienteRepositorio = new();
    private readonly ServicoHashSenhaFalso _servicoHashSenha = new();
    private readonly GeradorTokenAcessoFalso _geradorTokenAcesso = new();

    [Fact]
    public async Task Deve_gerar_token_quando_credenciais_validas()
    {
        var requisicao = new ConstrutorRequisicaoLoginCliente().Construir();
        var cliente = new ConstrutorCliente()
            .ComEmail(requisicao.Email)
            .ComSenhaHash("hash::senha")
            .Construir();

        _clienteRepositorio.ComClienteExistente(cliente);
        _servicoHashSenha.ResultadoValidacao = true;

        var casoDeUso = new CasoDeUsoLoginCliente(_clienteRepositorio, _servicoHashSenha, _geradorTokenAcesso);

        var resposta = await casoDeUso.Executar(requisicao);

        resposta.ShouldNotBeNull();
        resposta.ClienteId.ShouldBe(cliente.Id);
        resposta.Nome.ShouldBe(cliente.Nome);
        resposta.Token.Valor.ShouldBe(_geradorTokenAcesso.TokenRetornado.Valor);
        _geradorTokenAcesso.ClienteSolicitado.ShouldBe(cliente.Id);
    }

    [Fact]
    public async Task Deve_lancar_excecao_quando_senha_invalida()
    {
        var requisicao = new ConstrutorRequisicaoLoginCliente().Construir();
        var cliente = new ConstrutorCliente()
            .ComEmail(requisicao.Email)
            .ComSenhaHash("hash::senha")
            .Construir();

        _clienteRepositorio.ComClienteExistente(cliente);
        _servicoHashSenha.ResultadoValidacao = false;

        var casoDeUso = new CasoDeUsoLoginCliente(_clienteRepositorio, _servicoHashSenha, _geradorTokenAcesso);

        var excecao = await Should.ThrowAsync<CredenciaisInvalidasException>(() => casoDeUso.Executar(requisicao));
        excecao.Message.ShouldBe(MensagensDeExcecao.AUTENTICACAO_CREDENCIAIS_INVALIDAS);
    }
}
