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
        var simulacoesUltimos30Dias = simulacoesPeriodo.Count(simulacao => simulacao.DataSimulacao >= dataCalculo.AddDays(-30));

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
        var pontuacao = CalcularNotaComportamental(valorTotal, quantidade, rentabilidadeMedia, liquidezMediaDias);

        var perfil = MapearPerfil(pontuacao);

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

    private static decimal CalcularNotaComportamental(decimal valorTotal, int quantidade, decimal rentabilidadeMedia, int liquidezMediaDias)
    {
        var volumeNota = valorTotal switch
        {
            >= 500000m => 95m,
            >= 100000m => 80m,
            >= 20000m => 60m,
            >= 5000m => 40m,
            > 0 => 25m,
            _ => 0m
        };

        var frequenciaNota = quantidade switch
        {
            >= 24 => 95m,
            >= 12 => 75m,
            >= 6 => 55m,
            >= 3 => 35m,
            > 0 => 20m,
            _ => 0m
        };

        var rentabilidadeNota = rentabilidadeMedia switch
        {
            >= 0.18m => 95m,
            >= 0.12m => 75m,
            >= 0.08m => 55m,
            >= 0.05m => 35m,
            > 0m => 20m,
            _ => 0m
        };

        var liquidezNota = liquidezMediaDias switch
        {
            >= 180 => 90m,
            >= 90 => 70m,
            >= 30 => 50m,
            >= 7 => 30m,
            > 0 => 15m,
            _ => 0m
        };

        const decimal pesoVolume = 0.35m;
        const decimal pesoFrequencia = 0.25m;
        const decimal pesoRentabilidade = 0.2m;
        const decimal pesoLiquidez = 0.2m;

        return decimal.Round(
            (volumeNota * pesoVolume) +
            (frequenciaNota * pesoFrequencia) +
            (rentabilidadeNota * pesoRentabilidade) +
            (liquidezNota * pesoLiquidez),
            2,
            MidpointRounding.AwayFromZero);
    }

    private static PerfilInvestidor MapearPerfil(decimal pontuacaoFinal)
    {
        if (pontuacaoFinal <= 40)
            return PerfilInvestidor.Conservador;

        if (pontuacaoFinal <= 70)
            return PerfilInvestidor.Moderado;

        return PerfilInvestidor.Agressivo;
    }
}
