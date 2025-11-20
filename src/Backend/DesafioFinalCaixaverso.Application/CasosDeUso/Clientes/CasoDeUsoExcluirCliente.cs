using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class CasoDeUsoExcluirCliente : ICasoDeUsoExcluirCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public CasoDeUsoExcluirCliente(IClienteRepositorio clienteRepositorio, IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _clienteRepositorio = clienteRepositorio;
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public async Task Executar(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);
        if (cliente is null)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        await _clienteRepositorio.RemoverAsync(cliente, cancellationToken);
        await _unidadeDeTrabalho.Commit();
    }
}
