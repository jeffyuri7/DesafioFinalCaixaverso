using DesafioFinalCaixaverso.Dominio.Enumeradores;

namespace DesafioFinalCaixaverso.Aplicacao.Servicos.Perfis;

internal static class PerfilPontuacaoHelper
{
    private const decimal PesoVolume = 0.35m;
    private const decimal PesoFrequencia = 0.25m;
    private const decimal PesoRentabilidade = 0.2m;
    private const decimal PesoLiquidez = 0.2m;

    public static decimal CalcularNotaComportamental(decimal valorTotal, int quantidade, decimal rentabilidadeMedia, int liquidezMediaDias)
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

        return decimal.Round(
            (volumeNota * PesoVolume) +
            (frequenciaNota * PesoFrequencia) +
            (rentabilidadeNota * PesoRentabilidade) +
            (liquidezNota * PesoLiquidez),
            2,
            MidpointRounding.AwayFromZero);
    }

    public static PerfilInvestidor MapearPerfil(decimal pontuacaoFinal)
    {
        if (pontuacaoFinal <= 40)
            return PerfilInvestidor.Conservador;

        if (pontuacaoFinal <= 70)
            return PerfilInvestidor.Moderado;

        return PerfilInvestidor.Agressivo;
    }
}
