using DesafioFinalCaixaverso.Aplicacao.CasosDeUso.Simulacao;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioFinalCaixaverso.Aplicacao;

public static class InjecaoDeDependencia
{
    public static void RegistrarAplicacao(this IServiceCollection services)
    {
        AdicionarCasosDeUso(services);
    }

    public static void AdicionarCasosDeUso(IServiceCollection services)
    {
        services.AddScoped<ICasoDeUsoSolicitarSimulacao, CasoDeUsoSolicitarSimulacao>();
    }
}
