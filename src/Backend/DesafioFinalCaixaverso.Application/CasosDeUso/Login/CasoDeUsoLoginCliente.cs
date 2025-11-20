using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Dominio.Seguranca;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Login;

public class CasoDeUsoLoginCliente : ICasoDeUsoLoginCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IServicoHashSenha _servicoHashSenha;
    private readonly IGeradorTokenAcesso _geradorTokenAcesso;

    public CasoDeUsoLoginCliente(
        IClienteRepositorio clienteRepositorio,
        IServicoHashSenha servicoHashSenha,
        IGeradorTokenAcesso geradorTokenAcesso)
    {
        _clienteRepositorio = clienteRepositorio;
        _servicoHashSenha = servicoHashSenha;
        _geradorTokenAcesso = geradorTokenAcesso;
    }

    public async Task<ClienteAutenticadoJson> Executar(RequisicaoLoginClienteJson requisicao, CancellationToken cancellationToken = default)
    {
        var emailNormalizado = requisicao.Email.Trim().ToLowerInvariant();
        var cliente = await _clienteRepositorio.ObterPorEmailAsync(emailNormalizado, cancellationToken);

        if (cliente is null || !_servicoHashSenha.Validar(requisicao.Senha, cliente.Password))
            throw new CredenciaisInvalidasException(MensagensDeExcecao.AUTENTICACAO_CREDENCIAIS_INVALIDAS);

        var token = _geradorTokenAcesso.Gerar(cliente.Id);

        var resposta = cliente.Adapt<ClienteAutenticadoJson>();
        resposta.ClienteId = cliente.Id;
        resposta.Token = token.Adapt<TokenAcessoJson>();

        return resposta;
    }
}
