using System;
using System.Threading;
using System.Threading.Tasks;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Enumeradores;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Perfil;

public class CasoDeUsoConsultarPerfilDinamicoCliente : ICasoDeUsoConsultarPerfilDinamicoCliente
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IClientePerfilDinamicoRepositorio _clientePerfilDinamicoRepositorio;

    public CasoDeUsoConsultarPerfilDinamicoCliente(
        IClienteRepositorio clienteRepositorio,
        IClientePerfilDinamicoRepositorio clientePerfilDinamicoRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
        _clientePerfilDinamicoRepositorio = clientePerfilDinamicoRepositorio;
    }

    public async Task<PerfilClienteResumoJson> Executar(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(clienteId);

        if (cliente is null)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        var perfil = await _clientePerfilDinamicoRepositorio.ObterPorClienteAsync(clienteId, cancellationToken);

        if (perfil is null)
        {
            return new PerfilClienteResumoJson
            {
                ClienteId = clienteId,
                Perfil = nameof(PerfilInvestidor.NaoClassificado),
                Pontuacao = 0,
                Descricao = string.Empty
            };
        }

        return new PerfilClienteResumoJson
        {
            ClienteId = clienteId,
            Perfil = perfil.Perfil.ToString(),
            Pontuacao = (int)Math.Round(perfil.Pontuacao, MidpointRounding.AwayFromZero),
            Descricao = string.Empty
        };
    }
}
