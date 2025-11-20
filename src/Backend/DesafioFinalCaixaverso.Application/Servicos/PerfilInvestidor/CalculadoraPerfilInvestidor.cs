using System;
using System.Collections.Generic;
using System.Linq;
using DesafioFinalCaixaverso.Dominio.Entidades;
using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;

public class CalculadoraPerfilInvestidor : ICalculadoraPerfilInvestidor
{
    private const decimal PesoQuestionario = 0.6m;
    private const decimal PesoComportamental = 0.4m;

    public PerfilInvestidorResultado Calcular(
        Guid clienteId,
        IReadOnlyCollection<Simulacao> simulacoes,
        QuestionarioInvestidor? questionario)
    {
        var dataCalculo = DateTime.UtcNow;

        var comportamento = CalcularDimensaoComportamental(simulacoes, dataCalculo);
        var questionarioDimensao = CalcularDimensaoQuestionario(questionario);

        var possuiDados = comportamento.Disponivel || questionarioDimensao.Disponivel;
        var permiteRecomendacao = questionarioDimensao.Disponivel;

        if (!possuiDados)
        {
            return PerfilNaoClassificado(clienteId, dataCalculo, "Questionário e histórico indisponíveis. Impossível classificar conforme Art.23 ANBIMA.");
        }

        var pesoQuestionarioEfetivo = questionarioDimensao.Disponivel ? PesoQuestionario : 0m;
        var pesoComportamentalEfetivo = comportamento.Disponivel ? PesoComportamental : 0m;
        var somaPesos = pesoQuestionarioEfetivo + pesoComportamentalEfetivo;

        var pontuacaoFinal = somaPesos == 0
            ? 0
            : decimal.Round(
                (questionarioDimensao.Pontuacao * pesoQuestionarioEfetivo) +
                (comportamento.Pontuacao * pesoComportamentalEfetivo),
                2,
                MidpointRounding.AwayFromZero);

        var perfilCalculado = MapearPerfil(pontuacaoFinal);
        var perfilFinal = permiteRecomendacao ? perfilCalculado : PerfilInvestidor.NaoClassificado;

        var observacao = permiteRecomendacao
            ? string.Empty
            : "Questionário obrigatório não encontrado ou desatualizado. Reforçar coleta antes de recomendar (ANBIMA Art.23).";

        return new PerfilInvestidorResultado
        {
            ClienteId = clienteId,
            Perfil = perfilFinal,
            Pontuacao = (int)Math.Round(pontuacaoFinal, MidpointRounding.AwayFromZero),
            ValorMedioInvestido = comportamento.ValorMedioInvestido,
            ValorTotalInvestido = comportamento.ValorTotalInvestido,
            SimulacoesUltimos30Dias = comportamento.SimulacoesUltimos30Dias,
            RentabilidadeMediaProduto = comportamento.RentabilidadeMedia,
            LiquidezMediaDias = comportamento.LiquidezMediaDias,
            AtualizadoEm = dataCalculo,
            PontuacaoComportamental = comportamento.Pontuacao,
            PontuacaoQuestionario = questionarioDimensao.Pontuacao,
            PermiteRecomendacao = permiteRecomendacao,
            MetodoCalculo = permiteRecomendacao ? "motor_v2_compliance" : "motor_v2_comportamental_parcial",
            Observacoes = observacao,
            DadosSuficientes = possuiDados
        };
    }

    private static PerfilInvestidorResultado PerfilNaoClassificado(Guid clienteId, DateTime dataCalculo, string observacao)
    {
        return new PerfilInvestidorResultado
        {
            ClienteId = clienteId,
            Perfil = PerfilInvestidor.NaoClassificado,
            Pontuacao = 0,
            ValorMedioInvestido = 0,
            ValorTotalInvestido = 0,
            SimulacoesUltimos30Dias = 0,
            RentabilidadeMediaProduto = 0,
            LiquidezMediaDias = 0,
            AtualizadoEm = dataCalculo,
            PontuacaoComportamental = 0,
            PontuacaoQuestionario = 0,
            PermiteRecomendacao = false,
            MetodoCalculo = "motor_v2_sem_dados",
            Observacoes = observacao,
            DadosSuficientes = false
        };
    }

    private static DimensaoComportamental CalcularDimensaoComportamental(IReadOnlyCollection<Simulacao> simulacoes, DateTime dataCalculo)
    {
        var periodoAnalise = dataCalculo.AddMonths(-12);
        var simulacoesPeriodo = simulacoes
            .Where(simulacao => simulacao.DataSimulacao >= periodoAnalise)
            .ToList();

    if (simulacoesPeriodo.Count == 0)
        {
            return DimensaoComportamental.Indisponivel();
        }

        var valorTotal = simulacoesPeriodo.Sum(simulacao => simulacao.ValorInvestido);
        var quantidade = simulacoesPeriodo.Count;
        var valorMedio = decimal.Round(valorTotal / quantidade, 2, MidpointRounding.AwayFromZero);
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

        var nota = CalcularNotaComportamental(valorTotal, quantidade, rentabilidadeMedia, liquidezMediaDias);

        return new DimensaoComportamental(
            true,
            nota,
            valorMedio,
            decimal.Round(valorTotal, 2, MidpointRounding.AwayFromZero),
            simulacoesUltimos30Dias,
            rentabilidadeMedia,
            liquidezMediaDias);
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

    private static DimensaoQuestionario CalcularDimensaoQuestionario(QuestionarioInvestidor? questionario)
    {
        if (questionario is null)
            return DimensaoQuestionario.Indisponivel();

        var liquidezNota = questionario.PreferenciaLiquidez switch
        {
            PreferenciaLiquidez.Baixa => 90m,
            PreferenciaLiquidez.Media => 50m,
            _ => 20m
        };

        var horizonteNota = questionario.HorizonteMeses switch
        {
            >= 60 => 90m,
            >= 24 => 70m,
            >= 12 => 50m,
            >= 6 => 30m,
            _ => 10m
        };

        var toleranciaNota = questionario.ToleranciaPerdaPercentual switch
        {
            >= 25m => 95m,
            >= 15m => 75m,
            >= 5m => 50m,
            > 0m => 25m,
            _ => 5m
        };

        var conhecimentoNota = questionario.NivelConhecimento switch
        {
            NivelConhecimentoInvestidor.Avancado => 90m,
            NivelConhecimentoInvestidor.Intermediario => 65m,
            _ => 30m
        };

        var objetivoNota = questionario.ObjetivoInvestimento switch
        {
            ObjetivoInvestimento.Crescimento => 85m,
            ObjetivoInvestimento.Equilibrio => 60m,
            ObjetivoInvestimento.Renda => 45m,
            _ => 25m
        };

        var situacaoFinanceiraNota = CalcularSituacaoFinanceira(questionario.RendaMensal, questionario.PatrimonioTotal, questionario.FonteRendaEstavel);

        const decimal pesoLiquidez = 0.15m;
        const decimal pesoHorizonte = 0.15m;
        const decimal pesoTolerancia = 0.25m;
        const decimal pesoConhecimento = 0.2m;
        const decimal pesoObjetivo = 0.15m;
        const decimal pesoSituacao = 0.1m;

        var pontuacao = decimal.Round(
            (liquidezNota * pesoLiquidez) +
            (horizonteNota * pesoHorizonte) +
            (toleranciaNota * pesoTolerancia) +
            (conhecimentoNota * pesoConhecimento) +
            (objetivoNota * pesoObjetivo) +
            (situacaoFinanceiraNota * pesoSituacao),
            2,
            MidpointRounding.AwayFromZero);

        return new DimensaoQuestionario(true, pontuacao);
    }

    private static decimal CalcularSituacaoFinanceira(decimal rendaMensal, decimal patrimonioTotal, bool fonteRendaEstavel)
    {
        var rendaNota = rendaMensal switch
        {
            >= 20000m => 85m,
            >= 8000m => 65m,
            >= 4000m => 45m,
            >= 2000m => 30m,
            _ => 15m
        };

        var patrimonioNota = patrimonioTotal switch
        {
            >= 1000000m => 95m,
            >= 300000m => 75m,
            >= 100000m => 55m,
            >= 20000m => 35m,
            _ => 15m
        };

        var estabilidadeNota = fonteRendaEstavel ? 70m : 30m;

        return decimal.Round((rendaNota * 0.4m) + (patrimonioNota * 0.4m) + (estabilidadeNota * 0.2m), 2, MidpointRounding.AwayFromZero);
    }

    private static PerfilInvestidor MapearPerfil(decimal pontuacaoFinal)
    {
        if (pontuacaoFinal <= 40)
            return PerfilInvestidor.Conservador;

        if (pontuacaoFinal <= 70)
            return PerfilInvestidor.Moderado;

        return PerfilInvestidor.Agressivo;
    }

    private readonly record struct DimensaoComportamental(
        bool Disponivel,
        decimal Pontuacao,
        decimal ValorMedioInvestido,
        decimal ValorTotalInvestido,
        int SimulacoesUltimos30Dias,
        decimal RentabilidadeMedia,
        int LiquidezMediaDias)
    {
        public static DimensaoComportamental Indisponivel() => new(false, 0, 0, 0, 0, 0, 0);
    }

    private readonly record struct DimensaoQuestionario(bool Disponivel, decimal Pontuacao)
    {
        public static DimensaoQuestionario Indisponivel() => new(false, 0);
    }
}
