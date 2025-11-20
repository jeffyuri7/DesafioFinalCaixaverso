using System;
using DesafioFinalCaixaverso.Dominio.Entidades;

namespace DesafioFinalCaixaverso.TestUtilities.Construtores;

public class ConstrutorTelemetriaServico
{
    private readonly TelemetriaServico _telemetriaServico;

    public ConstrutorTelemetriaServico()
    {
        var agora = DateTime.UtcNow;

        _telemetriaServico = new TelemetriaServico
        {
            Id = Guid.NewGuid(),
            Servico = "servico-api",
            AnoReferencia = agora.Year,
            MesReferencia = agora.Month,
            QuantidadeChamadas = 1,
            TempoTotalRespostaMs = 250,
            UltimaChamada = agora
        };
    }

    public ConstrutorTelemetriaServico ComServico(string servico)
    {
        _telemetriaServico.Servico = servico;
        return this;
    }

    public ConstrutorTelemetriaServico ComAno(int ano)
    {
        _telemetriaServico.AnoReferencia = ano;
        return this;
    }

    public ConstrutorTelemetriaServico ComMes(int mes)
    {
        _telemetriaServico.MesReferencia = mes;
        return this;
    }

    public ConstrutorTelemetriaServico ComQuantidade(int quantidade)
    {
        _telemetriaServico.QuantidadeChamadas = quantidade;
        return this;
    }

    public ConstrutorTelemetriaServico ComTempoTotal(long tempoRespostaMs)
    {
        _telemetriaServico.TempoTotalRespostaMs = tempoRespostaMs;
        return this;
    }

    public ConstrutorTelemetriaServico ComUltimaChamada(DateTime ultimaChamada)
    {
        _telemetriaServico.UltimaChamada = ultimaChamada;
        return this;
    }

    public TelemetriaServico Construir() => _telemetriaServico;
}
