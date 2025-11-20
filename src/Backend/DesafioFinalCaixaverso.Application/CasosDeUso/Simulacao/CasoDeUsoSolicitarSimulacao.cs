using System;
using System.Collections.Generic;
using System.Linq;
using DesafioFinalCaixaverso.Communications.Requests;
using DesafioFinalCaixaverso.Communications.Responses;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Repositorios;
using DesafioFinalCaixaverso.Exceptions;
using DesafioFinalCaixaverso.Exceptions.ExceptionsBase;
using Mapster;
using SimulacaoDominio = DesafioFinalCaixaverso.Dominio.Entidades.Simulacao;

namespace DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;

public class CasoDeUsoSolicitarSimulacao : ICasoDeUsoSolicitarSimulacao
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IProdutoRepositorio _produtoRepositorio;
    private readonly ISimulacaoRepositorio _simulacaoRepositorio;
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public CasoDeUsoSolicitarSimulacao(
        IClienteRepositorio clienteRepositorio,
        IProdutoRepositorio produtoRepositorio,
        ISimulacaoRepositorio simulacaoRepositorio,
        IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _clienteRepositorio = clienteRepositorio;
        _produtoRepositorio = produtoRepositorio;
        _simulacaoRepositorio = simulacaoRepositorio;
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public async Task<RespostaSimulacaoJson> Executar(RequisicaoSimulacaoJson requisicao)
    {
        await Validar(requisicao);

        var clienteExiste = await _clienteRepositorio.ExisteClienteAsync(requisicao.ClienteId);
        if (!clienteExiste)
            throw new NaoEncontradoException(MensagensDeExcecao.CLIENTE_NAO_ENCONTRADO);

        var produtosCompativeis = await ObterProdutosCompativeis(requisicao);

        var simulacoes = new List<(Produto produto, SimulacaoDominio simulacao)>();
        foreach (var produto in produtosCompativeis)
        {
            var simulacao = CriarSimulacao(requisicao, produto);
            simulacoes.Add((produto, simulacao));
            await _simulacaoRepositorio.AdicionarAsync(simulacao);
        }

        await _unidadeDeTrabalho.Commit();

        var resposta = new RespostaSimulacaoJson
        {
            Simulacoes = simulacoes
                .Select(par => new SimulacaoGeradaJson
                {
                    Produto = par.produto.Adapt<ProdutoSimuladoJson>(),
                    Resultado = par.simulacao.Adapt<ResultadoSimulacaoJson>(),
                    DataSimulacao = par.simulacao.DataSimulacao
                })
                .ToList()
        };

        return resposta;
    }

    private async Task<IReadOnlyCollection<Produto>> ObterProdutosCompativeis(RequisicaoSimulacaoJson requisicao)
    {
        var produtos = await _produtoRepositorio.ListarAtivosPorTipoAsync(requisicao.TipoProduto);

        var produtosCompativeis = produtos
            .Where(produto => ProdutoEhCompativel(produto, requisicao))
            .OrderByDescending(produto => produto.Rentabilidade)
            .ToList();

        if (produtosCompativeis.Count == 0)
            throw new NaoEncontradoException(MensagensDeExcecao.PRODUTO_NAO_ENCONTRADO);

        return produtosCompativeis.AsReadOnly();
    }

    private static bool ProdutoEhCompativel(Produto produto, RequisicaoSimulacaoJson requisicao)
    {
        var prazoMinimo = Math.Max(produto.PrazoMinimoMeses, 0);
        var prazoMaximo = produto.PrazoMaximoMeses <= 0 ? int.MaxValue : produto.PrazoMaximoMeses;

        var prazoEhValido = requisicao.PrazoMeses >= prazoMinimo && requisicao.PrazoMeses <= prazoMaximo;
        var valorEhSuficiente = requisicao.Valor >= produto.MinimoInvestimento;

        return prazoEhValido && valorEhSuficiente;
    }

    private static SimulacaoDominio CriarSimulacao(
        RequisicaoSimulacaoJson requisicao,
        Produto produtoSelecionado)
    {
        var prazoAnos = requisicao.PrazoMeses / 12m;
        var rentabilidadeEfetiva = produtoSelecionado.Rentabilidade * prazoAnos;
        var valorFinal = requisicao.Valor * (1 + rentabilidadeEfetiva);

        return new SimulacaoDominio
        {
            Id = Guid.NewGuid(),
            ClienteId = requisicao.ClienteId,
            ProdutoId = produtoSelecionado.Id,
            ValorInvestido = requisicao.Valor,
            ValorFinal = decimal.Round(valorFinal, 2, MidpointRounding.AwayFromZero),
            RentabilidadeEfetiva = decimal.Round(rentabilidadeEfetiva, 4, MidpointRounding.AwayFromZero),
            PrazoMeses = requisicao.PrazoMeses,
            DataSimulacao = DateTime.UtcNow
        };
    }

    private static async Task Validar(RequisicaoSimulacaoJson requisicao)
    {
        var validador = new ValidadorSimulacao();
        var resultado = await validador.ValidateAsync(requisicao);

        if (!resultado.IsValid)
        {
            var erros = resultado.Errors.Select(erro => erro.ErrorMessage).ToList();
            throw new ErroValidacaoException(erros);
        }
    }
}
