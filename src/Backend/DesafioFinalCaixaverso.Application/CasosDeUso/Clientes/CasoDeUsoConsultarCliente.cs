using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class CasoDeUsoConsultarCliente : ICasoDeUsoConsultarCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;

    public CasoDeUsoConsultarCliente(IClienteRepositorio clienteRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
    }

    public async Task<ClienteCadastradoJson> Executar(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);

        if (cliente is null)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        return cliente.Adapt<ClienteCadastradoJson>();
    }
}
