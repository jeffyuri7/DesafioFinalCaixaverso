using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using Mapster;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Clientes;

public class CasoDeUsoListarClientes : ICasoDeUsoListarClientes
{
    private readonly IClienteRepositorio _clienteRepositorio;

    public CasoDeUsoListarClientes(IClienteRepositorio clienteRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
    }

    public async Task<IReadOnlyCollection<ClienteCadastradoJson>> Executar(CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepositorio.ListarAsync(cancellationToken);
        var ordenados = clientes
            .OrderBy(cliente => cliente.Nome)
            .ToList();

        return ordenados.Adapt<IReadOnlyCollection<ClienteCadastradoJson>>();
    }
}
