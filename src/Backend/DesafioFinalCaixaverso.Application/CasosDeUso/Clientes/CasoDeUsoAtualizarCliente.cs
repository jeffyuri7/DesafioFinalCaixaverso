using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Dominio.Seguranca;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class CasoDeUsoAtualizarCliente : ICasoDeUsoAtualizarCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
    private readonly IServicoHashSenha _servicoHashSenha;

    public CasoDeUsoAtualizarCliente(
        IClienteRepositorio clienteRepositorio,
        IUnidadeDeTrabalho unidadeDeTrabalho,
        IServicoHashSenha servicoHashSenha)
    {
        _clienteRepositorio = clienteRepositorio;
        _unidadeDeTrabalho = unidadeDeTrabalho;
        _servicoHashSenha = servicoHashSenha;
    }

    public async Task<ClienteCadastradoJson> Executar(Guid clienteId, RequisicaoAtualizacaoClienteJson requisicao, CancellationToken cancellationToken = default)
    {
        await Validar(requisicao);

        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);
        if (cliente is null)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        var emailNormalizado = requisicao.Email.Trim().ToLowerInvariant();
        var nomeNormalizado = requisicao.Nome.Trim();

        var emailMudou = !string.Equals(cliente.Email, emailNormalizado, StringComparison.OrdinalIgnoreCase);
        if (emailMudou)
        {
            var emailEmUso = await _clienteRepositorio.EmailJaCadastradoAsync(emailNormalizado, cancellationToken);
            if (emailEmUso)
                throw new ErroValidacaoException(new[] { MensagensDeExcecao.CLIENTE_EMAIL_JA_CADASTRADO });
        }

        cliente.Nome = nomeNormalizado;
        cliente.Email = emailNormalizado;

        if (!string.IsNullOrWhiteSpace(requisicao.Senha))
            cliente.Password = _servicoHashSenha.Gerar(requisicao.Senha);

        await _clienteRepositorio.AtualizarAsync(cliente, cancellationToken);
        await _unidadeDeTrabalho.Commit();

        return cliente.Adapt<ClienteCadastradoJson>();
    }

    private static async Task Validar(RequisicaoAtualizacaoClienteJson requisicao)
    {
        var validador = new ValidadorAtualizacaoCliente();
        var resultado = await validador.ValidateAsync(requisicao);

        if (!resultado.IsValid)
        {
            var erros = resultado.Errors.Select(erro => erro.ErrorMessage).ToList();
            throw new ErroValidacaoException(erros);
        }
    }
}
