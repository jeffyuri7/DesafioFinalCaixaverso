using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Dominio.Seguranca;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using DesafioFinalCaixaverso.Exceptions;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class CasoDeUsoCadastrarCliente : ICasoDeUsoCadastrarCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
    private readonly IServicoHashSenha _servicoHashSenha;

    public CasoDeUsoCadastrarCliente(
        IClienteRepositorio clienteRepositorio,
        IUnidadeDeTrabalho unidadeDeTrabalho,
        IServicoHashSenha servicoHashSenha)
    {
        _clienteRepositorio = clienteRepositorio;
        _unidadeDeTrabalho = unidadeDeTrabalho;
        _servicoHashSenha = servicoHashSenha;
    }

    public async Task<ClienteCadastradoJson> Executar(RequisicaoCadastroClienteJson requisicao, CancellationToken cancellationToken = default)
    {
        await Validar(requisicao);

        var emailNormalizado = requisicao.Email.Trim().ToLowerInvariant();

        var emailJaExiste = await _clienteRepositorio.EmailJaCadastradoAsync(emailNormalizado, cancellationToken);
        if (emailJaExiste)
            throw new ErroValidacaoException(new[] { MensagensDeExcecao.CLIENTE_EMAIL_JA_CADASTRADO });

        var senhaHash = _servicoHashSenha.Gerar(requisicao.Senha);

        var cliente = requisicao.Adapt<Cliente>();
        cliente.Id = Guid.NewGuid();
        cliente.Email = emailNormalizado;
        cliente.Password = senhaHash;
        cliente.DataCriacao = DateTime.UtcNow;

        await _clienteRepositorio.AdicionarAsync(cliente, cancellationToken);
        await _unidadeDeTrabalho.Commit();

        return cliente.Adapt<ClienteCadastradoJson>();
    }

    private static async Task Validar(RequisicaoCadastroClienteJson requisicao)
    {
        var validador = new ValidadorCadastroCliente();
        var resultado = await validador.ValidateAsync(requisicao);

        if (!resultado.IsValid)
        {
            var erros = resultado.Errors.Select(erro => erro.ErrorMessage).ToList();
            throw new ErroValidacaoException(erros);
        }
    }
}
