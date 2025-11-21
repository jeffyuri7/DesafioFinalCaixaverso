using System;
using System.Collections.Generic;
using System.Linq;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;

public class CalculadoraPerfilDinamico : ICalculadoraPerfilDinamico
{
    public ClientePerfilDinamico Calcular(Guid clienteId, IReadOnlyCollection<Simulacao> simulacoes)
    {
        var dataCalculo = DateTime.UtcNow;
        var periodoAnalise = dataCalculo.AddMonths(-12);
        var simulacoesPeriodo = simulacoes
            .Where(simulacao => simulacao.DataSimulacao >= periodoAnalise)
            .ToList();

    var valorTotal = simulacoesPeriodo.Sum(simulacao => simulacao.ValorInvestido);
    var quantidade = simulacoesPeriodo.Count;

        var produtos = simulacoesPeriodo
            .Where(simulacao => simulacao.Produto is not null)
            .Select(simulacao => simulacao.Produto!)
            .ToList();

        var rentabilidadeMedia = produtos.Count > 0
            ? decimal.Round(produtos.Average(produto => produto.Rentabilidade), 4, MidpointRounding.AwayFromZero)
            : 0;

        var liquidezMediaDias = produtos.Count > 0
            ? (int)Math.Round(produtos.Average(produto => produto.LiquidezDias), MidpointRounding.AwayFromZero)
            : 0;

        // Mesma lógica de pontuação do perfil comportamental
    var pontuacao = PerfilPontuacaoHelper.CalcularNotaComportamental(valorTotal, quantidade, rentabilidadeMedia, liquidezMediaDias);

    var perfil = PerfilPontuacaoHelper.MapearPerfil(pontuacao);

        return new ClientePerfilDinamico
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Perfil = perfil,
            Pontuacao = pontuacao,
            VolumeTotalInvestido = decimal.Round(valorTotal, 2, MidpointRounding.AwayFromZero),
            FrequenciaMovimentacoes = quantidade,
            PreferenciaLiquidez = liquidezMediaDias < 30,
            AtualizadoEm = dataCalculo
        };
    }
}
